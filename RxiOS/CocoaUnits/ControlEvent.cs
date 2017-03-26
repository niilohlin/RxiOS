using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;

namespace UIKit.Reactive.CocoaUnits
{
    public class ControlEvent<TProperty> : IObservable<TProperty>
    {
        private readonly IObservable<TProperty> _events;
        public ControlEvent(IObservable<TProperty> events)
        {
            _events = events; //.SubscribeOn(Scheduler.Default);
        }
        public IDisposable Subscribe(IObserver<TProperty> observer)
        {
            return _events.Subscribe(observer);
        }

        public IObservable<TProperty> ToObservable()
        {
            return _events;
        }
    }
}