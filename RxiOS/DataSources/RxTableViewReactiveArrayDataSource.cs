
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Foundation;
using UIKit.Reactive.CocoaUnits;
using UIKit.Reactive.Common;

namespace UIKit.Reactive.DataSources
{
    public abstract class _RxTableViewReactiveArrayDataSource: UITableViewSource
    {
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            return _GetCell(tableView, indexPath);
        }

        internal abstract UITableViewCell _GetCell(UITableView tableView, NSIndexPath indexpath);

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _RowsInSection(tableview, section);
        }

        public abstract nint _RowsInSection(UITableView tableview, nint section);
    }

    public class RxTableViewReactiveArrayDataSourceSequenceWrapper<TSequence, TElement>: RxTableViewReactiveArrayDataSource<TElement>, IRxTableViewDataSource<TElement> where TSequence : IEnumerable<TElement> where TElement : class
    {
        public RxTableViewReactiveArrayDataSourceSequenceWrapper(Func<UITableView, int, TElement, UITableViewCell> cellFactory) : base(cellFactory)
        {
        }

        public new void ObserveElements(UITableView tableView, IEnumerable<TElement> elements)
        {
            new UIBindingObserver<RxTableViewReactiveArrayDataSourceSequenceWrapper<TSequence, TElement>, IEnumerable<TElement>>(this,
                (tableViewDataSource, sectionmodels) =>
                {
                    tableViewDataSource.ObserveElements(tableView, elements);
                }).OnNext(elements);
        }
    }

    public class RxTableViewReactiveArrayDataSource<TElement> : _RxTableViewReactiveArrayDataSource, ISectionedViewDatasource where TElement: class
    {
        private List<TElement> _itemModels;
        private Func<UITableView, int, TElement, UITableViewCell> _cellFactory;

        public RxTableViewReactiveArrayDataSource(Func<UITableView, int, TElement, UITableViewCell> cellFactory)
        {
            _cellFactory = cellFactory;
        }

        public TElement ModelAtIndex(int index)
        {
            return _itemModels?[index];
        }

        public object ModelAt(NSIndexPath indexPath)
        {
            Debug.Assert(indexPath.Section == 0);
            if (_itemModels == null)
            {
                throw new Exception("Items not bound yet");
            }
            return _itemModels[(int)indexPath.Item];
        }

        internal override UITableViewCell _GetCell(UITableView tableView, NSIndexPath indexpath)
        {
            return _cellFactory(tableView, (int)indexpath.Item, _itemModels[(int)indexpath.Item]);
        }

        public override nint _RowsInSection(UITableView tableview, nint section)
        {
            return _itemModels?.Count ?? 0;
        }

        public void ObserveElements(UITableView tableView, IEnumerable<TElement> elements)
        {
            _itemModels = elements.ToList();
            tableView.ReloadData();
        }
    }


}