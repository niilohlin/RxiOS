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
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();


        public override void ViewDidLoad()
        {
            _viewModel = new LoginViewModel();
            _usernameTextField = new UITextField(new CGRect(10, 10, 100, 100));
            _usernameTextField.BackgroundColor = UIColor.LightGray;
            _usernameTextField.Placeholder = "test";
            _passwordTextField = new UITextField(new CGRect(10, 110, 100, 100));
            _passwordTextField.BackgroundColor = UIColor.LightGray;
            _passwordTextField.Placeholder = "test";
            _loginButton = new UIButton(new CGRect(10, 210, 100, 100));
            _loginButton.SetTitleColor(UIColor.LightGray, UIControlState.Disabled);
            _loginButton.SetTitleColor(UIColor.Black, UIControlState.Normal);

            _viewModel.LoginButtonTitle.BindTo(_loginButton.Rx().Title(UIControlState.Normal)).DisposedBy(_compositeDisposable);
            _usernameTextField.Rx().Text().BindTo(_viewModel.Username).DisposedBy(_compositeDisposable);
            _passwordTextField.Rx().Text().BindTo(_viewModel.Password).DisposedBy(_compositeDisposable);
            _viewModel.LoginButtonEnabled.BindTo(_loginButton.Rx().Enabled()).DisposedBy(_compositeDisposable);


            var loginObserver = Observer.Create(
                (Unit n) => { }, 
                () =>
                {
                    Debug.Print("completed");
                });
            _loginButton.Rx().Tap().SelectMany(b =>
                {
                    return _viewModel.Login().Catch((Exception e) =>
                    {
                        ShowError(e.Message);
                        return Observable.Never<Unit>();
                    });

                })
                .Subscribe(loginObserver)
                .DisposedBy(_compositeDisposable);



            View.AddSubview(_usernameTextField);
            View.AddSubview(_passwordTextField);
            View.AddSubview(_loginButton);
            View.BackgroundColor = UIColor.White;
        }

        private void ShowError(string message)
        {
            InvokeOnMainThread(() =>
            {
                var alertController = UIAlertController.Create("Error", message, UIAlertControllerStyle.Alert);
                alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                PresentViewController(alertController, true, null);
            });
        }
    }
}