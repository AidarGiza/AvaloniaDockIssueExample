using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DockIssueDemo.ViewModels;

namespace DockIssueDemo.Views
{
    public class ThirdView : UserControl
    {
        public ThirdView()
        {
            InitializeComponent();

            DataContextChanged += ThirdView_DataContextChanged;
        }

        private void ThirdView_DataContextChanged(object? sender, System.EventArgs e)
        {
            if (DataContext is ThirdViewModel vm)
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
