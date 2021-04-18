using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.Windows.Shapes;

namespace eAIEditor
{
    public partial class FSMNode
    {
        [NonSerialized]
        public Point Poisiton;

        public string Name;
    }

    public class FSMState: FSMNode
    {

    }

    public class MainCanvasContext: INotifyPropertyChanged
    {


        // INotifyPropertyChanged implement
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    /// <summary>
    /// Interaction logic for MainCanvas.xaml
    /// </summary>
    public partial class MainCanvas : Window
    {
        public MainCanvas()
        {
            InitializeComponent();
        }

        void InsertNode(FSMNode node)
        {

        }

        void RemoveNode(FSMNode node)
        {

        }
    }
}
