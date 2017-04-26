using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using RxiOSExample.Models;
using UIKit;
using UIKit.Reactive;
using System;
using Foundation;

namespace RxiOSExample
{
    public class TodoViewController: UIViewController
    {
        private readonly MainViewModel _viewModel = new MainViewModel();
        private readonly UITableView _tableView = new UITableView();
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public override void ViewDidLoad()
        {
            View.BackgroundColor = UIColor.White;
            base.ViewDidLoad();
            _viewModel.TodoItems.BindTo(_tableView.Rx().Items((UITableView tv, int row, TodoItem todoItem) =>
            {
                var cell = new UITableViewCell();
                cell.TextLabel.Text = todoItem.Name;
                return cell;
            })).DisposedBy(_compositeDisposable);

            _viewModel
                .TodoItems
                .Select(items => items.Select(TodoTableViewCell.Height))
                .BindTo(_tableView.Rx().RowHeights<UITableView, TodoItem>())
                .DisposedBy(_compositeDisposable);

            Observable.Return("").Subscribe(s => Debug.Print("" + s));
            _tableView.Rx()
                .ItemSelected<UITableView, TodoItem>()
                .Subscribe(indexPath =>
                    {
                        var todoItems = _viewModel.TodoItems.Value;
                        todoItems[indexPath.Row] = todoItems[indexPath.Row].Switch();
                        _viewModel.TodoItems.OnNext(todoItems);
                    }
                ).DisposedBy(_compositeDisposable);

            View.AddSubview(_tableView);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            _tableView.Frame = View.Frame;
        }
    }
}