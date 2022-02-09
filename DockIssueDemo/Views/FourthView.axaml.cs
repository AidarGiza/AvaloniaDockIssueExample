using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DockIssueDemo.ViewModels;

namespace DockIssueDemo.Views
{
    public class FourthView : UserControl
    {
        public FourthView()
        {
            InitializeComponent();

            DataContextChanged += FourthView_DataContextChanged;
        }

        private void FourthView_DataContextChanged(object? sender, System.EventArgs e)
        {
            if (DataContext is FourthViewModel vm)
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
