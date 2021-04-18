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
    /*public class FSMTransition : ViewModelBase
    {
        public FSMTransition(ObservableCollection<FSMState> states, XmlNode node)
        {
            string src = node["from_state"] != null ? node["from_state"].GetAttribute("name") : "";
            string dst = node["to_state"] != null ? node["to_state"].GetAttribute("name") : "";
            Event = node["event"] != null ? node["event"].GetAttribute("name") : "";

            foreach (FSMState state in states)
            {
                if (state.Name == src) Source = state;
                if (state.Name == dst) Destination = state;
            }

            var editor_data = node["editor_data"];
            if (editor_data != null)
            {
                _Src_RelX = double.Parse(editor_data["position_source"].GetAttribute("x"));
                _Src_RelY = double.Parse(editor_data["position_source"].GetAttribute("y"));
                _Dst_RelX = double.Parse(editor_data["position_destination"].GetAttribute("x"));
                _Dst_RelY = double.Parse(editor_data["position_destination"].GetAttribute("y"));

                StateChanged();
            }

            Guard = node["guard"] != null ? node["guard"].InnerText : "";
        }

        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("transition");

            writer.WriteStartElement("editor_data");
            writer.WriteStartElement("position_source");
            writer.WriteAttributeString("x", _Src_RelX.ToString());
            writer.WriteAttributeString("y", _Src_RelY.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("position_destination");
            writer.WriteAttributeString("x", _Dst_RelX.ToString());
            writer.WriteAttributeString("y", _Dst_RelY.ToString());
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("guard");
            writer.WriteString(Guard);
            writer.WriteEndElement();

            writer.WriteStartElement("event");
            writer.WriteAttributeString("name", Event);
            writer.WriteEndElement();
            writer.WriteStartElement("from_state");
            writer.WriteAttributeString("name", Source != null ? Source.Name : "");
            writer.WriteEndElement();
            writer.WriteStartElement("to_state");
            writer.WriteAttributeString("name", Destination != null ? Destination.Name : "");
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        private FSMState _Source;
        public FSMState Source
        {
            get { return _Source; }
            set
            {
                if (_Source != null) _Source.Transitions.Remove(this);
                _Source = value;
                if (_Source != null) _Source.Transitions.Add(this);

                OnPropertyChanged();
            }
        }

        private FSMState _Destination;
        public FSMState Destination
        {
            get { return _Destination; }
            set
            {
                if (_Destination != null) _Destination.Transitions.Remove(this);
                _Destination = value;
                if (_Destination != null) _Destination.Transitions.Add(this);

                OnPropertyChanged();
            }
        }

        private bool _DraggingSource;
        public bool DraggingSource
        {
            get { return _DraggingSource; }
            set { _DraggingSource = value; OnPropertyChanged(); }
        }

        private bool _DraggingDestination;
        public bool DraggingDestination
        {
            get { return _DraggingDestination; }
            set { _DraggingDestination = value; OnPropertyChanged(); }
        }

        private bool _Selected;
        public bool Selected
        {
            get { return _Selected; }
            set { _Selected = value; OnPropertyChanged(); }
        }

        private double _Src_RelX;
        private double _Src_AbsX;
        public double SrcX
        {
            get { return _Src_AbsX; }
            set { _Src_AbsX = value; OnPropertyChanged(); }
        }

        private double _Src_RelY;
        private double _Src_AbsY;

        public double SrcY
        {
            get { return _Src_AbsY; }
            set { _Src_AbsY = value; OnPropertyChanged(); }
        }

        private double _Dst_RelX;
        private double _Dst_AbsX;
        public double DstX
        {
            get { return _Dst_AbsX; }
            set { _Dst_AbsX = value; OnPropertyChanged(); }
        }

        private double _Dst_RelY;
        private double _Dst_AbsY;

        public double DstY
        {
            get { return _Dst_AbsY; }
            set { _Dst_AbsY = value; OnPropertyChanged(); }
        }

        public double X
        {
            get { return 0; }
            set { }
        }

        public double Y
        {
            get { return 0; }
            set { }
        }

        private string _Event;
        public string Event
        {
            get { return _Event; }
            set { _Event = value; OnPropertyChanged(); }
        }

        private string _Guard;
        public string Guard
        {
            get { return _Guard; }
            set { _Guard = value; OnPropertyChanged(); }
        }

        public void UpdateAbsoluteSource()
        {
            SrcX = Source.X + (Source.Width * _Src_RelX);
            SrcY = Source.Y + (Source.Height * _Src_RelY);
        }

        public void UpdateAbsoluteDestination()
        {
            DstX = Destination.X + (Destination.Width * _Dst_RelX);
            DstY = Destination.Y + (Destination.Height * _Dst_RelY);
        }

        public void StateChanged()
        {
            if (!DraggingSource && !DraggingDestination && Source == null && Destination == null) return; // big error

            if (!DraggingSource)
            {
                if (Source != null)
                {
                    UpdateAbsoluteSource();
                }
                else
                {
                    if (_Src_RelX == 0)
                    {
                        SrcX = Destination.X + Destination.Width + (20.0);
                        SrcY = Destination.Y + (Destination.Height * _Dst_RelY);
                    }
                    else if (_Src_RelY == 0)
                    {
                        SrcX = Destination.X + (Destination.Width * _Dst_RelX);
                        SrcY = Destination.Y + Destination.Height + (20.0);
                    }
                    else if (_Src_RelX == 1)
                    {
                        SrcX = Destination.X + (-20.0);
                        SrcY = Destination.Y + (Destination.Height * _Dst_RelY);
                    }
                    else if (_Src_RelY == 1)
                    {
                        SrcX = Destination.X + (Destination.Width * _Dst_RelX);
                        SrcY = Destination.Y + (-20.0);
                    }
                }
            }

            if (!DraggingDestination)
            {
                if (Destination != null)
                {
                    UpdateAbsoluteDestination();
                }
                else
                {
                    if (_Dst_RelX == 0)
                    {
                        DstX = Source.X + Source.Width + (20.0);
                        DstY = Source.Y + (Source.Height * _Src_RelY);
                    }
                    else if (_Dst_RelY == 0)
                    {
                        DstX = Source.X + (Source.Width * _Src_RelX);
                        DstY = Source.Y + Source.Height + (20.0);
                    }
                    else if (_Dst_RelX == 1)
                    {
                        DstX = Source.X + (-20.0);
                        DstY = Source.Y + (Source.Height * _Src_RelY);
                    }
                    else if (_Dst_RelY == 1)
                    {
                        DstX = Source.X + (Source.Width * _Src_RelX);
                        DstY = Source.Y + (-20.0);
                    }
                }
            }
        }
    }*/
}