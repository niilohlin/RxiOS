using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Text;
using Foundation;

namespace UIKit.Reactive.CocoaUnits
{
    class SimpleList<T> : List<T>, IGrouping<Unit, T>
    {
        public Unit Key => Unit.Default;

        public SimpleList(IEnumerable<T> lst):  base(lst)
        {
            
        }
    }

    internal abstract class _RxTableViewSourceBase: UITableViewSource
    {
        internal Subject<NSIndexPath> ItemSelected = new Subject<NSIndexPath>();
        internal IList<IList<nfloat>> RowHeights;

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            ItemSelected.OnNext(indexPath);
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return RowHeights?[indexPath.Section]?[indexPath.Row] ?? 44;
        }
    }
    
    internal abstract class _RxTableViewSource<TKey, TElement> : _RxTableViewSourceBase
    {
        internal IList<IGrouping<TKey, TElement>> Elements;
        internal _RxTableViewSource(IList<IGrouping<TKey, TElement>> elements)
        {
            Elements = elements;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return Elements.Count;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return Elements[(int) section].Count();
        }

    }

    internal class _RxTableViewSimpleSource<TElement>: _RxTableViewSource<Unit,TElement>
    {
        private readonly Func<UITableView, int, TElement, UITableViewCell> _cellFactory;

        public _RxTableViewSimpleSource(IEnumerable<TElement> elements, Func<UITableView, int, TElement, UITableViewCell> cellFactory): 
            base(new List<IGrouping<Unit, TElement>>{new SimpleList<TElement>(elements)})
        {
            
            _cellFactory = cellFactory;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            return _cellFactory(tableView, indexPath.Row, Elements[indexPath.Section].ToList()[indexPath.Row]);
        }
    }

    internal class _RxTableViewIdentifierSource<TElement, TCell>: _RxTableViewSource<Unit, TElement> where TCell: UITableViewCell
    {
        private readonly Action<int, TElement, TCell> _cellInitializer;
        private readonly string _cellIdentifier;

        public _RxTableViewIdentifierSource(string cellIdentifier, IEnumerable<TElement> elements, Action<int, TElement, TCell> cellInitializer):  base(new List<IGrouping<Unit, TElement>>{new SimpleList<TElement>(elements)})
        {
            _cellInitializer = cellInitializer;
            _cellIdentifier = cellIdentifier;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (TCell)tableView.DequeueReusableCell(_cellIdentifier);
            _cellInitializer(indexPath.Row, Elements[indexPath.Section].ToList()[indexPath.Row], cell);
            return cell;
        }
    }
}
