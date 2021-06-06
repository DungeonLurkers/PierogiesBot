using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Models.EntityChanged;

namespace PierogiesBot.Data.Services
{
    public class MessageBus : IMessageBus
    {
        private readonly ILogger<MessageBus> _logger;
        private Dictionary<string, object> _subjects;

        public MessageBus(ILogger<MessageBus> logger)
        {
            _logger = logger;
            _subjects = new Dictionary<string, object>();
        }

        public void Send<T>(T message, string? contract = null)
        {
            var typeName = typeof(T).Name;
            var key = contract is not null ? $"{typeName}.{contract}" : typeName;

            if (_subjects.TryGetValue(key, out var maybeSubject) && maybeSubject is ISubject<T> subject)
            {
                _logger.LogDebug($"Send: Sending new message at: {{{key}}}");
                subject.OnNext(message);
            }
            else
            {
                _logger.LogDebug($"Send: New subscription: {{{key}}}");
                var newSubject = new Subject<T>();
                _subjects[key] = newSubject;
            }
        }

        public void SendEntityChanged<T>(EntityChangedNotification<T> message, string? contract = null)
            where T : EntityBase
        {
            var typeName = $"EntityChangedNotification.{typeof(T).Name}";
            var key = contract is not null ? $"{typeName}.{contract}" : typeName;

            if (_subjects.TryGetValue(key, out var maybeSubject) &&
                maybeSubject is ISubject<EntityChangedNotification<T>> subject)
            {
                _logger.LogDebug($"Send: Sending new message at: {{{key}}}");
                subject.OnNext(message);
            }
            else
            {
                _logger.LogDebug($"Send: New subscription: {{{key}}}");
                var newSubject = new Subject<T>();
                _subjects[key] = newSubject;
            }
        }

        public IObservable<T> Listen<T>(string? contract = null)
        {
            var typeName = typeof(T).Name;
            var key = contract is not null ? $"{typeName}.{contract}" : typeName;

            _logger.LogDebug($"Listen: New listener at: {{{key}}}");

            if (_subjects.TryGetValue(key, out var maybeSubject) && maybeSubject is ISubject<T> subject)
                return subject.AsObservable();

            _logger.LogDebug($"Listen: Not found subscriptions at: {{{key}}}");
            _logger.LogDebug($"Listen: Creating new subcription at: {{{key}}}");
            var newSubject = new Subject<T>();
            _subjects[key] = newSubject;
            return newSubject.AsObservable();
        }

        public IObservable<EntityChangedNotification<T>> ListenEntityChanged<T>(string? contract = null)
            where T : EntityBase
        {
            var typeName = $"EntityChangedNotification.{typeof(T).Name}";
            var key = contract is not null ? $"{typeName}.{contract}" : typeName;

            _logger.LogDebug($"ListenEntityChanged: New listener at: {{{key}}}");

            if (_subjects.TryGetValue(key, out var maybeSubject) &&
                maybeSubject is ISubject<EntityChangedNotification<T>> subject)
                return subject.AsObservable();

            _logger.LogDebug($"ListenEntityChanged: Not found subscriptions at: {{{key}}}");
            _logger.LogDebug($"ListenEntityChanged: Creating new subcription at: {{{key}}}");
            var newSubject = new Subject<EntityChangedNotification<T>>();
            _subjects[key] = newSubject;
            return newSubject.AsObservable();
        }
    }
}