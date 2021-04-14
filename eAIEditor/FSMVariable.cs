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
    public class FSMVariable : ViewModelBase
    {
        public FSMVariable(XmlElement element)
        {
            Name = element.GetAttribute("name");
            Type = element.GetAttribute("type");
            Default = element.GetAttribute("default");
        }

        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("variable");
            writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("type", Type);
            if (Default != "") writer.WriteAttributeString("default", Default);
            writer.WriteEndElement();
        }

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
    }
}