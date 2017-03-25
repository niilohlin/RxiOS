using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using NUnit.Framework;
using UIKit;
using RxiOS;

namespace RxiOSTests
{
    [TestFixture]
    public class RxiOSTests
    {
        [Test]
        public void SetsLabelText()
        {
            var label = new UILabel();
            var testStrings = new string[] {"test"};
            var subscription = testStrings.ToObservable().Subscribe(label.Rx().Text());
            Assert.AreEqual(label.Text, "test");
            subscription.Dispose();
        }

        [Test]
        public void SetsButtonTitle()
        {
            var button = new UIButton();
            var testStrings = new string[] {"test"};
            var subscription = testStrings.ToObservable().Subscribe(button.Rx().Title(UIControlState.Normal));
            Assert.AreEqual(button.Title(UIControlState.Normal), "test");
            subscription.Dispose();
            
        }

        [Test]
        public void ButtonTap()
        {
            var button = new UIButton();
            var subscription = button.Rx().Tap().Subscribe(b => b.Enabled = false);
            Assert.True(button.Enabled);
            var touchUpinsidEvent = UIControlEvent.TouchUpInside;
            button.SendActionForControlEvents(touchUpinsidEvent);
            Assert.False(button.Enabled);
            subscription.Dispose();
        }
    }
}