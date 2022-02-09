using ReactiveUI;

namespace DockIssueDemo.ViewModels
{
    public class SecondViewModel : DockViewModelBase
    {
        public string Text
        {
            get => text;
            set => this.RaiseAndSetIfChanged(ref text, value, nameof(Text));
        }
        private string text = "Text of Second";
    }
}
