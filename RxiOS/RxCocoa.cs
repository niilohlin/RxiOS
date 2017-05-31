using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CoreFoundation;
using Foundation;
using UIKit.Reactive.CocoaUnits;
using UIKit.Reactive.Common;

namespace UIKit.Reactive
{
    public static class RxUILabel
    {
        /// <summary>
        /// Binding for <c>Text</c> property on <c>UILabel</c>.
        /// <example>
        ///     <code>
        ///         Observable.Return("my text").BindTo(label.Rx().Text()).DisposedBy(disposable);
        ///     </code>
        /// </example>
        /// </summary>
        /// <param name="rx"></param>
        /// <returns>BindingObserver for text property on UILabel.</returns>
        /// <seealso cref="AttributedText"/>>
        public static UIBindingObserver<UILabel, string> Text(this Reactive<UILabel> rx) => 
            new UIBindingObserver<UILabel, string>(rx.Parent, (label, s) => label.Text = s);


        /// <summary>
        /// Binding for <c>AttibutedText</c> property on <c>UILabel</c>.
        /// <example>
        ///     <code>
        ///         Observable.Return("my text")
        ///             .Select(text => new NSAttibutedText(new NSString(text)))
        ///             .BindTo(label.Rx().Text())
        ///             .DisposedBy(disposable);
        ///     </code>
        /// </example>
        /// </summary>
        /// <param name="rx"></param>
        /// <returns>BindingObserver for text property on UILabel.</returns>
        /// <seealso cref="AttributedText"/>>
        public static UIBindingObserver<UILabel, NSAttributedString> AttributedText(this Reactive<UILabel> rx) => 
            new UIBindingObserver<UILabel, NSAttributedString>(rx.Parent, (label, s) => label.AttributedText = s);
    }

    public static class RxUIButton
    {

        /// <summary>
        /// Binding for <c>Title</c> property on <c>UIButton</c>
        /// </summary>
        /// <typeparam name="T">T is UIButton</typeparam>
        /// <param name="rx"></param>
        /// <param name="state">The <c>UIControlState</c> for the <c>Title</c></param>
        /// <example>
        ///     <code>
        ///         Observable.Return("press me")
        ///             .BindTo(button.Rx().Title(UIControlState.Normal))
        ///             .DisposedBy(disposable);
        ///     </code>
        /// </example>
        /// <returns></returns>
        public static UIBindingObserver<T, string> Title<T>(this Reactive<T> rx, UIControlState state) where T: UIButton => 
            new UIBindingObserver<T, string>(rx.Parent, (button, s) => button.SetTitle(s, state));

        /// <summary>
        /// Binding for <c>Title</c> property on <c>UIButton</c>
        /// </summary>
        /// <param name="rx"></param>
        /// <example>
        ///     <code>
        ///       button.Rx().Tap()
        ///         .Subscribe(b => Console.WriteLine("Button tapped")
        ///         .DisposedBy(disposable);
        ///     </code>
        /// </example>
        /// <returns></returns>
        public static ControlEvent<UIButton> Tap(this Reactive<UIButton> rx) => 
            rx.ControlEvent(UIControlEvent.TouchUpInside);
    }

    public static class RxUIControl
    {
        /// <summary>
        /// Returns An <c>IOBservable<T></c> instance for T when the <c>UIControlEvent is generated</c>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rx"></param>
        /// <param name="controlEvent"></param>
        /// <returns>An <c>IOBservable</c> instance for <c>T</c> when the <c>UIControlEvent</c> is generated</returns>
        public static ControlEvent<T> ControlEvent<T>(this Reactive<T> rx, UIControlEvent controlEvent) where T: UIControl
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
            where T : UIControl => 
            new UIBindingObserver<T, bool>(rx.Parent, (control, b) => control.Enabled = b);

        public static UIBindingObserver<T, bool> Selected<T>(this Reactive<T> rx)
            where T : UIControl => 
            new UIBindingObserver<T, bool>(rx.Parent, (control, b) => control.Selected = b);

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
        public static ControlProperty<string> Text<T>(this Reactive<T> rx) where T : UITextField => rx.Value();

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
            where T : UIApplication => 
            new UIBindingObserver<T, bool>(rx.Parent, (application, b) => application.NetworkActivityIndicatorVisible = b);
    }

