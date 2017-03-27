using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CoreFoundation;
using Foundation;
using UIKit.Reactive.CocoaUnits;
using UIKit.Reactive.Common;
using UIKit;t

namespace UIKit.Reactive
{
    public static class RxUILabel
    {
        public static UIBindingObserver<UILabel, string> Text(this Reactive<UILabel> rx)
        {
            return new UIBindingObserver<UILabel, string>(rx.Parent, (label, s) => label.Text = s);
        }

        public static UIBindingObserver<UILabel, NSAttributedString> AttributedText(this Reactive<UILabel> rx)
        {
            return new UIBindingObserver<UILabel, NSAttributedString>(rx.Parent, (label, s) => label.AttributedText = s);
        }
    }

    public static class RxUIButton
    {

        public static UIBindingObserver<T, string> Title<T>(this Reactive<T> rx, UIControlState state) where T: UIButton
        {
            return new UIBindingObserver<T, string>(rx.Parent, (button, s) => button.SetTitle(s, state));
        }

        public static ControlEvent<UIButton> Tap(this Reactive<UIButton> rx)
        {
            return rx.CreateControlEvent(UIControlEvent.TouchUpInside);
        }
    }

    public static class RxUIControl
    {
        public static ControlEvent<T> CreateControlEvent<T>(this Reactive<T> rx, UIControlEvent controlEvent) where T: UIControl
        {
            var source = Observable.Create<T>(observer =>
            {
                if (rx.Parent == null)
                {
                    observer.OnCompleted();
                    return Disposable.Empty;
                }
                var controltarget = new ControlTarget(rx.Parent, controlEvent, uiControl => observer.OnNext(rx.Parent));
                return Disposable.Create(() => controltarget.Dispose() );
            }); //.TakeUntil(self.deallocated?);

            return new ControlEvent<T>(source);
        }

        public static UIBindingObserver<T, bool> Enabled<T>(this Reactive<T> rx)
            where T : UIControl
        {
            return new UIBindingObserver<T, bool>(rx.Parent, (control, b) => control.Enabled = b);
        }

        public static UIBindingObserver<T, bool> Selected<T>(this Reactive<T> rx)
            where T : UIControl
        {
            return new UIBindingObserver<T, bool>(rx.Parent, (control, b) => control.Selected = b);
        }

        internal static ControlProperty<T> Value<T, C>(C control, Func<C, T> getter, Action<C, T> setter) where C : UIControl
        {
            var source = Observable.Create<T>(observer =>
            {
                if (control == null)
                {
                    observer.OnCompleted();
                    return Disposable.Empty;
                }
                observer.OnNext(getter(control));

                var controlTarget = new ControlTarget(control, (UIControlEvent.AllEditingEvents | UIControlEvent.ValueChanged),
                    c =>
                    {
                        if (control == null)
                        {
                            return;
                        }
                        observer.OnNext(getter(control));
                    });
                return Disposable.Create(() => controlTarget.Dispose());

            });
            var bindingObserver = new UIBindingObserver<C, T>(control, setter);
            return new ControlProperty<T>(source, bindingObserver);
        }


    }

    public static class RxUITextField
    {
        public static ControlProperty<string> Text<T>(this Reactive<T> rx) where T : UITextField
        {
            return rx.Value();
        }
        public static ControlProperty<string> Value<T>(this Reactive<T> rx) where T : UITextField
        {
            return RxUIControl.Value(rx.Parent, textFied => textFied.Text, (textField, text) =>
            {
                if (textField.Text != text)
                {
                    textField.Text = text;
                }
            });
        }
    }

    public static class RxUIApplication
    {
        public static UIBindingObserver<T, bool> NetworkActivityIndicatorVisible<T>(this Reactive<T> rx)
            where T : UIApplication
        {
            return new UIBindingObserver<T, bool>(rx.Parent, (application, b) => application.NetworkActivityIndicatorVisible = b);
        }
    }

    public static class RxUIView
    {
        public static UIBindingObserver<T, bool> Hidden<T>(this Reactive<T> rx) where T: UIView
        {
            return new UIBindingObserver<T, bool>(rx.Parent, (view, b) => view.Hidden = b);
        }

        public static UIBindingObserver<T, float> Alpha<T>(this Reactive<T> rx) where T: UIView
        {
            return new UIBindingObserver<T, float>(rx.Parent, (view, f) => view.Alpha = f);
        }
    }

    public static class RxUIActivityIndicatorView
    {
        public static UIBindingObserver<T, bool> Animating<T>(this Reactive<T> rx) where T : UIActivityIndicatorView
        {
            return new UIBindingObserver<T, bool>(rx.Parent, (activity, b) =>
            {
                if (b)
                {
                    activity.StartAnimating();
                }
                else
                {
                    activity.StopAnimating();
                }
            });
        }
    }

    public static class RxUISegmentedControl
    {
        public static ControlProperty<nint> Value<T>(this Reactive<T> rx) where T : UISegmentedControl
        {
            return RxUIControl.Value(rx.Parent, seg => seg.SelectedSegment,
                (seg, segIndex) => seg.SelectedSegment = segIndex);
        }
    }

    public static class IObservableExtensions
    {
        public static IDisposable BindTo<T>(this IObservable<T> observable, IObserver<T> observer)
        {
            return observable.Subscribe(observer);
        }

        public static IObservable<Tuple<T, G>> CombineLatest<T, G>(this IObservable<T> obs1, IObservable<G> obs2)
        {
            return obs1.CombineLatest(obs2, Tuple.Create);
        }

        public static IDisposable SubscribeLatest<T, G>(this IObservable<T> obs1, IObservable<G> obs2, Action<T, G> sub)
        {
            return obs1.CombineLatest(obs2).Subscribe(tuple => sub(tuple.Item1, tuple.Item2));
        }

        public static IObservable<T> SubscribeOnMain<T>(this IObservable<T> observable)
        {
            return Observable.Create<T>(observer =>
            {
                var a = new NSObject();
                return observable.Subscribe(
                    next => a.InvokeOnMainThread(() => observer.OnNext(next)) ,
                    exception => a.InvokeOnMainThread(() => observer.OnError(exception)) ,
                    () => a.InvokeOnMainThread(observer.OnCompleted)
                );
            });


        }
    }

    public static class DisposableExtensions
    {
        public static void DisposedBy(this IDisposable disposable, CompositeDisposable compositeDisposable)
        {
            compositeDisposable.Add(disposable);
        }
    }
}