using System;
using System.Collections.Generic;
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

            var subscription = items.BindTo(tableView.Rx().Items(nameof(UITableViewCell),
                (int row, string element, UITableViewCell cell) => cell.TextLabel.Text = element ));

            var indexPath = NSIndexPath.FromRowSection(0, 0);
            Assert.AreEqual(tableView.Source.GetCell(tableView, indexPath).TextLabel.Text, "First");
            subscription.Dispose();
            
        }

        private class SectionModel
        {
            public string Header;
            public List<string> Items;
        }
        [Test]
        public void BindSequencesOfElementsToTableViewRows_WithSections()
        {
            //var sections = Observable.Return(new[]
            //{
            //    new SectionModel {Header = "First Section", Items = new List<string> {"Item1", "Item2", "Item3"}},
            //    new SectionModel {Header = "Second Section", Items = new List<string> {"Item4", "Item5", "Item6"}},
            //    new SectionModel {Header = "Third Section", Items = new List<string> {"Item7", "Item8", "Item9"}}
            //});

            //var tableView = new UITableView();
            //var subscription =
            //    sections.BindTo(
            //        tableView.Rx()
            //            .Sections((UITableView tv, int section, SectionModel sectionModel) => sectionModel.Items,
            //                (UITableView tv, int section, SectionModel sectionModel) => sectionModel.Header,
            //                (UITableView tv, NSIndexPath ip, string item) =>
            //                {
            //                    var cell = new UITableViewCell();
            //                    cell.TextLabel.Text = item;
            //                    return cell;
            //                }));

            //var indexPath = NSIndexPath.FromRowSection(1, 1);
            //Assert.AreEqual(tableView.Source.GetCell(tableView, indexPath).TextLabel.Text, "Item5");
            //subscription.Dispose();


        }
    }
}