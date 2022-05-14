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

        public ViewModelBase Selected;
        public bool Dragging;

        public MainCanvasContext()
        {
            FSMs = new ObservableCollection<FSM>();
        }

        public void LoadFSM(string filename)
        {
            FSM fsm = new FSM(this);
            fsm.Load(filename);
            FSMs.Add(fsm);

            OnPropertyChanged("FSMs");
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
        }

        void New(object target, ExecutedRoutedEventArgs e)
        {
            //FSM model = (FSM)DataContext;
            //model.New();
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
            /*
            FSM model = (FSM)DataContext;

            if (model.NeedPath())
            {
                SaveAs(target, e);
                return;
            }

            model.Save("");
            */
        }

        void CanSave(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void SaveAs(object target, ExecutedRoutedEventArgs e)
        {
            /*
            FSM model = (FSM)DataContext;

            SaveFileDialog dialog = new SaveFileDialog();

            dialog.InitialDirectory = "P:\\eai\\Scripts\\FSM";
            dialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            dialog.FilterIndex = 2;
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == true)
            {
                model.Save(dialog.FileName);
            }
            */
        }

        void CanSaveAs(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
