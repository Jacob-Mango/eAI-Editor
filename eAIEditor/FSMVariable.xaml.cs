using System;
using System.Collections.Generic;
using System.Linq;
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
    public class FSMVariable : ViewModelBase
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; OnPropertyChanged(); }
        }

        private string _Type;
        public string Type
        {
            get { return _Type; }
            set { _Type = value; OnPropertyChanged(); }
        }

        private string _Default;

        public string Default
        {
            get { return _Default; }
            set { _Default = value; OnPropertyChanged(); }
        }

        public FSM Root;
        public FSMState State;

        public FSMVariableView View { get; protected set; }

        public FSMVariable(FSM root)
        {
            Root = root;
            View = new FSMVariableView(this);
        }

        public FSMVariable(FSMState root)
        {
            State = root;
            Root = root.Root;
            View = new FSMVariableView(this);
        }

        public void Read(XmlElement node)
        {
            Name = node.GetAttribute("name");
            Type = node.GetAttribute("type");
            Default = node.GetAttribute("default");
        }

        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("variable");

            if (!string.IsNullOrWhiteSpace(Name))
            {
                writer.WriteAttributeString("name", Name);
            }

            if (!string.IsNullOrWhiteSpace(Type))
            {
                writer.WriteAttributeString("type", Type);
            }

            if (!string.IsNullOrWhiteSpace(Default))
            {
                writer.WriteAttributeString("default", Default);
            }

            writer.WriteEndElement();
        }
    }

    public partial class FSMVariableView : UserControl
    {
        protected FSMVariable m_Variable;

        public FSMVariableView(FSMVariable node)
        {
            InitializeComponent();
            DataContext = m_Variable = node;
        }
        private void RemoveVariable(object sender, RoutedEventArgs e)
        {
            if (m_Variable.State != null)
            {
                m_Variable.State.Variables.Remove(m_Variable);
                return;
            }

            m_Variable.Root.Variables.Remove(m_Variable);
        }
    }
}
