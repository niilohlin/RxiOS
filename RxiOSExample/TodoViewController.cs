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
            _tableView.RegisterClassForCellReuse(typeof(TodoTableViewCell), nameof(TodoTableViewCell));
            _viewModel
                .TodoItems
                .BindTo(_tableView.Rx().Items<UITableView, TodoItem, TodoTableViewCell>(
                    nameof(TodoTableViewCell),
                    (row, todoItem, cell) => cell.Configure(todoItem))
                )
                .DisposedBy(_compositeDisposable);

            _viewModel
                .TodoItems
                .Select(items => items.Select(TodoTableViewCell.Height))
                .BindTo(_tableView.Rx().RowHeights())
                .DisposedBy(_compositeDisposable);

            _tableView.Rx()
                .ItemSelected()
                .Select(indexPath =>
                    {
                        var todoItems = _viewModel.TodoItems.Value;
                        todoItems[indexPath.Row] = todoItems[indexPath.Row].Switch();
                        return todoItems;
                    }
                )
                .BindTo(_viewModel.TodoItems)
                .DisposedBy(_compositeDisposable);

            View.AddSubview(_tableView);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            _tableView.Frame = View.Frame;
        }
    }
}