using ScintillaNET;

using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Interaction logic for Properties.xaml
    /// </summary>
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
            "exception",
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

        void InitScintilla(Scintilla area)
        {
            area.UseTabs = true;
            area.Lexer = Lexer.Cpp;

            InitSyntaxColoring(area);
        }

        void InitSyntaxColoring(Scintilla area)
        {
            area.StyleResetDefault();

            area.Styles[ScintillaNET.Style.Default].Font = "Consolas";
            area.Styles[ScintillaNET.Style.Default].Size = 10;
            area.Styles[ScintillaNET.Style.Default].BackColor = IntToColor(0x212121);
            area.Styles[ScintillaNET.Style.Default].ForeColor = IntToColor(0xFFFFFF);
            area.StyleClearAll();

            area.CaretForeColor = IntToColor(0xFFFFFF);

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
