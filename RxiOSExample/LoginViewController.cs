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
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _viewModel = new LoginViewModel();
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

            AddBindings();

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
        }

        private void AddBindings()
        {
            _viewModel.LoginButtonTitle.BindTo(_loginButton.Rx().Title(UIControlState.Normal)).DisposedBy(_compositeDisposable);
            _usernameTextField.Rx().Text().BindTo(_viewModel.Username).DisposedBy(_compositeDisposable);
            _passwordTextField.Rx().Text().BindTo(_viewModel.Password).DisposedBy(_compositeDisposable);
            _viewModel.LoginButtonEnabled.BindTo(_loginButton.Rx().Enabled()).DisposedBy(_compositeDisposable);

            var loginObserver = Observer.Create(
                (Unit n) => { Debug.Print("next("); }, 
                () =>
                {
                    Debug.Print("completed");
                });
            _loginButton.Rx().Tap().SelectMany(b =>
                {
                    return _viewModel.Login().SubscribeOnMain().Catch((Exception e) =>
                    {
                        ShowError(e.Message);
                        return Observable.Never<Unit>();
                    });
                })
                .Subscribe(loginObserver)
                .DisposedBy(_compositeDisposable);
        }

        private void ShowError(string message)
        {
            var alertController = UIAlertController.Create("Error", message, UIAlertControllerStyle.Alert);
            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            PresentViewController(alertController, true, null);
        }
    }
}