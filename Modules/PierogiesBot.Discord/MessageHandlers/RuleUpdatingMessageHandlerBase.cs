using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Models.EntityChanged;
using PierogiesBot.Data.Services;

namespace PierogiesBot.Discord.MessageHandlers
{
    public abstract class RuleUpdatingMessageHandlerBase<T> where T : EntityBase
    {
        private readonly ILogger<RuleUpdatingMessageHandlerBase<T>> _logger;
        private readonly IMessageBus _messageBus;
        private readonly IRepository<T> _repository;

        public RuleUpdatingMessageHandlerBase(IMessageBus messageBus, ILogger<RuleUpdatingMessageHandlerBase<T>> logger,
            IRepository<T> repository)
        {
            _messageBus = messageBus;
            _logger = logger;
            _repository = repository;

            Rules = new Lazy<List<T>>(() =>
                repository.GetAll().ConfigureAwait(false).GetAwaiter().GetResult().ToList());

            _messageBus.ListenEntityChanged<T>()
                .Do(OnBotRulesChanged)
                .Subscribe();
        }

        protected Lazy<List<T>> Rules { get; }

        private void OnBotRulesChanged(EntityChangedNotification<T> notification)
        {
            _logger.LogDebug($"New {typeof(T).Name} change notification: {notification.GetType().Name}");
            switch (notification)
            {
                case AddEntity<T> addEntity:
                    Rules.Value.Add(addEntity.NewEntity);
                    break;
                case RemoveEntity<T> removeEntity:
                    var existing = Rules.Value.Single(x => x.Id.Equals(removeEntity.Id));
                    Rules.Value.Remove(existing);
                    break;
                case UpdateEntity<T> updateEntity:
                    var updatedIndex = Rules.Value.FindIndex(r => r.Id.Equals(updateEntity.UpdatedEntity.Id));
                    Rules.Value.Insert(updatedIndex, updateEntity.UpdatedEntity);
                    break;
            }
        }
    }
}