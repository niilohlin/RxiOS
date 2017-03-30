using System;
using System.Net;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace RxiOSExample
{
    public class ConnectionManager
    {
        public IObservable<HttpResponseMessage> Login(string username, string password)
        {
            return Observable.FromAsync(async observer =>
            {
                using (var handler = new HttpClientHandler { Credentials = new NetworkCredential(username, password), UseDefaultCredentials = true})
                using (var client = new HttpClient(handler))
                {
                    return await client.GetAsync("https://api.github.com/user/", observer);
                }
            });
        }
    }
}