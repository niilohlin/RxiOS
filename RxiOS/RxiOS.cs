using System;
using System.Reactive;
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
}