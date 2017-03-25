using System;
using System.Reactive;
using System.Reactive.Linq;
using Foundation;
using UIKit;
namespace RxiOS
{
    public static class RxUILabel
    {
        public static Reactive<UILabel> Rx(this UILabel label)
        {
            return new Reactive<UILabel>(label);
        }

        public static IObserver<string> Text(this Reactive<UILabel> rx)
        {
            return Observer.Create<string>(str => rx.Parent.Text = str);
        }

        public static IObserver<NSAttributedString> AttributedText(this Reactive<UILabel> rx)
        {
            return Observer.Create<NSAttributedString>(attrString => rx.Parent.AttributedText = attrString);
        }
    }

    public static class RxUIButton
    {
        public static Reactive<UIButton> Rx(this UIButton button)
        {
            return new Reactive<UIButton>(button);
        }

        public static IObserver<string> Title(this Reactive<UIButton> rx, UIControlState state)
        {
            return Observer.Create<string>(str => rx.Parent.SetTitle(str, state));
        }

        public static IObservable<UIButton> Tap(this Reactive<UIButton> rx)
        {
            return Observable.FromEventPattern(ev => rx.Parent.TouchUpInside += ev, ev => rx.Parent.TouchUpInside -= ev)
                    .Select((o) => rx.Parent);
        }
    }
}