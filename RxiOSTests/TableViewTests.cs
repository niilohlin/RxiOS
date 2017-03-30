using System;
using System.Reactive.Linq;
using Foundation;
using NUnit.Framework;
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
            var cell1 = tableView.Source.GetCell(tableView, indexPath);
            Assert.True((cell1?.TextLabel?.Text ?? "") == "First Element");

            subscription.Dispose();
        }

        [Test]
        public void Pass()
        {
            var tableView = new UITableView();
            var models= new [] {"First", "Second", "Third", "Fourth"};

            var observable = Observable.Return(models);

            var subscription = tableView.Rx().Items(observable)((tv, row, model) =>
            {
                var cell = new UITableViewCell();
                cell.TextLabel.Text = model;
                return cell;
            });

            var indexPath = NSIndexPath.FromRowSection(0, 0);
            Assert.AreEqual(tableView.Source.GetCell(tableView, indexPath).TextLabel.Text, "First");
            subscription.Dispose();
            
        }
    }
}