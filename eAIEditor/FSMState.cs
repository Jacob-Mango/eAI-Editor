using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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

namespace eAIEditor
{
    public class FSMState : ViewModelBase
    {
        public FSMState(XmlNode node)
        {
            Name = node.Attributes["name"].Value;

            var editor_data = node["editor_data"];
            if (editor_data != null)
            {
                X = double.Parse(editor_data["position"].GetAttribute("x"));
                Y = double.Parse(editor_data["position"].GetAttribute("y"));
                Width = double.Parse(editor_data["size"].GetAttribute("width"));
                Height = double.Parse(editor_data["size"].GetAttribute("height"));
            }

            Height = 40;
            Width = 150;

            EventEntry = node["event_entry"] != null ? node["event_entry"].InnerText : "";
            EventExit = node["event_exit"] != null ? node["event_exit"].InnerText : "";
            EventUpdate = node["event_update"] != null ? node["event_update"].InnerText : "";

            XmlElement variables = node["variables"];
            foreach (XmlElement variable in variables)
            {
                Variables.Add(new FSMVariable(variable));
            }
        }

        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("state");
            writer.WriteAttributeString("name", Name);

            writer.WriteStartElement("variables");
            foreach (var variable in Variables) variable.Write(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("editor_data");
            writer.WriteStartElement("position");
            writer.WriteAttributeString("x", X.ToString());
            writer.WriteAttributeString("y", Y.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("size");
            writer.WriteAttributeString("width", Width.ToString());
            writer.WriteAttributeString("height", Height.ToString());
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("event_entry");
            writer.WriteString(EventEntry);
            writer.WriteEndElement();
            writer.WriteStartElement("event_exit");
            writer.WriteString(EventExit);
            writer.WriteEndElement();
            writer.WriteStartElement("event_update");
            writer.WriteString(EventUpdate);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        public ObservableCollection<FSMVariable> Variables { get; } = new ObservableCollection<FSMVariable>();
        public ObservableCollection<FSMTransition> Transitions { get; } = new ObservableCollection<FSMTransition>();

        private bool _Selected;
        public bool Selected
        {
            get { return _Selected; }
            set { _Selected = value; OnPropertyChanged(); }
        }

        private double _X;
        public double X
        {
            get { return _X; }
            set { _X = value; OnPropertyChanged(); foreach (FSMTransition transition in Transitions) transition.StateChanged(); }
        }

        private double _Y;
        public double Y
        {
            get { return _Y; }
            set { _Y = value; OnPropertyChanged(); foreach (FSMTransition transition in Transitions) transition.StateChanged(); }
        }

        private double _Width;
        public double Width
        {
            get { return _Width; }
            set { _Width = value; OnPropertyChanged(); foreach (FSMTransition transition in Transitions) transition.StateChanged(); }
        }

        private double _Height;
        public double Height
        {
            get { return _Height; }
            set { _Height = value; OnPropertyChanged(); foreach (FSMTransition transition in Transitions) transition.StateChanged(); }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; OnPropertyChanged(); }
        }

        private string _SubFSM;
        public string SubFSM
        {
            get { return _SubFSM; }
            set { _SubFSM = value; OnPropertyChanged(); }
        }

        private string _EventEntry;
        public string EventEntry
        {
            get { return _EventEntry; }
            set { _EventEntry = value; OnPropertyChanged(); }
        }

        private string _EventExit;
        public string EventExit
        {
            get { return _EventExit; }
            set { _EventExit = value; OnPropertyChanged(); }
        }

        private string _EventUpdate;
        public string EventUpdate
        {
            get { return _EventUpdate; }
            set { _EventUpdate = value; OnPropertyChanged(); }
        }
    }
}