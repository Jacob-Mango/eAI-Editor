using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;

namespace eAIEditor
{
    public class FSM : ViewModelBase
    {
        public MainCanvasContext Root;
        public ObservableCollection<ViewModelBase> Items { get; }

        public ObservableCollection<FSMState> States { get; }
        public ObservableCollection<FSMTransition> Transitions { get; }

        private string _fileName;
        public string FileName
        {
            set
            {
                _fileName = value;
                OnPropertyChanged();
            }
            get => _fileName;
        }

        private string _name;
        public string Name
        {
            set
            {
                _name = value;
                OnPropertyChanged();
            }
            get => _name;
        }

        private string _defaultState;
        public string DefaultState
        {
            set
            {
                _defaultState = value;
                OnPropertyChanged();
            }
            get => _defaultState;
        }

        public FSMView View { get; protected set; }

        public FSM(MainCanvasContext root)
        {
            Root = root;
            View = new FSMView(this);

            Items = new ObservableCollection<ViewModelBase>();
            States = new ObservableCollection<FSMState>();
            Transitions = new ObservableCollection<FSMTransition>();
        }

        public void AddState(FSMState state)
        {
            Items.Add(state);
            States.Add(state);
            View.canvas.Children.Add(state.View);
        }

        public void RemoveState(FSMState state)
        {
            Items.Remove(state);
            States.Remove(state);
            View.canvas.Children.Remove(state.View);
        }

        public void AddTransition(FSMTransition transition)
        {
            Items.Add(transition);
            Transitions.Add(transition);
            View.canvas.Children.Add(transition.View);
        }

        public void RemoveTransition(FSMTransition transition)
        {
            Items.Remove(transition);
            Transitions.Remove(transition);
            View.canvas.Children.Remove(transition.View);
        }

        public bool NeedPath()
        {
            return FileName == "";
        }

        public void Save(string file)
        {
            if (file != "" && file != FileName) FileName = file;

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
                CheckCharacters = false,
            };

            using (var writer = XmlWriter.Create(FileName, settings))
            {
                writer.WriteStartDocument();

                Write(writer);

                writer.WriteEndDocument();
            }
        }

        public void Clear()
        {
            States.Clear();
            Transitions.Clear();
        }

        public void New()
        {
            Clear();
            FileName = "";
        }

        public void Load(string file)
        {
            Clear();

            FileName = file;

            XmlDocument doc = new XmlDocument();
            doc.Load(FileName);

            Read(doc["fsm"]);
        }

        public void Read(XmlElement node)
        {
            Name = node.GetAttribute("name");
            XmlElement states = node["states"];
            DefaultState = states.GetAttribute("default");

            foreach (XmlElement stateNode in states.ChildNodes)
            {
                FSMState state = new FSMState(this);
                state.Read(stateNode);
                AddState(state);
            }

            XmlElement transitions = node["transitions"];
            foreach (XmlElement transitionNode in transitions.ChildNodes)
            {
                FSMTransition transition = new FSMTransition(this);
                transition.Read(transitionNode);
                AddTransition(transition);
            }
        }

        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("fsm");
            if (!string.IsNullOrWhiteSpace(Name))
            {
                writer.WriteAttributeString("name", Name);
            }

            writer.WriteStartElement("states");
            if (!string.IsNullOrWhiteSpace(DefaultState))
            {
                writer.WriteAttributeString("default", DefaultState);
            }
            foreach (FSMState state in States)
            {
                state.Write(writer);
            }
            writer.WriteEndElement();

            writer.WriteStartElement("transitions");
            foreach (FSMTransition transition in Transitions)
            {
                transition.Write(writer);
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }

    public partial class FSMView : UserControl
    {
        protected FSM m_FSM;

        private readonly MatrixTransform m_Transform = new MatrixTransform();
        private Point? m_MousePosition;

        public FSMView(FSM node)
        {
            InitializeComponent();
            DataContext = m_FSM = node;

            canvas.RenderTransform = m_Transform;
        }

        public FSMView()
        {
            InitializeComponent();
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = FindResource("MainCtxMenu") as ContextMenu;
            cm.IsOpen = true;
        }

        private void MouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = (FrameworkElement)sender;
            element.CaptureMouse();

            m_MousePosition = m_Transform.Inverse.Transform(e.GetPosition(this));
        }

        private void MouseUp(object sender, MouseButtonEventArgs e)
        {
            var element = (FrameworkElement)sender;
            element.ReleaseMouseCapture();

            m_MousePosition = null;

            m_FSM.Root.Dragging = false;
        }

        private void MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double scaleFactor = 1.1;
            if (e.Delta < 0.0)
            {
                scaleFactor = 1.0 / scaleFactor;
            }

            Point mousePostion = e.GetPosition(this);

            Matrix scaleMatrix = m_Transform.Matrix;
            scaleMatrix.ScaleAt(scaleFactor, scaleFactor, mousePostion.X, mousePostion.Y);
            m_Transform.Matrix = scaleMatrix;
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (m_FSM.Root.Dragging)
            {
                var state = m_FSM.Root.Selected as FSMState;
                if (state != null)
                {
                    state.View.MouseMove(sender, e);
                }

                var transition = m_FSM.Root.Selected as FSMTransition;
                if (transition != null)
                {
                    transition.View.MouseMove(sender, e);
                }
            }
            else
            {
                if (m_MousePosition == null)
                {
                    return;
                }

                if (e.LeftButton != MouseButtonState.Pressed)
                {
                    return;
                }

                Point mousePosition = m_Transform.Inverse.Transform(e.GetPosition(this));
                Vector delta = Point.Subtract(mousePosition, m_MousePosition.Value);
                var translate = new TranslateTransform(delta.X, delta.Y);
                m_Transform.Matrix = translate.Value * m_Transform.Matrix;
            }
        }
    }

    public class FSMToTabConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var source = (IEnumerable<FSM>)value;
                if (source != null)
                {
                    var controlTemplate = (ControlTemplate)parameter;

                    var tabItems = new List<TabItem>();

                    foreach (FSM fsm in source)
                    {
                        var tabItem = new TabItem
                        {
                            DataContext = fsm,
                            Header = fsm.Name,
                            Content = fsm.View
                        };

                        tabItems.Add(tabItem);
                    }

                    return tabItems;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack method is not supported");
        }
    }
}
