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
        //public static Func<Func<UITableView, int, TElement, UITableViewCell>, IDisposable> Items<TParent, TElement>(
            //this Reactive<TParent> rx, IEnumerable<IObservable<TElement>> source) where TParent: UITableView
        //{
            //return cellFactory =>
            //{
                //var dataSource = new RxTableViewReactiveArrayDataSourceSequenceWrapper<IEnumerable<TElement>>(cellFactory);
                //return 
            //};
        //}

        public static Func<Func<UITableView, int, TElement, UITableViewCell>, IDisposable> Items<TParent, TElement>(
            this Reactive<TParent> rx, IObservable<IEnumerable<TElement>> source) where TParent: UITableView
        {
            return cellFactory =>
            {
                var tableViewSource = new RxTableViewSource<TElement>(source, cellFactory, rx.Parent);
                rx.Parent.Source = tableViewSource;
                return Disposable.Create(() => tableViewSource.Dispose());
            };
        }

        public static IObserver<IEnumerable<TElement>> Items<TParent, TElement>(this Reactive<TParent> rx,
            Func<UITableView, int, TElement, UITableViewCell> cellFactory) where TParent: UITableView
        {
            return Observer.Create<IEnumerable<TElement>>(nextElements =>
            {
                rx.Parent.Source = new _RxTableViewSource<TElement>(nextElements, cellFactory);
            });
        }
    }

    internal class _RxTableViewSource<TElement>: UITableViewSource
    {
        private Func<UITableView, int, TElement, UITableViewCell> _cellFactory;
        private IEnumerable<TElement> _elements;

        public _RxTableViewSource(IEnumerable<TElement> elements, Func<UITableView, int, TElement, UITableViewCell> cellFactory)
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
    internal class RxTableViewSource<TElement>: UITableViewSource
    {
        private IEnumerable<TElement> _source;
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private readonly Func<UITableView, int, TElement, UITableViewCell> _cellFactory;

        public RxTableViewSource(IObservable<IEnumerable<TElement>> source, Func<UITableView, int, TElement, UITableViewCell> cellFactory, UITableView tableView)
        {
            source.Subscribe(newSource =>
            {
                _source = newSource;
                tableView.ReloadData();
            }).DisposedBy(_compositeDisposable);
            _cellFactory = cellFactory;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            return _cellFactory(tableView, indexPath.Row, _source.ToArray()[indexPath.Row]);
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _source.Count();
        }
    }
}