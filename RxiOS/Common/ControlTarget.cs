using System;
using System.Diagnostics;
using System.Runtime.Remoting.Channels;
using ObjCRuntime;

namespace UIKit.Reactive.Common
{
    sealed class ControlTarget: IDisposable
    {
        private Action<UIControl> _callback;
        private readonly UIControl _control;
        private readonly UIControlEvent _controlEvent;

        internal ControlTarget(UIControl control, UIControlEvent controlEvent, Action<UIControl> callback)
        {
            _control = control;
            _controlEvent = controlEvent;
            _callback = callback;
            control.AddTarget(EventHandler, controlEvent);
        }

        private void EventHandler(object sender, EventArgs e)
        {
            if (_callback == null || _control == null)
            {
                return;
            }
            _callback(_control);
        }

        public void Dispose()
        {
            _control.RemoveTarget(EventHandler, _controlEvent);
            _control?.Dispose();
            _callback = null;

        }
    }
}