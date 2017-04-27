using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Foundation;
using UIKit.Reactive.CocoaUnits;
using UIKit.Reactive.DataSources;


namespace UIKit.Reactive
{
    public static class RxUITableView
    {
        public static IObserver<IEnumerable<TElement>> Items<TParent, TElement>(this Reactive<TParent> rx,
            Func<UITableView, int, TElement, UITableViewCell> cellFactory) where TParent: UITableView
        {
            Debug.Assert(rx.Parent.Source == null, "Source is already set");
            var source = new _RxTableViewSimpleSource<TElement>(new List<TElement>(), cellFactory);
            return Observer.Create<IEnumerable<TElement>>(nextElements =>
            {
                source.Elements = new List<IGrouping<Unit, TElement>>{new SimpleList<TElement>(nextElements)};
                if (rx.Parent.Source == null)
                {
                    rx.Parent.Source = source;
                }
                rx.Parent.ReloadData();
            });
        }

        public static IObserver<IEnumerable<TElement>> Items<TParent, TElement, TCell>(this Reactive<TParent> rx, string cellIdentifier,
            Action<int, TElement, TCell> cellInitializer) where TParent: UITableView where TCell: UITableViewCell
        {
            Debug.Assert(rx.Parent.Source == null, "Source is already set");
            _RxTableViewIdentifierSource<TElement, TCell> source = new _RxTableViewIdentifierSource<TElement, TCell>(cellIdentifier, new List<TElement>(), cellInitializer);
            return Observer.Create<IEnumerable<TElement>>(nextElements =>
            {
                source.Elements = new List<IGrouping<Unit, TElement>>{new SimpleList<TElement>(nextElements)};
                if (rx.Parent.Source == null)
                {
                    rx.Parent.Source = source;
                }
                rx.Parent.ReloadData();
            });
        }


        public static IObserver<IEnumerable<string>> SectionTitles<TParent>(this Reactive<TParent> rx) where TParent: UITableView
        {
            return Observer.Create<IEnumerable<string>>(strings =>
            {
                throw new NotImplementedException("");
            });
        }

        public static IObservable<NSIndexPath> ItemSelected<TParent>(this Reactive<TParent> rx)
            where TParent : UITableView
        {
            Debug.Assert(rx.Parent.Source != null, "Source is not set");
            return ((_RxTableViewSourceBase) rx.Parent.Source).ItemSelected;
        }

        public static IObservable<TModel> ModelSelected<TParent, TModel>(this Reactive<TParent> rx)
            where TParent : UITableView
        {
            var source = (_RxTableViewSimpleSource<TModel>) rx.Parent.Source;
            return source.ItemSelected.Select(indexPath => source.Elements[indexPath.Section].ToList()[indexPath.Row]);
        }

        public static IObserver<IEnumerable<nfloat>> RowHeights<TParent>(this Reactive<TParent> rx)
            where TParent : UITableView
        {
            Debug.Assert(rx.Parent.Source != null, "Source is not set");
            return Observer.Create<IEnumerable<nfloat>>(heights =>
            {
                ((_RxTableViewSourceBase) rx.Parent.Source).RowHeights = new List<IList<nfloat>>{heights.ToList()};
                rx.Parent.ReloadData();// TODO only reload necessary rows
            });
        }
    }
}