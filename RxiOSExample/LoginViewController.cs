using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CoreGraphics;
using UIKit;
using UIKit.Reactive;

namespace RxiOSExample
{
    public class LoginViewController: UIViewController
    {
        private UITextField _usernameTextField;
        private UITextField _passwordTextField;
        private UIButton _loginButton;
        private LoginViewModel _viewModel;
        private UIActivityIndicatorView _activityIndicatorView;
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public LoginViewController()
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _usernameTextField = new UITextField()
            {
                BackgroundColor = UIColor.LightGray,
                Placeholder = "Username"
            };
            _passwordTextField = new UITextField()
            {
                BackgroundColor = UIColor.LightGray,
                Placeholder = "Password",
                SecureTextEntry = true
            };
            _loginButton = new UIButton();
            _loginButton.SetTitleColor(UIColor.LightGray, UIControlState.Disabled);
            _loginButton.SetTitleColor(UIColor.Black, UIControlState.Normal);

            _activityIndicatorView =
                new UIActivityIndicatorView
                {
                    TintColor = UIColor.Black,
                    ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray
                };

            AddBindings();

            View.AddSubview(_activityIndicatorView);
            View.AddSubview(_usernameTextField);
            View.AddSubview(_passwordTextField);
            View.AddSubview(_loginButton);
            View.BackgroundColor = UIColor.White;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            SetFrames();
        }

        private void SetFrames()
        {
            const int width = 200;
            const int height = 25;
            _usernameTextField.Frame = new CGRect(View.Frame.Width / 2 - width / 2, 200, width, height);
            _passwordTextField.Frame = new CGRect(_usernameTextField.Frame.X, _usernameTextField.Frame.Bottom + 8, width, height);
            _loginButton.Frame = new CGRect(_usernameTextField.Frame.X, _passwordTextField.Frame.Bottom + 8, width, height);
            _activityIndicatorView.Frame = new CGRect(_loginButton.Frame.GetMidX() - 10, _loginButton.Frame.Bottom + 8, 20, 20);
        }

        private void AddBindings()
        {
            _viewModel = new LoginViewModel(_usernameTextField.Rx().Text(), _passwordTextField.Rx().Text(), _loginButton.Rx().Tap().Select(button => Unit.Default));
            _viewModel.LoginButtonTitle.BindTo(_loginButton.Rx().Title(UIControlState.Normal)).DisposedBy(_compositeDisposable);
            _viewModel.LoginButtonEnabled.BindTo(_loginButton.Rx().Enabled()).DisposedBy(_compositeDisposable);
            _viewModel.ActivityIndicatorViewShowing.BindTo(_activityIndicatorView.Rx().Animating()).DisposedBy(_compositeDisposable);
            _viewModel.ErrorOccured.SubscribeOnMain().Subscribe(e => ShowError(e.Message)).DisposedBy(_compositeDisposable);

            var loginObserver = Observer.Create((Unit n) => GotoMain());

            _viewModel.LoginSuccessful.Subscribe(loginObserver).DisposedBy(_compositeDisposable);
        }

        private void GotoMain()
        {
            var mainViewController = new TodoViewController();
            var navigationController = new UINavigationController(mainViewController);
            PresentViewController(navigationController, true, null);
            
        }

        private void ShowError(string message)
        {
            var alertController = UIAlertController.Create("Error", message, UIAlertControllerStyle.Alert);
            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            PresentViewController(alertController, true, null);
        }
    }
}