    public static class RxUIView
    {
        public static UIBindingObserver<T, bool> Hidden<T>(this Reactive<T> rx) where T: UIView => 
            new UIBindingObserver<T, bool>(rx.Parent, (view, b) => view.Hidden = b);

        public static UIBindingObserver<T, float> Alpha<T>(this Reactive<T> rx) where T: UIView => 
            new UIBindingObserver<T, float>(rx.Parent, (view, f) => view.Alpha = f);
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
    public static class RxUIRefreshControl
    {
        public static UIBindingObserver<T, bool> Refreshing<T>(this Reactive<T> rx) where T : UIRefreshControl
        {
            return new UIBindingObserver<T, bool>(rx.Parent, (refresh, b) =>
            {
                if (b)
                {
                    refresh.BeginRefreshing();
                }
                else
                {
                    refresh.EndRefreshing();
                }
            });
        }

        public static ControlEvent<T> Refresh<T>(this Reactive<T> rx) where T : UIRefreshControl => 
            rx.ControlEvent(UIControlEvent.ValueChanged);
    }

    public static class RxUIViewController
    {
        public static UIBindingObserver<T, string> Title<T>(this Reactive<T> rx) where T : UIViewController => 
            new UIBindingObserver<T, string>(rx.Parent, (vc, s) => vc.Title = s);
    }

    public static class RxUIAlertAction
    {
        public static UIBindingObserver<T, bool> AlertActionEnabled<T>(this Reactive<T> rx) where T : UIAlertAction => 
            new UIBindingObserver<T, bool>(rx.Parent, (aa, b) => aa.Enabled = b);
    }

    public static class RxUISegmentedControl
    {
        public static ControlProperty<nint> SelectedSegment<T>(this Reactive<T> rx) where T : UISegmentedControl 
            => rx.SegValue();

        public static ControlProperty<nint> SegValue<T>(this Reactive<T> rx) where T : UISegmentedControl 
            => RxUIControl.Value(rx.Parent, seg => seg.SelectedSegment,
                (seg, segIndex) => seg.SelectedSegment = segIndex);
    }

    public static class IObservableExtensions
    {
        public static IObservable<T> Debug<T>(this IObservable<T> obs, string label)
        {
            return obs.Do(
                o =>
                {
                    Console.WriteLine("{0} Next: {1}", label, o);
                },
                e =>
                {
                    Console.WriteLine("{0} Error: {1}", label, e);
                },
                () =>
                {
                    Console.WriteLine("{0} Completed", label);
                }
            );
        }

        public static IObservable<G> SelectFirst<T, G>(this IObservable<T> observable, Func<T, IObservable<G>> selector) 
            => observable.Select(selector).Switch();

        public static IDisposable BindTo<T>(this IObservable<T> observable, IObserver<T> observer) 
            => observable.Subscribe(observer);

        public static IObservable<Tuple<T, G>> CombineLatest<T, G>(this IObservable<T> obs1, IObservable<G> obs2) 
            => obs1.CombineLatest(obs2, Tuple.Create);

        public static IDisposable SubscribeLatest<T, G>(this IObservable<T> obs1, IObservable<G> obs2, Action<T, G> sub) 
            => obs1.CombineLatest(obs2).Subscribe(tuple => sub(tuple.Item1, tuple.Item2));

        public static IObservable<T> SubscribeOnMain<T>(this IObservable<T> observable)
        {
            return Observable.Create<T>(observer =>
            {
                return observable.Subscribe(
                    next => DispatchQueue.MainQueue.DispatchAsync(() => observer.OnNext(next)),
                    exception => DispatchQueue.MainQueue.DispatchAsync(() => observer.OnError(exception)),
                    () => DispatchQueue.MainQueue.DispatchAsync(observer.OnCompleted)
                );
            });
        }
    }

    public static class DisposableExtensions
    {
        public static void DisposedBy(this IDisposable disposable, CompositeDisposable compositeDisposable) 
            => compositeDisposable.Add(disposable);
    }
}