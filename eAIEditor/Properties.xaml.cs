using ScintillaNET;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace eAIEditor
{
    public class NullVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TypeMatchesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[1] == null)
            {
                return Visibility.Hidden;
            }

            if (values[0] != values[1])
            {
                return Visibility.Hidden;
            }

            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FSMStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var states = value as ObservableCollection<FSMState>;

            if (states != null)
            {
                List<string> strings = new List<string>();
                foreach (var state in states)
                {
                    strings.Add(state.Name);
                }

                return strings;
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class PropertiesView : UserControl
    {
        public List<string> Keywords_0 = new List<string> {
            "class",
            "extends",
            "switch",
            "auto",
            "while",
            "delete",
            "default",
            "return",
            "private",
            "public",
            "static",
            "native",
            "override",
            "protected",
            "super",
            "this",
            "null",
            "bool",
            "break",
            "case",
            "class",
            "const",
            "continue",
            "default",
            "do",
            "event",
            "else",
            "enum",
            "false",
            "float",
            "for",
            "foreach",
            "if",
            "int",
            "new",
            "null",
            "override",
            "private",
            "protected",
            "public",
            "ref",
            "in",
            "out",
            "inout",
            "return",
            "switch",
            "static",
            "string",
            "this",
            "true",
            "auto",
            "void",
            "while",
            "where"
        };

        public List<string> Keywords_1 = new List<string> {
            ""
        };

        private string _keywords_0 = "";
        private string _keywords_1 = "";

        public PropertiesView()
        {
            InitializeComponent();

            foreach (var keyword in Keywords_0)
            {
                if (_keywords_0.Length != 0)
                    _keywords_0 += " ";
                _keywords_0 += keyword;
            }

            foreach (var keyword in Keywords_1)
            {
                if (_keywords_1.Length != 0)
                    _keywords_1 += " ";
                _keywords_1 += keyword;
            }

            InitScintilla(Control_Transition_Guard);

            InitScintilla(Control_State_EventEntry);
            InitScintilla(Control_State_EventExit);
            InitScintilla(Control_State_EventUpdate);
        }

        System.Drawing.Color IntToColor(int value)
        {
            return System.Drawing.Color.FromArgb(value);
        }

        private const int BACK_COLOR = 0x2A211C;

        private const int FORE_COLOR = 0xB7B7B7;

        private const int NUMBER_MARGIN = 1;

        private const int BOOKMARK_MARGIN = 2;
        private const int BOOKMARK_MARKER = 2;

        private const int FOLDING_MARGIN = 3;

        private const bool CODEFOLDING_CIRCULAR = true;

        void InitScintilla(Scintilla area)
        {
            area.UseTabs = true;
            area.Lexer = Lexer.Cpp;

            //area.Dock = System.Windows.Forms.DockStyle.Fill;

            area.WrapMode = WrapMode.None;
            area.IndentationGuides = IndentView.LookBoth;

            InitColors(area);
            InitSyntaxColoring(area);

            // NUMBER MARGIN
            InitNumberMargin(area);

            // BOOKMARK MARGIN
            InitBookmarkMargin(area);

            // CODE FOLDING MARGIN
            InitCodeFolding(area);
        }

        private void InitColors(Scintilla area)
        {
            area.SetSelectionBackColor(true, IntToColor(0x114D9C));

            area.BorderStyle = System.Windows.Forms.BorderStyle.None;
            //area.CaretLineVisible = true;
            //area.CaretLineBackColor = IntToColor(0x114D9C);
        }

        private void InitNumberMargin(Scintilla area)
        {
            area.Styles[ScintillaNET.Style.LineNumber].BackColor = IntToColor(BACK_COLOR);
            area.Styles[ScintillaNET.Style.LineNumber].ForeColor = IntToColor(FORE_COLOR);
            area.Styles[ScintillaNET.Style.IndentGuide].ForeColor = IntToColor(FORE_COLOR);
            area.Styles[ScintillaNET.Style.IndentGuide].BackColor = IntToColor(BACK_COLOR);

            var nums = area.Margins[NUMBER_MARGIN];
            nums.Width = 30;
            nums.Type = MarginType.Number;
            nums.Sensitive = true;
            nums.Mask = 0;

            //area.MarginClick += TextArea_MarginClick;
        }


        private void InitBookmarkMargin(Scintilla area)
        {

            //TextArea.SetFoldMarginColor(true, IntToColor(BACK_COLOR));

            var margin = area.Margins[BOOKMARK_MARGIN];
            margin.Width = 20;
            margin.Sensitive = true;
            margin.Type = MarginType.Symbol;
            margin.Mask = (1 << BOOKMARK_MARKER);
            //margin.Cursor = MarginCursor.Arrow;

            var marker = area.Markers[BOOKMARK_MARKER];
            marker.Symbol = MarkerSymbol.Circle;
            marker.SetBackColor(IntToColor(0xFF003B));
            marker.SetForeColor(IntToColor(0x000000));
            marker.SetAlpha(100);

        }

        private void InitCodeFolding(Scintilla area)
        {

            area.SetFoldMarginColor(true, IntToColor(BACK_COLOR));
            area.SetFoldMarginHighlightColor(true, IntToColor(BACK_COLOR));

            // Enable code folding
            area.SetProperty("fold", "1");
            area.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            area.Margins[FOLDING_MARGIN].Type = MarginType.Symbol;
            area.Margins[FOLDING_MARGIN].Mask = Marker.MaskFolders;
            area.Margins[FOLDING_MARGIN].Sensitive = true;
            area.Margins[FOLDING_MARGIN].Width = 20;

            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++)
            {
                area.Markers[i].SetForeColor(IntToColor(BACK_COLOR)); // styles for [+] and [-]
                area.Markers[i].SetBackColor(IntToColor(FORE_COLOR)); // styles for [+] and [-]
            }

            // Configure folding markers with respective symbols
            area.Markers[Marker.Folder].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlus : MarkerSymbol.BoxPlus;
            area.Markers[Marker.FolderOpen].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinus : MarkerSymbol.BoxMinus;
            area.Markers[Marker.FolderEnd].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlusConnected : MarkerSymbol.BoxPlusConnected;
            area.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            area.Markers[Marker.FolderOpenMid].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinusConnected : MarkerSymbol.BoxMinusConnected;
            area.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            area.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            area.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);

        }

        void InitSyntaxColoring(Scintilla area)
        {
            area.StyleResetDefault();

            area.Styles[ScintillaNET.Style.Default].BackColor = IntToColor(0x212121);
            area.Styles[ScintillaNET.Style.Default].ForeColor = IntToColor(0xFFFFFF);
            area.StyleClearAll();

            area.CaretForeColor = IntToColor(0xFFFFFF);

            area.Styles[ScintillaNET.Style.Default].Font = "Consolas";
            area.Styles[ScintillaNET.Style.Default].Size = 10;

            area.Styles[ScintillaNET.Style.Cpp.Identifier].ForeColor = IntToColor(0xD0DAE2);
            area.Styles[ScintillaNET.Style.Cpp.Comment].ForeColor = IntToColor(0xBD758B);
            area.Styles[ScintillaNET.Style.Cpp.CommentLine].ForeColor = IntToColor(0x40BF57);
            area.Styles[ScintillaNET.Style.Cpp.CommentDoc].ForeColor = IntToColor(0x2FAE35);
            area.Styles[ScintillaNET.Style.Cpp.Number].ForeColor = IntToColor(0xFFFF00);
            area.Styles[ScintillaNET.Style.Cpp.String].ForeColor = IntToColor(0xFFFF00);
            area.Styles[ScintillaNET.Style.Cpp.Character].ForeColor = IntToColor(0xE95454);
            area.Styles[ScintillaNET.Style.Cpp.Preprocessor].ForeColor = IntToColor(0x8AAFEE);
            area.Styles[ScintillaNET.Style.Cpp.Operator].ForeColor = IntToColor(0xE0E0E0);
            area.Styles[ScintillaNET.Style.Cpp.Regex].ForeColor = IntToColor(0xff00ff);
            area.Styles[ScintillaNET.Style.Cpp.CommentLineDoc].ForeColor = IntToColor(0x77A7DB);
            area.Styles[ScintillaNET.Style.Cpp.Word].ForeColor = IntToColor(0x48A8EE);
            area.Styles[ScintillaNET.Style.Cpp.Word2].ForeColor = IntToColor(0xF98906);
            area.Styles[ScintillaNET.Style.Cpp.CommentDocKeyword].ForeColor = IntToColor(0xB3D991);
            area.Styles[ScintillaNET.Style.Cpp.CommentDocKeywordError].ForeColor = IntToColor(0xFF0000);
            area.Styles[ScintillaNET.Style.Cpp.GlobalClass].ForeColor = IntToColor(0x48A8EE);

            area.SetKeywords(0, _keywords_0);
            area.SetKeywords(1, _keywords_1);
        }
    }

    public static class WindowsFormsHostMap
    {
        public static readonly DependencyProperty TextProperty
            = DependencyProperty.RegisterAttached("Text", typeof(string), typeof(WindowsFormsHostMap), new PropertyMetadata(propertyChanged));
        public static string GetText(WindowsFormsHost o)
        {
            return (string)o.GetValue(TextProperty);
        }
        public static void SetText(WindowsFormsHost o, string value)
        {
            o.SetValue(TextProperty, value);
        }
        static void propertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var t = (sender as WindowsFormsHost).Child as Scintilla;
            if (t != null) t.Text = Convert.ToString(e.NewValue);
        }
    }
}
