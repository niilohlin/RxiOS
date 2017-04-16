using System.Reactive.Disposables;
using RxiOSExample.Models;
using UIKit;
using UIKit.Reactive;

namespace RxiOSExample
{
    public class MainViewController: UIViewController
    {
        private MainViewModel _viewModel = new MainViewModel();
        private UITableView _tableView = new UITableView();
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _viewModel.TodoItems.BindTo(_tableView.Rx().Items((UITableView tv, int row, TodoItem todoItem) =>
            {
                var cell = new UITableViewCell();
                cell.TextLabel.Text = todoItem.Name;
                return cell;
            })).DisposedBy(_compositeDisposable);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            _tableView.Frame = View.Frame;
        }



        
    }
}