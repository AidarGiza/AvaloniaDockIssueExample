using ReactiveUI;

namespace DockIssueDemo.ViewModels
{
    public class FourthViewModel : DockViewModelBase
    {
        public string Info
        {
            get => info;
            set => this.RaiseAndSetIfChanged(ref info, value, nameof(Info));
        }
        private string info = "Info of Fourth";
    }
}
