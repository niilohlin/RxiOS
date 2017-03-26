using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using NUnit.Framework;
using UIKit;
using UIKit.Reactive;

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
            Assume.That(button.Enabled);
            button.SendActionForControlEvents(UIControlEvent.TouchUpInside);
            Assert.False(button.Enabled);
            subscription.Dispose();
        }

        [Test]
        public void ButtonEnabled()
        {
            var button = new UIButton();
            var enabled = new bool[] {false};
            Assume.That(button.Enabled);
            var sub = enabled.ToObservable().Subscribe(button.Rx().Enabled());
            Assert.False(button.Enabled);
        }

        [Test]
        public void UITextFieldTextProperty()
        {
            var textField = new UITextField();
            Assume.That(textField.Text == "");
            var input = new string[] {"", "i", "in", "inp", "inpu", "input"};
            var subscription = input.ToObservable().Subscribe(textField.Rx().Text());
            Assert.AreEqual(textField.Text, input.Last());
            subscription.Dispose();
        }

        [Test]
        public void TypicalLoginTest()
        {
            var usernameTextField = new UITextField();
            var passwordTextField = new UITextField();
            var loginButton = new UIButton() ;
            var compositeDisposable = new CompositeDisposable();
            

            var usernameInput = new string[] {"u", "us", "use", "user"};
            var passwordInput = new string[] {"p", "pa", "pas", "pass"};

            usernameTextField.Rx().Text()
                .CombineLatest(passwordTextField.Rx().Text(), Tuple.Create)
                .Select(tuple => tuple.Item1 != "" && tuple.Item2 != "")
                .Subscribe(loginButton.Rx().Enabled())
                .DisposedBy(compositeDisposable);

            Assume.That(!loginButton.Enabled);

            foreach (var username in usernameInput)
            {
                usernameTextField.Text = username;
                usernameTextField.SendActionForControlEvents(UIControlEvent.ValueChanged);
            }
            foreach (var password in passwordInput)
            {
                passwordTextField.Text = password;
                passwordTextField.SendActionForControlEvents(UIControlEvent.ValueChanged);
            }

            Assert.True(loginButton.Enabled);

            compositeDisposable.Dispose();
        }

            
    }
}