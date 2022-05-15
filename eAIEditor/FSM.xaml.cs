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
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace eAIEditor
{
    public class FSM : ViewModelBase
    {
        public MainCanvasContext Root;
        public ObservableCollection<FSMVariable> Variables { get; } = new ObservableCollection<FSMVariable>();
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

            States = new ObservableCollection<FSMState>();
            Transitions = new ObservableCollection<FSMTransition>();
        }

        public void AddState(FSMState state)
        {
            States.Add(state);
            View.canvas.Children.Add(state.View);
        }

        public void RemoveState(FSMState state)
        {
            if (state == null)
            {
                return;
            }

            States.Remove(state);
            View.canvas.Children.Remove(state.View);

            var removing = new List<FSMTransition>();
            foreach (var transition in Transitions)
            {
                if (transition.Destination == state)
                {
                    removing.Add(transition);
                }
                else if (transition.Source == state)
                {
                    removing.Add(transition);
                }
            }

            foreach (var transition in removing)
            {
                RemoveTransition(transition);
            }
        }

        public void AddTransition(FSMTransition transition)
        {
            Transitions.Add(transition);
            View.canvas.Children.Add(transition.View);
        }

        public void RemoveTransition(FSMTransition transition)
        {
            Transitions.Remove(transition);
            View.canvas.Children.Remove(transition.View);
        }

        public bool NeedPath()
        {
            return FileName == "";
        }

        public void Save()
        {
            Save(FileName);
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

        public void Load(string file)
        {
            FileName = file;

            XmlDocument doc = new XmlDocument();
            doc.Load(FileName);

            Read(doc["fsm"]);
        }

        public void Read(XmlElement node)
        {
            Name = node.GetAttribute("name");

            XmlElement files = node["files"];
            if (files != null)
            {
                string path = Path.GetDirectoryName(FileName);

                foreach (var file in files.OfType<XmlElement>())
                {
                    string fileName = file.GetAttribute("name");
                    fileName = path + Path.DirectorySeparatorChar + fileName + ".xml";
                    Root.LoadFSM(fileName);
                }
            }

            XmlElement variables = node["variables"];
            if (variables != null)
            {
                foreach (var variable in variables.OfType<XmlElement>())
                {
                    Variables.Add(new FSMVariable(variable));
                }
            }

            XmlElement states = node["states"];
            if (states != null)
            {
                DefaultState = states.GetAttribute("default");

                foreach (var stateNode in states.OfType<XmlElement>())
                {
                    FSMState state = new FSMState(this);
                    state.Read(stateNode);
                    AddState(state);
                }
            }

            XmlElement transitions = node["transitions"];
            if (transitions != null)
            {
                foreach (var transitionNode in transitions.OfType<XmlElement>())
                {
                    FSMTransition transition = new FSMTransition(this);
                    transition.Read(transitionNode);
                    AddTransition(transition);
                }
            }
        }

        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("fsm");
            if (!string.IsNullOrWhiteSpace(Name))
            {
                writer.WriteAttributeString("name", Name);
            }

            writer.WriteStartElement("files");
            foreach (FSMState state in States)
            {
                if (state.SubFSM != null)
                {
                    writer.WriteStartElement("file");
                    writer.WriteAttributeString("name", state.SubFSM.Name);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();

            writer.WriteStartElement("variables");
            foreach (var variable in Variables)
            {
                variable.Write(writer);
            }
            writer.WriteEndElement();

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

        private void AddState(object target, ExecutedRoutedEventArgs e)
        {
            Point point = Mouse.GetPosition(canvas);

            FSMState state = new FSMState(m_FSM);
            state.Name = "State_" + m_FSM.States.Count();
            state.Position = point;
            m_FSM.AddState(state);
        }

        private void CanAddState(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void PasteState(object target, ExecutedRoutedEventArgs e)
        {
            Point point = Mouse.GetPosition(canvas);

            FSMState state = new FSMState(m_FSM);
            state.PasteFromClipboard();

            state.Name = state.Name + "_Copy";
            state.Position = point;

            m_FSM.AddState(state);
        }

        private void CanPasteState(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
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
