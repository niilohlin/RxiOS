
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
        public readonly IObservable<string> Username;
        public readonly IObservable<string> Password;
        public readonly IObservable<Unit> ButtonTap;
        public readonly IObservable<string> LoginButtonTitle;
        public readonly IObservable<string> UsernamePlaceholder;
        public readonly IObservable<string> PasswordPlaceholder;
        public readonly IObservable<bool> LoginButtonEnabled;
        public readonly IObservable<bool> ActivityIndicatorViewShowing;
        public readonly IObservable<Exception> ErrorOccured;
        public readonly IObservable<Unit> LoginSuccessful;
        public readonly IObservable<bool> LoggingIn;


        public LoginViewModel(IObservable<string> username, IObservable<string> password, IObservable<Unit> buttonTap)
        {
            Username = username;
            Password = password;
            ButtonTap = buttonTap;
            LoginButtonEnabled = Username
                .CombineLatest(Password, (usernamestring, passwordstring) => usernamestring != "" && passwordstring != "")
                .CombineLatest(ActivityIndicatorViewShowing, (enabled, showing) => enabled && !showing);

            LoggingIn = ButtonTap.Scan(false, (b, unit) => !b);
            ActivityIndicatorViewShowing = LoggingIn;

            LoginSuccessful = ButtonTap.CombineLatest(Username, (u, s) => s).CombineLatest(Password,
                ((usernamestring, passwordstring) =>
                {
                    if (LoginSucceded(usernamestring, passwordstring) )
                    {
                        return Observable.Return(Unit.Default)
                            .DelaySubscription(TimeSpan.FromSeconds(3));
                    }
                    return Observable.Never<Unit>();
                })).SelectMany(s => s);
        }

        public bool LoginSucceded(string username, string password)
        {
            return username == "Asdf" && password == "fdsa";
        }

    }
}