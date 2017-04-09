using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using Intents;
using Newtonsoft.Json;

namespace RxiOSExample
{
    public class ConnectionManager
    {
        private IObservable<T> Get<T>(Uri uri)
        {
            return
                Observable.Using(
                    () =>
                        new HttpClient(new HttpClientHandler
                        {
                            Credentials = new NetworkCredential("", ""),
                            UseDefaultCredentials = true
                        }),
                    client => client.GetStreamAsync(uri)
                        .ToObservable()
                        .Select(
                            x =>
                                Observable.FromAsync(
                                        () => new StreamReader(x, Encoding.UTF8, false, 1024, true).ReadLineAsync())
                                    .Repeat())
                        .Concat()
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(JsonConvert.DeserializeObject<T>));
                
            //return Observable.FromAsync(async cancellationToken =>
            //{
            //    using (var handler = new HttpClientHandler { Credentials = new NetworkCredential(username, password), UseDefaultCredentials = true})
            //    using (var client = new HttpClient(handler))
            //    {
            //        var result = await client.GetAsync(, cancellationToken);
            //        if (!result.IsSuccessStatusCode)
            //        {
            //        }
                    
            //        return result;
            //    }
            //});
        }
    }
}