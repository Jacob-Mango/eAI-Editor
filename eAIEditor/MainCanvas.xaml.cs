using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace eAIEditor
{
    public class MainCanvasContext : INotifyPropertyChanged
    {
        public ObservableCollection<FSM> FSMs { get; }

        private int _selectedFSMIndex;
        public int SelectedFSMIndex
        {
            set
            {
                _selectedFSMIndex = value;
                OnPropertyChanged();
            }
            get => _selectedFSMIndex;
        }

        public FSM FocusedFSM {
            set
            {
                SelectedFSMIndex = FSMs.IndexOf(value);
            }
            get
            {
                if (SelectedFSMIndex < 0 || SelectedFSMIndex >= FSMs.Count())
                {
                    return null;
                }

                return FSMs[SelectedFSMIndex];
            } 
        }

        public ViewModelBase Selected;
        public bool Dragging;

        public MainCanvasContext()
        {
            FSMs = new ObservableCollection<FSM>();
        }

        public FSM Get(string name)
        {
            foreach (var fsm in FSMs)
            {
                if (fsm.Name.Equals(name))
                {
                    return fsm;
                }
            }

            return null;
        }

        public void LoadFSM(string filename)
        {
            FSM fsm = new FSM(this);
            fsm.Load(filename);
            FSMs.Add(fsm);

            FocusedFSM = fsm;

            OnPropertyChanged("FSMs");
        }

        public void NewFSM(string filename)
        {
            FSM fsm = new FSM(this);
            fsm.Save(filename);
            FSMs.Add(fsm);

            FocusedFSM = fsm;

            OnPropertyChanged("FSMs");
        }

        public void SaveFSM()
        {
            FocusedFSM.Save();
        }

        public void SaveAll()
        {
            foreach (var fsm in FSMs)
            {
                fsm.Save();
            }
        }

        // INotifyPropertyChanged implement
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Interaction logic for MainCanvas.xaml
    /// </summary>
    public partial class MainCanvas : Window
    {
        protected MainCanvasContext m_Context;

        public MainCanvas()
        {
            InitializeComponent();

            m_Context = DataContext as MainCanvasContext;

#if DEBUG
            //m_Context.LoadFSM("P:\\DayZExpansion\\AI\\Scripts\\FSM\\Test.xml");
            m_Context.LoadFSM("P:\\DayZExpansion\\AI\\Scripts\\FSM\\Master.xml");
#endif
        }

        void New(object target, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.InitialDirectory = "P:\\DayZExpansion\\AI\\Scripts\\FSM";
            dialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            dialog.FilterIndex = 2;
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == true)
            {
                m_Context.NewFSM(dialog.FileName);
            }
        }

        void CanNew(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void Open(object target, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.InitialDirectory = "P:\\DayZExpansion\\AI\\Scripts\\FSM";
            dialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            dialog.FilterIndex = 2;
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == true)
            {
                m_Context.LoadFSM(dialog.FileName);
            }
        }

        void CanOpen(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void Save(object target, ExecutedRoutedEventArgs e)
        {
            m_Context.SaveFSM();
        }

        void CanSave(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = m_Context.FocusedFSM != null;
        }

        void SaveAll(object target, ExecutedRoutedEventArgs e)
        {
            m_Context.SaveAll();
        }

        void CanSaveAll(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
