﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

namespace eAIEditor
{
    public partial class FSMNodeData : INotifyPropertyChanged
    {
        public Point Poisiton;

        private string _name;
        public string Name {
            set {
                _name = value;
                OnPropertyChanged();
            }
            get => _name;
        }

        // INotifyPropertyChanged implement
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class FSMState : FSMNodeData
    {

    }

    public partial class FSMTransition
    {

    }

    public partial class FSMNode : UserControl
    {
        protected FSMNodeData m_FSMNodeData;

        public FSMNode(FSMNodeData data)
        {
            Debug.WriteLine("Creating Node...");
            InitializeComponent();
            m_FSMNodeData = data;
            DataContext = m_FSMNodeData;
        }

        public void SetPosition(Point position)
        {
            m_FSMNodeData.Poisiton = position;
            Canvas.SetLeft(this, (double)position.X);
            Canvas.SetTop(this, (double)position.Y);
        }
    }
}