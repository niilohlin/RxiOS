
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using CoreAnimation;

namespace RxiOSExample
{
    public class LoginViewModel
    {
        private ConnectionManager _connectionManager;
        public BehaviorSubject<string> Username = new BehaviorSubject<string>("");
        public BehaviorSubject<string> Password = new BehaviorSubject<string>("");
        public IObservable<string> LoginButtonTitle = Observable.Return("Login");
        public IObservable<string> UsernamePlaceholder = Observable.Return("Username");
        public IObservable<string> PasswordPlaceholder = Observable.Return("Password");
        public IObservable<bool> LoginButtonEnabled;
        public BehaviorSubject<bool> ActivityIndicatorViewShowing = new BehaviorSubject<bool>(false);
        public Subject<Exception> ErrorOccured = new Subject<Exception>();

        public LoginViewModel(ConnectionManager manager)
        {
            _connectionManager = manager;
            LoginButtonEnabled = Username
                .CombineLatest(Password, (username, password) => username != "" && password != "")
                .CombineLatest(ActivityIndicatorViewShowing, (enabled, showing) => enabled && !showing);
        }

        public IObservable<Unit> Login()
        {
            ActivityIndicatorViewShowing.OnNext(true);
            return _connectionManager.Login(Username.Value, Password.Value)
                .Select(response => Unit.Default)
                .Finally(() => ActivityIndicatorViewShowing.OnNext(false))
                .Catch(Observable.Never<Unit>());
            ;
        }
    }
}