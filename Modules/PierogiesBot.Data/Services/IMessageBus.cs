using System;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Models.EntityChanged;

namespace PierogiesBot.Data.Services
{
    public interface IMessageBus
    {
        void Send<T>(T message, string? contract = null);
        void SendEntityChanged<T>(EntityChangedNotification<T> message, string? contract = null) where T : EntityBase;

        IObservable<T> Listen<T>(string? contract = null);
        IObservable<EntityChangedNotification<T>> ListenEntityChanged<T>(string? contract = null) where T : EntityBase;
    }
}