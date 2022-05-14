using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Xml;

namespace eAIEditor
{
    public class FSM
    {
        public ObservableCollection<FSMState> States { get; } = new ObservableCollection<FSMState>();
        public ObservableCollection<FSMTransition> Transitions { get; } = new ObservableCollection<FSMTransition>();

        public ObservableCollection<ViewModelBase> Items { get; } = new ObservableCollection<ViewModelBase>();

        public string File;

        public string FSMName;
        public string DefaultState;

        public FSM()
        {
        }

        public bool NeedPath()
        {
            return File == "";
        }

        public void Save(string file)
        {
            if (file != "" && file != File) File = file;

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
                CheckCharacters = false,
            };

            using (var writer = XmlWriter.Create(File, settings))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("fsm");
                writer.WriteAttributeString("name", FSMName);
                writer.WriteStartElement("states");
                writer.WriteAttributeString("default", DefaultState);
                foreach (FSMState state in States) state.Write(writer);
                writer.WriteEndElement();
                writer.WriteStartElement("transitions");
                foreach (FSMTransition transition in Transitions) transition.Write(writer);
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteEndDocument();
            }
        }

        public void Clear()
        {
            States.Clear();
            Transitions.Clear();
            Items.Clear();
        }

        public void New()
        {
            Clear();
            File = "";
        }

        public void Load(string file)
        {
            Clear();

            File = file;

            XmlDocument doc = new XmlDocument();
            doc.Load(File);

            FSMName = doc["fsm"].GetAttribute("name");
            XmlElement states = doc["fsm"]["states"];
            DefaultState = states.GetAttribute("default");

            foreach (XmlNode node in states.ChildNodes)
            {
                AddState(node);
            }

            XmlElement transitions = doc["fsm"]["transitions"];
            foreach (XmlNode node in transitions.ChildNodes)
            {
                AddTransition(node);
            }
        }

        public void AddState(XmlNode node)
        {
            FSMState state = new FSMState(node);
            Items.Add(state);
            States.Add(state);
        }

        public void AddTransition(XmlNode node)
        {
            FSMTransition transition = new FSMTransition(States, node);
            Items.Add(transition);
            Transitions.Add(transition);
        }
    }
}
