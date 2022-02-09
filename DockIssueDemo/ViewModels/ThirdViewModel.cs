using ReactiveUI;

namespace DockIssueDemo.ViewModels
{
    public class ThirdViewModel : DockViewModelBase
    {
        public string Description
        {
            get => description;
            set => this.RaiseAndSetIfChanged(ref description, value, nameof(Description));
        }
        private string description = "Description of Third";
    }
}
