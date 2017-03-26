using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CoreFoundation;
using Foundation;
using UIKit.Reactive.CocoaUnits;
using UIKit.Reactive.Common;

namespace UIKit.Reactive
{
    public static class RxUILabel
    {
        public static Reactive<UILabel> Rx(this UILabel label)
        {
            return new Reactive<UILabel>(label);
        }

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
        public static Reactive<UIButton> Rx(this UIButton button)
        {
            return new Reactive<UIButton>(button);
        }

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

        public static Reactive<UITextField> Rx(this UITextField textField)
        {
            return new Reactive<UITextField>(textField);
        }

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

    public static class DisposableExtensions
    {
        public static void DisposedBy(this IDisposable disposable, CompositeDisposable compositeDisposable)
        {
            compositeDisposable.Add(disposable);
        }
    }
}