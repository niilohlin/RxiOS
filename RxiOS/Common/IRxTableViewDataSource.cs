using System.Collections;
using System.Collections.Generic;

namespace UIKit.Reactive.Common
{
    public interface IRxTableViewDataSource<in TElement>
    {
        void ObserveElements(UITableView tableView, IEnumerable<TElement> elements);
    }
}