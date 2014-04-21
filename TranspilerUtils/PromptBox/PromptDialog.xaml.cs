using Microsoft.Practices.Prism.Commands;
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
using System.Windows.Shapes;

namespace TranspilerUtils.PromptBox
{
    /// <summary>
    /// Interaction logic for PromptDialog.xaml
    /// </summary>
    public partial class PromptDialog : Window
    {
        public PromptDialog()
        {
            InitializeComponent();
            InputBindings.Add(new KeyBinding(new DelegateCommand(OnCancel), Key.Escape, ModifierKeys.None));
            InputBindings.Add(new KeyBinding(new DelegateCommand(OnOk), Key.Enter, ModifierKeys.None));
            TextValue.Focus();
        }

        private void OnCancel()
        {
            TextValue.Text = null;
            Close();
        }

        private void OnOk()
        {
            Close();
        }

        public static string Show(string caption, string title)
        {
            var dialog = new PromptDialog();
            dialog.Title = title;
            dialog.Caption.Text = caption;

            dialog.ShowDialog();
            return dialog.TextValue.Text;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            OnOk();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnCancel();
        }
    }
}
