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
                CheckCharacters = true,
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

        public string ReadCodeElement(XmlElement element)
        {
            if (element == null)
            {
                return "";
            }
            string text = element.InnerText;

            string[] lines = text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            text = "";
            for (int i = 1; i < lines.Length - 1; i++)
            {
                var line = lines[i];
                if (line.Length < 4)
                {
                    text += "\n";
                }
                else
                {
                    text += "\n" + line.Remove(0, 4);
                }
            }
            if (text.Length == 0) return "";
            return text.Remove(0, 1);
        }

        public void WriteCodeElement(XmlWriter writer, string node, string text)
        {
            writer.WriteStartElement(node);
            if (!String.IsNullOrWhiteSpace(text))
            {
                string[] lines = text.Split( new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                foreach (var line in lines)
                {
                    writer.WriteString("\n\t\t\t\t" + line);
                }
                writer.WriteString("\n\t\t\t");
            }
            writer.WriteEndElement();
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
                foreach (var variableNode in variables.OfType<XmlElement>())
                {
                    var variable = new FSMVariable(this);
                    variable.Read(variableNode);
                    Variables.Add(variable);
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
        private Point m_MousePositionContextMenu;

        public FSMView(FSM fsm)
        {
            InitializeComponent();
            DataContext = m_FSM = fsm;

            canvas.RenderTransform = m_Transform;
        }

        private void AddState(object target, ExecutedRoutedEventArgs e)
        {
            Point point = Mouse.GetPosition(canvas);

            FSMState state = new FSMState(m_FSM);
            state.Name = "State_" + m_FSM.States.Count();
            state.Position = m_MousePositionContextMenu;
            m_FSM.AddState(state);
        }

        private void CanAddState(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void AddTransition(object target, ExecutedRoutedEventArgs e)
        {
            FSMTransition transition = new FSMTransition(m_FSM);
            transition.Src = m_MousePositionContextMenu;
            transition.Dst = new Point(m_MousePositionContextMenu.X, m_MousePositionContextMenu.Y + 20);
            m_FSM.AddTransition(transition);
        }

        private void CanAddTransition(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Paste(object target, ExecutedRoutedEventArgs e)
        {
            Point point = Mouse.GetPosition(canvas);

            FSMState state = new FSMState(m_FSM);
            state.PasteFromClipboard();

            state.Name = state.Name + "_Copy";
            state.Position = point;

            m_FSM.AddState(state);
        }

        private void CanPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (m_FSM == null)
            {
                return;
            }

            if (e.ChangedButton == MouseButton.Right)
            {
                m_MousePositionContextMenu = Mouse.GetPosition(canvas);
            }

            if (e.OriginalSource == canvas)
            {
                m_FSM.Root.Selected = null;
            }

            var element = (FrameworkElement)sender;
            element.CaptureMouse();

            m_MousePosition = m_Transform.Inverse.Transform(e.GetPosition(this));
        }

        private void MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (m_FSM == null)
            {
                return;
            }

            var element = (FrameworkElement)sender;
            element.ReleaseMouseCapture();

            m_MousePosition = null;

            if (m_FSM.Root.Dragging)
            {
                var transition = m_FSM.Root.Selected as FSMTransition;
                if (transition != null)
                {
                    transition.View.MouseUp(sender, e);
                }
            }

            m_FSM.Root.Dragging = false;
        }

        private void MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (m_FSM == null)
            {
                return;
            }

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
            if (m_FSM == null)
            {
                return;
            }

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
}
