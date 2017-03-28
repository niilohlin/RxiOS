
using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;

namespace UIKit.Reactive.CocoaUnits
{
    public class ControlProperty<TProperty>: IObservable<TProperty>, IObserver<TProperty>
    {
        internal readonly IObservable<TProperty> Values;
        internal readonly IObserver<TProperty> ValueSink;

        public ControlProperty(IObservable<TProperty> values, IObserver<TProperty> valueSink)
        {
            Values = values.SubscribeOnMain();
            ValueSink = valueSink.AsObserver();
        }

        public IDisposable Subscribe(IObserver<TProperty> observer)
        {
            return Values.Subscribe(observer);
        }

        public void OnNext(TProperty value)
        {
            ValueSink.OnNext(value);
        }

        public void OnError(Exception error)
        {
            ValueSink.OnError(error);
        }

        public void OnCompleted()
        {
            ValueSink.OnCompleted();
        }

        public IObservable<TProperty> ToObservable()
        {
            return Values;
        }
    }

    public static class ControlPropertyExtensions
    {
        public static ControlProperty<string> OrEmpty(this ControlProperty<string> controlProperty)
        {
            var values = controlProperty.Values.Select(str => str ?? "");
            return new ControlProperty<string>(values, controlProperty.ValueSink);
        }
    }
}