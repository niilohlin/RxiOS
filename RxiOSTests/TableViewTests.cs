using System;
using System.Reactive.Linq;
using Foundation;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UIKit;
using UIKit.Reactive;

namespace RxiOSTests
{
    [TestFixture]
    public class TableViewTests
    {

        [Test]
        public void BindSequencesOfElementsToTableViewRows()
        {
            var tableView = new UITableView();
            var items = Observable.Return(new[] {"First Item", "Second Item", "Third Item"});
            var subscription = items.BindTo(tableView.Rx().Items((UITableView tv, int row, string element) =>
            {
                var cell = new UITableViewCell();
                cell.TextLabel.Text = element;
                return cell;
            }));

            var indexPath = NSIndexPath.FromRowSection(0, 0);
            Assert.AreEqual(tableView.Source.GetCell(tableView, indexPath).TextLabel.Text,  "First Item");

            subscription.Dispose();
        }

        [Test]
        public void BindSequencesOfElementsToTableViewRows_WithCellIdentifier()
        {
            var tableView = new UITableView();
            tableView.RegisterClassForCellReuse(typeof(UITableViewCell), nameof(UITableViewCell));
            var items = Observable.Return(new [] {"First", "Second", "Third", "Fourth"});

//Action<int, TElement, TCell> cellInitializer
            var subscription = items.BindTo(tableView.Rx().Items(nameof(UITableViewCell),
                (int row, string element, UITableViewCell cell) => cell.TextLabel.Text = element ));

            var indexPath = NSIndexPath.FromRowSection(0, 0);
            Assert.AreEqual(tableView.Source.GetCell(tableView, indexPath).TextLabel.Text, "First");
            subscription.Dispose();
            
        }
    }
}