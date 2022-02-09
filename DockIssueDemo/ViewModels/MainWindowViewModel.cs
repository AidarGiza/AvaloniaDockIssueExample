using Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using DockIssueDemo.Utils;
using DockIssueDemo.Views;
using ReactiveUI;

namespace DockIssueDemo.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {


        public MainWindowViewModel()
        {
            AddDockControl<FirstViewModel>(DockPosition.TopRightDock, "First");
            AddDockControl<SecondViewModel>(DockPosition.TopLeftDock, "Second");
            AddDockControl<ThirdViewModel>(DockPosition.TopLeftDock, "Third");
            AddDockControl<FourthViewModel>(DockPosition.BottomDock, "Fourth");
        }

        public IRootDock? Layout
        {
            get => layout;
            set => this.RaiseAndSetIfChanged(ref layout, value, nameof(Layout));
        }
        private IRootDock layout;

        private DockFactory? factory;

        void InitDock()
        {
            factory = new DockFactory();

            Layout = factory.CreateLayout();

            if (Layout is { })
            {
                factory.InitLayout(Layout);
            }

        }

        public void AddDockControl<VM>(DockPosition dockPosition, string title)
        where VM : DockViewModelBase, new()
        {
            if (factory == null) InitDock();
            var dock = factory?.FindDocumentDock(dockPosition);
            if (Layout is { } && dock is { })
            {
                if (dock.VisibleDockables == null) dock.VisibleDockables = factory.CreateList<IDockable>();
                VM tab = new VM()
                {
                    CanClose = true
                };

                tab.Id = tab.GetHashCode().ToString();
                tab.Title = title;
                factory?.InsertDockable(dock, tab, 0);
                factory?.SetActiveDockable(tab);
                factory?.SetFocusedDockable(dock, tab);
            }
        }
    }
}
