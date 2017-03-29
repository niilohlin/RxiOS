using System;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace RxiOSExample
{
    public class ConnectionManager
    {
        public IObservable<HttpResponseMessage> GetGithub()
        {
            var client = new HttpClient();
            return Observable.Create<HttpResponseMessage>(async observer =>
            {
                var response = await client.GetAsync("https://api.github.com/");
                observer.OnNext(response);
                observer.OnCompleted();
                return Disposable.Empty;
            });
        }

    }
}