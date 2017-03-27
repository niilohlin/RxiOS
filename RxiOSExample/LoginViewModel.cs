
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace RxiOSExample
{
    public class LoginViewModel
    {
        public BehaviorSubject<string> Username = new BehaviorSubject<string>("");
        public BehaviorSubject<string> Password= new BehaviorSubject<string>("");
        public IObservable<string> LoginButtonTitle = Observable.Return("Login");
        public IObservable<string> UsernamePlaceholder = Observable.Return("Username");
        public IObservable<string> PasswordPlaceholder = Observable.Return("Password");
        public IObservable<bool> LoginButtonEnabled;
        public BehaviorSubject<bool> ActivityIndicatorViewShowing = new BehaviorSubject<bool>(false);

        public LoginViewModel()
        {
            LoginButtonEnabled = Username.CombineLatest(Password,
                (username, password) => username != "" && password != "");
        }

        public IObservable<Unit> Login()
        {
            ActivityIndicatorViewShowing.OnNext(true);
            return Observable.Create<Unit>(observer =>
            {
                if (Username.Value == "username" && Password.Value == "password")
                {
                    observer.OnCompleted();
                }
                else
                {
                    observer.OnError(new Exception("wrong username or password"));
                }
                ActivityIndicatorViewShowing.OnNext(false);
                return Disposable.Empty;
            }).Delay(TimeSpan.FromSeconds(2));

        }
    }
}