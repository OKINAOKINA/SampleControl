using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            new Sample<Grid>(L1, C, R); 
            new Sample<Expander>(L1, C, R);
            
            new Sample<ComboBox>(L2, C, R);
            new Sample<Button>(L2, C, R);
            new Sample<RadioButton>(L2, C, R);
            new Sample<CheckBox>(L2, C, R);

            new Sample<TextBox>(L3, C, R);
            new Sample<TextBlock>(L3, C, R);

            new Sample<Slider>(L4, C, R);
            new Sample<ProgressBar>(L4, C, R);
            new Sample<StatusBar>(L4, C, R);
        }

        public class Sample<T> where T : FrameworkElement, new()
        {
            public T Target { get; set; }
            public TreeViewItem Left { get; set; }

            readonly TreeViewItem _L;
            readonly GroupBox _C;
            readonly PropertyGrid _R;

            public Sample(TreeViewItem l, GroupBox c, PropertyGrid r)
            {
                _L = l;
                _C = c;
                _R = r;

                Target = new T
                {
                    Name = "Sample" + typeof(T).Name
                };
                Left = new TreeViewItem
                {
                    Header = Target.Name
                };
                Left.Selected += SampleSelected;
                _L.Items.Add(Left);
            }

            private void SampleSelected(object sender, RoutedEventArgs e)
            {
                _C.Header = Target.Name;
                _C.Content = Target;

                _R.SelectedObject = Target;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var result = await CSharpScript.EvaluateAsync(CodeEditor.Text);
            var script = CSharpScript.Create(CodeEditor.Text);

            var diagnostics = script.Compile();
            if (diagnostics.Any(x => x.Severity == DiagnosticSeverity.Error))
            {
                ResultTextBox.Text = string.Join(Environment.NewLine, diagnostics);
                return;
            }
            var state = await script.RunAsync(catchException: _ => true);
            var formatter = CSharpObjectFormatter.Instance;
            ResultTextBox.Text = state.Exception == null ? formatter.FormatObject(state.ReturnValue) : formatter.FormatException(state.Exception);
        }
    }
}
