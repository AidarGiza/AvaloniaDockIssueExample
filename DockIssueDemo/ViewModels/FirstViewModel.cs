using ReactiveUI;

namespace DockIssueDemo.ViewModels
{
    public class FirstViewModel : DockViewModelBase
    {
        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value, nameof(Name));
        }
        private string name = "Name of First";

        
    }
}
