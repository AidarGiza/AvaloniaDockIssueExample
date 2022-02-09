using Avalonia.Controls;
using Dock.Model.ReactiveUI.Controls;
using ReactiveUI;
using System;

namespace DockIssueDemo.ViewModels
{
    public class DockViewModelBase : Document
    {
        public event EventHandler UserControlChanged;
        public event EventHandler<ContentControl> UserControlChange;

        private ContentControl userControl;
        public virtual ContentControl UserControl
        {
            get => userControl;
            set
            {
                if (userControl == null && value == null) return;
                UserControlChange?.Invoke(this, value);
                this.userControl = value;
                UserControlChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
