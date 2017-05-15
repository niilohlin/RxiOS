
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using CoreAnimation;

namespace RxiOSExample
{
    public interface ILoginViewModelInputs
    {
        IObservable<string> Username { get; }
        IObservable<string> Password { get; }
        IObservable<Unit> ButtonTap { get; }
    }
    public interface ILoginViewModelOutputs
    {
        IObservable<string> LoginButtonTitle { get; }
        IObservable<string> UsernamePlaceholder { get; }
        IObservable<string> PasswordPlaceholder { get; }
        IObservable<bool> LoginButtonEnabled { get; }
        IObservable<bool> ActivityIndicatorViewShowing { get;  }
        IObservable<Exception> ErrorOccured { get; }
        IObservable<Unit> Login { get; }
    }

    public interface ILoginViewModel
    {
        ILoginViewModelInputs Inputs { get; }
        ILoginViewModelOutputs Outputst { get; }

    }

    public class LoginViewModel: ILoginViewModel, ILoginViewModelInputs, ILoginViewModelOutputs
    {
        public IObservable<string> Username { get; set; }
        public IObservable<string> Password { get; set; }
        public IObservable<Unit> ButtonTap { get; }
        public IObservable<string> LoginButtonTitle { get; }
        public IObservable<string> UsernamePlaceholder { get; }
        public IObservable<string> PasswordPlaceholder { get; }
        public IObservable<bool> LoginButtonEnabled { get; }
        public IObservable<bool> ActivityIndicatorViewShowing { get; }
        public IObservable<Exception> ErrorOccured { get; }
        public ILoginViewModelInputs Inputs => this;
        public ILoginViewModelOutputs Outputst => this;

        public LoginViewModel()
        {
            LoginButtonEnabled = Username
                .CombineLatest(Password, (username, password) => username != "" && password != "")
                .CombineLatest(ActivityIndicatorViewShowing, (enabled, showing) => enabled && !showing);

            Login

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

        public IObservable<Unit> Login { get; }

    }
}