using System;
using System.Collections.Generic;
using Module.Data.Models;
using Persistence.Models;

namespace Persistence.Helpers
{
    public static class MongoHelper
    {
        private static Dictionary<Type, string> _collNamesToEntitiesMapping = new Dictionary<Type, string>
        {
            {typeof(RoleEntity), "Roles"},
            {typeof(GuildEntity), "Guilds"},
            {typeof(GuildUserEntity), "GuildUsers"},
            {typeof(SettingEntity), "Settings"},
            {typeof(BotResponseRule), "BotResponseRules"},
        };

        public static string GetCollNameFromEntity<TId>(EntityBase<TId> entity) => _collNamesToEntitiesMapping[entity.GetType()];

        public static string GetCollNameForEntityType(Type entityType) => _collNamesToEntitiesMapping[entityType];
    }
}