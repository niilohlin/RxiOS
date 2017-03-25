using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using NUnit.Framework;
using UIKit;
using RxiOS;

namespace RxiOSTests
{
    [TestFixture]
    public class UILabelTests
    {
        [Test]
        public void SetsText()
        {
            var label = new UILabel();
            var testStrings = new string[] {"test"};
            var subscription = testStrings.ToObservable().Subscribe(label.Rx().Text());
            Assert.AreEqual(label.Text, "test");
        }
    }
}