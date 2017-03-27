
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

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

        public LoginViewModel()
        {
            LoginButtonEnabled = Username.CombineLatest(Password,
                (username, password) => username != "" && password != "");
        }
    }
}