using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Foundation;
using UIKit.Reactive.DataSources;


namespace UIKit.Reactive
{
    public static class RxUITableView
    {
        public static IObserver<IEnumerable<TElement>> Items<TParent, TElement>(this Reactive<TParent> rx,
            Func<UITableView, int, TElement, UITableViewCell> cellFactory) where TParent: UITableView
        {
            return Observer.Create<IEnumerable<TElement>>(nextElements =>
            {
                rx.Parent.Source = new _RxTableViewSimpleSource<TElement>(nextElements, cellFactory);
            });
        }

        public static IObserver<IEnumerable<TElement>> Items<TParent, TElement, TCell>(this Reactive<TParent> rx, string cellIdentifier,
            Action<int, TElement, TCell> cellInitializer) where TParent: UITableView where TCell: UITableViewCell
        {
            return Observer.Create<IEnumerable<TElement>>(nextElements =>
            {
                rx.Parent.Source = new _RxTableViewIdentifierSource<TElement, TCell>(cellIdentifier, nextElements, cellInitializer);
            });
        }

        //public static IObserver<IEnumerable<TSection>> Sections<TParent, TSection, TElement>(
        //    this Reactive<TParent> rx,
        //    Func<UITableView, int, TSection, string> getHeader,
        //    Func<UITableView, int, IEnumerable<TElement>> getItems,
        //    Func<UITableView, int, TElement, UITableViewCell> cellFactory
        //) where TParent: UITableView
        //{
        //    return Observer.Create<IEnumerable<TSection>>(nextSections =>
        //    {
        //        rx.Parent.Source = new _RxTableViewSimpleSectionedSource<TSection, TElement>(nextSections, getHeader, getItems, cellFactory);
        //    });
            
        //}

    }

    internal class _RxTableViewSimpleSource<TElement>: UITableViewSource
    {
        private readonly Func<UITableView, int, TElement, UITableViewCell> _cellFactory;
        private readonly IEnumerable<TElement> _elements;

        public _RxTableViewSimpleSource(IEnumerable<TElement> elements, Func<UITableView, int, TElement, UITableViewCell> cellFactory)
        {
            _cellFactory = cellFactory;
            _elements = elements;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            return _cellFactory(tableView, indexPath.Row, _elements.ToArray()[indexPath.Row]);
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _elements.Count();
        }
    }

    internal class _RxTableViewIdentifierSource<TElement, TCell>: UITableViewSource where TCell: UITableViewCell
    {
        private readonly Action<int, TElement, TCell> _cellInitializer;
        private readonly IEnumerable<TElement> _elements;
        private readonly string _cellIdentifier;

        public _RxTableViewIdentifierSource(string cellIdentifier, IEnumerable<TElement> elements, Action<int, TElement, TCell> cellInitializer)
        {
            _cellInitializer = cellInitializer;
            _elements = elements;
            _cellIdentifier = cellIdentifier;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (TCell)tableView.DequeueReusableCell(_cellIdentifier);
            _cellInitializer(indexPath.Row, _elements.ToArray()[indexPath.Row], cell);
            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _elements.Count();
        }
    }

    //internal class _RxTableViewSimpleSectionedSource<TSection, TElement> : UITableViewSource
    //{
    //    private IEnumerable<TSection> _sections;
    //    private Func<UITableView, int, TSection, string> _getHeader;
    //    private Func<UITableView, int,  IEnumerable<TElement>> _getItems;
    //    private Func<UITableView, int, TElement, UITableViewCell> _cellFactory;

    //    public _RxTableViewSimpleSectionedSource(IEnumerable<TSection> sections, Func<UITableView, int, TSection, string> getHeader, Func<UITableView, int, IEnumerable<TElement>> getItems, Func<UITableView, int, TElement, UITableViewCell> cellFactory)
    //    {
    //        _sections = sections;
    //        _getHeader = getHeader;
    //        _getItems = getItems;
    //        _cellFactory = cellFactory;
    //    }

    //    public override nint NumberOfSections(UITableView tableView)
    //    {
    //        return _sections.Count();
    //    }

    //    public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
    //    {

    //        return _cellFactory(tableView, indexPath.Row, _elements.ToArray()[indexPath.Row]);
    //    }

    //    public override nint RowsInSection(UITableView tableview, nint section)
    //    {

    //        var items = _getItems(tableview, section);
    //        return items.ToArray().Length;
    //    }
    //}
}