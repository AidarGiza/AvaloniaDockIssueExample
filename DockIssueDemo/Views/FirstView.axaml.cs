using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DockIssueDemo.ViewModels;

namespace DockIssueDemo.Views
{
    public class FirstView : UserControl
    {
        public FirstView()
        {
            InitializeComponent();

            DataContextChanged += FirstView_DataContextChanged;
        }

        private void FirstView_DataContextChanged(object? sender, System.EventArgs e)
        {
            if (DataContext is FirstViewModel vm)
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
