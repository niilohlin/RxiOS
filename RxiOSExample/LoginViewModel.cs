
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
        public BehaviorSubject<string> Username = new BehaviorSubject<string>("");
        public BehaviorSubject<string> Password = new BehaviorSubject<string>("");
        public IObservable<string> LoginButtonTitle = Observable.Return("Login");
        public IObservable<string> UsernamePlaceholder = Observable.Return("Username");
        public IObservable<string> PasswordPlaceholder = Observable.Return("Password");
        public IObservable<bool> LoginButtonEnabled;
        public BehaviorSubject<bool> ActivityIndicatorViewShowing = new BehaviorSubject<bool>(false);
        public Subject<Exception> ErrorOccured = new Subject<Exception>();

        public LoginViewModel()
        {
            LoginButtonEnabled = Username
                .CombineLatest(Password, (username, password) => username != "" && password != "")
                .CombineLatest(ActivityIndicatorViewShowing, (enabled, showing) => enabled && !showing);
        }

        public IObservable<Unit> Login()
        {
            ActivityIndicatorViewShowing.OnNext(true);
            if (Username.Value == "Asdf" && Password.Value == "fdsa")
            {
                return Observable.Return(Unit.Default)
                    .DelaySubscription(TimeSpan.FromSeconds(3))
                    .Finally(() => ActivityIndicatorViewShowing.OnNext(false));
            }
            ErrorOccured.OnNext(new Exception("Wrong username or password"));
            ActivityIndicatorViewShowing.OnNext(false);
            return Observable.Never<Unit>();
        }
    }
}