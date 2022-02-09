using Dock.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.ReactiveUI;
using Dock.Model.ReactiveUI.Controls;
using Dock.Model.ReactiveUI.Core;
using DockIssueDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DockIssueDemo.Utils
{
    public class DockFactory : Factory
    {
        private IRootDock? rootDock;
        private ProportionalDock mainLayout;

        public override IDocumentDock CreateDocumentDock() => new DocumentDock();

        public DockFactory()
        {
            this.DockableClosed += DockFactory_DockableClosed;
        }

        private void DockFactory_DockableClosed(object sender, Dock.Model.Core.Events.DockableClosedEventArgs e)
        {
            var parent = e.Dockable.Owner as DocumentDock;
            if (parent.VisibleDockables.Count == 0) DeleteDockable(parent);
            //InitLayout(rootDock);
        }

        void DeleteDockable(IDockable dockable)
        {
            if (dockable != mainLayout)
            {
                var parent = dockable.Owner as DockBase;
                if (parent != null)
                {
                    parent.VisibleDockables.Remove(dockable);
                    if (dockable is DockViewModelBase) ContextLocator.Remove(dockable.Id);
                    if (dockable is DockBase) DockableLocator.Remove(dockable.Id);
                    if (parent.VisibleDockables.Count == 0) DeleteDockable(parent);
                }
            }
        }

        IDocumentDock GetExistedDocumentDock(IDockable dockable, string id)
        {
            if (dockable.Id == id && dockable is IDocumentDock docDock) return docDock;
            if (dockable is DockBase dbase)
            {
                if (dbase.VisibleDockables != null && dbase.VisibleDockables.Count > 0)
                {
                    foreach (var vdockable in dbase.VisibleDockables)
                    {
                        var res = GetExistedDocumentDock(vdockable, id);
                        if (res != null) return res;
                    }
                    return null;
                }
                else return null;
            }
            else return null;
        }

        IProportionalDock GetExistedProportionalDock(IDockable dockable, string id)
        {
            if (dockable.Id == id && dockable is IProportionalDock propDock) return propDock;
            if (dockable is DockBase dbase)
            {
                if (dbase.VisibleDockables != null && dbase.VisibleDockables.Count > 0)
                {
                    foreach (var vdockable in dbase.VisibleDockables)
                    {
                        var res = GetExistedProportionalDock(vdockable, id);
                        if (res != null) return res;
                    }
                    return null;
                }
                else return null;
            }
            else return null;
        }

        public IDocumentDock FindDocumentDock(DockPosition pos)
        {
            if (rootDock == null) return null;


            // Если нет главного - создать
            if (mainLayout == null) CreateMainDock();

            // Если ищется нижняя панель
            if (pos == DockPosition.BottomDock)
            {
                // Найти нижнюю панель
                var bottom = GetExistedDocumentDock(mainLayout, "BottomDock");
                // Вернуть его если найден
                if (bottom != null)
                {
                    return bottom;
                }

                // Если нет, ищем подходящий
                // Если у главного много панелей
                if (mainLayout.VisibleDockables.Count > 1)
                {
                    // Если нет, ищем последнюю
                    var last = mainLayout.VisibleDockables.Last();
                    if (last is DocumentDock)
                    {
                        return last as IDocumentDock;
                    }
                    else // Если последняя - не панель, создаем новую после нее
                    {
                        mainLayout.VisibleDockables.Add(new ProportionalDockSplitter());
                        var bottomDock = CreateDocumentDock();
                        bottomDock.Factory = this;
                        bottomDock.Id = "BottomDock";
                        bottomDock.Proportion = 0.5;
                        bottomDock.IsCollapsable = true;
                        bottomDock.VisibleDockables = new List<IDockable>();

                        mainLayout.VisibleDockables.Add(bottomDock);

                        InitLayout(rootDock);
                        return bottomDock;
                    }
                }
                else
                {
                    if (mainLayout.VisibleDockables.Count == 1) mainLayout.VisibleDockables.Add(new ProportionalDockSplitter());

                    var bottomDock = CreateDocumentDock();
                    bottomDock.Factory = this;
                    bottomDock.Id = "BottomDock";
                    bottomDock.Proportion = 0.5;
                    bottomDock.IsCollapsable = true;
                    bottomDock.VisibleDockables = new List<IDockable>();

                    mainLayout.VisibleDockables.Add(bottomDock);

                    InitLayout(rootDock);
                    return bottomDock;
                }
            }

            // Если ищется верхняя левая панель
            if (pos == DockPosition.TopLeftDock)
            {
                // Найти верхнюю левую панель
                var topLeft = GetExistedDocumentDock(mainLayout, "TopLeftDock");
                // Вернуть его если найден
                if (topLeft != null)
                {
                    return topLeft;
                }

                // Если в главной панели ничего нет - создать
                if (mainLayout.VisibleDockables.Count == 0)
                {

                    topLeft = CreateDocumentDock();
                    topLeft.Factory = this;
                    topLeft.Id = "TopLeftDock";
                    topLeft.Proportion = 0.5;
                    topLeft.IsCollapsable = true;
                    topLeft.VisibleDockables = new List<IDockable>();

                    var top = CreateProportionalDock();
                    top.Factory = this;
                    top.Id = "TopDock";
                    top.Proportion = 0.5;
                    top.Orientation = Orientation.Horizontal;
                    top.VisibleDockables = CreateList<IDockable>(topLeft);

                    mainLayout.VisibleDockables.Add(top);

                    InitLayout(rootDock);
                    return topLeft;
                }

                if (mainLayout.VisibleDockables.Count > 0)
                {
                    // Ищем вернюю панель
                    var top = GetExistedProportionalDock(mainLayout, "TopDock");
                    // Если нет, берем за верхнюю первый горизониальный ProportionalDock из главной панели
                    if (top == null) top = mainLayout.VisibleDockables.FirstOrDefault(vd => vd is ProportionalDock pd && pd.Orientation == Orientation.Horizontal) as ProportionalDock;

                    // Если подходящая нашлась
                    if (top != null)
                    {
                        // Берем за левую верхнюю первый DocumentDock
                        topLeft = top.VisibleDockables.FirstOrDefault(vb => vb is DocumentDock && vb.Id != "TopRightDock") as DocumentDock;
                        // Если есть такой - возвращаем
                        if (topLeft != null)
                        {
                            return topLeft;
                        }
                        else // Если нет - создаем там
                        {
                            topLeft = CreateDocumentDock();
                            topLeft.Factory = this;
                            topLeft.Id = "TopLeftDock";
                            topLeft.Proportion = 0.5;
                            topLeft.IsCollapsable = true;
                            topLeft.VisibleDockables = new List<IDockable>();

                            top.VisibleDockables.Insert(0, topLeft);
                            if (top.VisibleDockables.Count > 1) top.VisibleDockables.Insert(1, new ProportionalDockSplitter());

                            InitLayout(rootDock);
                            return topLeft;
                        }
                    }
                    else // Если подходящая не нашлась
                    {
                        topLeft = CreateDocumentDock();
                        topLeft.Factory = this;
                        topLeft.Id = "TopLeftDock";
                        topLeft.Proportion = 0.5;
                        topLeft.IsCollapsable = true;
                        topLeft.VisibleDockables = new List<IDockable>();

                        //DockableLocator.Add("TopLeftDock", () => topLeft);

                        top = CreateProportionalDock();
                        top.Factory = this;
                        top.Id = "TopDock";
                        top.Proportion = 0.5;
                        top.Orientation = Orientation.Horizontal;
                        top.VisibleDockables = CreateList<IDockable>(topLeft);


                        mainLayout.VisibleDockables.Insert(0, top);
                        if (mainLayout.VisibleDockables.Count > 1) mainLayout.VisibleDockables.Insert(1, new ProportionalDockSplitter());

                        InitLayout(rootDock);
                        return topLeft;
                    }
                }
            }

            // Если ищется верхняя правая панель
            if (pos == DockPosition.TopRightDock)
            {
                // Найти верхнюю правую панель
                var topRight = GetExistedDocumentDock(mainLayout, "TopRightDock");
                // Вернуть его если найден
                if (topRight != null)
                {
                    return topRight;
                }
                // Если в главной панели ничего нет - создать
                if (mainLayout.VisibleDockables.Count == 0)
                {
                    topRight = CreateDocumentDock();
                    topRight.Factory = this;
                    topRight.Id = "TopRightDock";
                    topRight.Proportion = 0.5;
                    topRight.IsCollapsable = true;
                    topRight.VisibleDockables = new List<IDockable>();

                    var top = CreateProportionalDock();
                    top.Factory = this;
                    top.Id = "TopDock";
                    top.Proportion = 0.5;
                    top.Orientation = Orientation.Horizontal;
                    top.VisibleDockables = CreateList<IDockable>(topRight);

                    mainLayout.VisibleDockables.Add(top);

                    InitLayout(rootDock);
                    return topRight;
                }

                if (mainLayout.VisibleDockables.Count > 0)
                {
                    // Ищем вернюю панель
                    var top = GetExistedProportionalDock(mainLayout, "TopDock");
                    // Если нет, берем за верхнюю первый горизониальный ProportionalDock из главной панели
                    if (top == null) top = mainLayout.VisibleDockables.FirstOrDefault(vd => vd is ProportionalDock pd && pd.Orientation == Orientation.Horizontal) as ProportionalDock;

                    // Если подходящая нашлась
                    if (top != null)
                    {
                        // Если в верхней панели больше одной панели ищем последнюю
                        if (top.VisibleDockables.Count > 1)
                        {
                            // Берем за левую верхнюю последний DocumentDock
                            topRight = top.VisibleDockables.LastOrDefault(vb => vb is DocumentDock && vb.Id != "TopLeftDock") as DocumentDock;
                            // Если есть такой - возвращаем
                            if (topRight != null)
                            {
                                return topRight;
                            }
                        }

                        // Если нет - создаем там

                        topRight = CreateDocumentDock();
                        topRight.Factory = this;
                        topRight.Id = "TopRightDock";
                        topRight.Proportion = 0.5;
                        topRight.IsCollapsable = true;
                        topRight.VisibleDockables = new List<IDockable>();

                        if (top.VisibleDockables.Count == 1) top.VisibleDockables.Add(new ProportionalDockSplitter());
                        top.VisibleDockables.Add(topRight);

                        InitLayout(rootDock);
                        return topRight;
                    }
                    else
                    {

                        topRight = CreateDocumentDock();
                        topRight.Factory = this;
                        topRight.Id = "TopRightDock";
                        topRight.Proportion = 0.5;
                        topRight.IsCollapsable = true;
                        topRight.VisibleDockables = new List<IDockable>();

                        top = CreateProportionalDock();
                        top.Factory = this;
                        top.Id = "TopDock";
                        top.Proportion = 0.5;
                        top.Orientation = Orientation.Horizontal;
                        top.VisibleDockables = CreateList<IDockable>(topRight);

                        mainLayout.VisibleDockables.Insert(0, top);
                        if (mainLayout.VisibleDockables.Count > 1) mainLayout.VisibleDockables.Insert(1, new ProportionalDockSplitter());

                        InitLayout(rootDock);
                        return topRight;
                    }
                }
            }

            return null;
        }

        //IDocumentDock NewDocumentDock(string id, double proportion, bool collapsable)
        //{
        //    var d = CreateDocumentDock();
        //    d.Factory = this;
        //    d.Id = id;
        //    d.Proportion = proportion;
        //    d.IsCollapsable = collapsable;
        //    d.VisibleDockables = new List<IDockable>();
        //    return d;
        //}

        //IProportionalDock NewProportionalDock(string id, double proportion, bool collapsable, Orientation orientation)
        //{
        //    var p = CreateProportionalDock();
        //    p.Factory = this;
        //    p.Id = id;
        //    p.Proportion = proportion;
        //    p.IsCollapsable = collapsable;
        //    p.Orientation = orientation;
        //    p.VisibleDockables = new List<IDockable>();
        //    return p;
        //}

        //public override void MoveDockable(IDock sourceDock, IDock targetDock, IDockable sourceDockable, IDockable targetDockable)
        //{
        //    base.MoveDockable(sourceDock, targetDock, sourceDockable, targetDockable);

        //    InitLayout(rootDock);
        //}

        public void CreateMainDock()
        {
            if (rootDock.VisibleDockables == null) rootDock.VisibleDockables = CreateList<IDockable>();
            mainLayout = new ProportionalDock
            {
                Orientation = Orientation.Vertical,
                VisibleDockables = CreateList<IDockable>()
            };
            rootDock.VisibleDockables.Add(mainLayout);
        }

        public override IRootDock CreateLayout()
        {
            mainLayout = new ProportionalDock
            {
                Orientation = Orientation.Vertical,
                VisibleDockables = CreateList<IDockable>()
            };

            rootDock = CreateRootDock();

            rootDock.IsCollapsable = false;
            rootDock.ActiveDockable = mainLayout;
            rootDock.DefaultDockable = mainLayout;
            rootDock.VisibleDockables = CreateList<IDockable>(mainLayout);

            return rootDock;
        }

        List<IDockable> GetAllDockables(IDockable dock)
        {
            var list = new List<IDockable>();
            if (dock is DockBase baseDock)
            {
                if (baseDock.VisibleDockables != null && baseDock.VisibleDockables.Count > 0)
                {
                    foreach (var vd in baseDock.VisibleDockables)
                    {
                        list.AddRange(GetAllDockables(vd));
                    }
                }
                if (dock is DocumentDock)
                {
                    list.Add(dock);
                }
            }
            return list;
        }

        public override void InitLayout(IDockable layout)
        {
            if (ContextLocator == null)
            {
                ContextLocator = new Dictionary<string, Func<object>>();
            }

            DockableLocator = new Dictionary<string, Func<IDockable?>>()
            {
                ["Root"] = () => rootDock
            };

            if (HostWindowLocator == null)
            {
                HostWindowLocator = new Dictionary<string, Func<IHostWindow>>
                {
                    [nameof(IDockWindow)] = () => new HostWindow()
                };
            }

            base.InitLayout(layout);
        }
    }

    public enum DockPosition
    {
        BottomDock,
        TopLeftDock,
        TopRightDock
    }
}
