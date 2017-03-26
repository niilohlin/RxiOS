using System;
using System.Reactive;
using System.Threading;
using CoreFoundation;

namespace UIKit.Reactive.CocoaUnits
{
    public sealed class UIBindingObserver<TUIElement, TValue>: IObserver<TValue>
    {
        private TUIElement _uiElement;
        private Action<TUIElement, TValue> _binding;

        public UIBindingObserver(TUIElement uiElement, Action<TUIElement, TValue> binding)
        {
            _uiElement = uiElement;
            _binding = binding;
        }

        public void OnNext(TValue value)
        {
            if (!DispatchQueue.CurrentQueue.Equals(DispatchQueue.MainQueue))
            {
                DispatchQueue.MainQueue.DispatchAsync(() => this.OnNext(value));
                return;
            }
            if (_uiElement == null)
            {
                return;
            }
            _binding(_uiElement, value);
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
        }
    }
}