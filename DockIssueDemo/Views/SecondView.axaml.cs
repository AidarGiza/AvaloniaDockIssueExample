using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DockIssueDemo.ViewModels;

namespace DockIssueDemo.Views
{
    public class SecondView : UserControl
    {
        public SecondView()
        {
            InitializeComponent();

            DataContextChanged += SecondView_DataContextChanged;
        }

        private void SecondView_DataContextChanged(object? sender, System.EventArgs e)
        {
            if (DataContext is SecondViewModel vm)
            {
                vm.UserControl = this;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
