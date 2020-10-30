using System;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Module.Core.Extensions
{
    public static class ObservableExtensions
    {
        public static ILogger Logger { get; set; } = new NullLogger<string>();

        public static IObservable<T> LogInfo<T>(this IObservable<T> observable, Func<T, string> logCreator, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            return Observable.Create<T>(o => observable.Subscribe(obj =>
                {
                    var msg = logCreator(obj);
                    Logger.LogInformation($"{memberName}: {msg}");
                    o.OnNext(obj);
                },
                o.OnError,
                o.OnCompleted));
        }

        public static IObservable<T> LogDebug<T>(this IObservable<T> observable, Func<T, string> logCreator, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            return Observable.Create<T>(o => observable.Subscribe(obj =>
                {
                    var msg = logCreator(obj);
                    Logger.LogDebug($"{memberName}: {msg}");
                    o.OnNext(obj);
                },
                o.OnError,
                o.OnCompleted));
        }

        public static IObservable<T> LogError<T>(this IObservable<T> observable, Func<T, string> logCreator, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            return Observable.Create<T>(o => observable.Subscribe(obj =>
                {
                    var msg = logCreator(obj);
                    Logger.LogError($"{memberName}: {msg}");
                    o.OnNext(obj);
                },
                o.OnError,
                o.OnCompleted));
        }

        public static IObservable<T> LogTrace<T>(this IObservable<T> observable, Func<T, string> logCreator, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            return Observable.Create<T>(o => observable.Subscribe(obj =>
                {
                    var msg = logCreator(obj);
                    Logger.LogTrace($"{memberName}: {msg}");
                    o.OnNext(obj);
                },
                o.OnError,
                o.OnCompleted));
        }
    }
}