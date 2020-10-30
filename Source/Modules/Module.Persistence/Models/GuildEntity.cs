using System.Collections.Generic;
using System.Linq;
using Discord;
using Module.Data.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Persistence.Models
{
    public class GuildEntity : EntityBase<ulong>
    {
        [BsonElement] public ulong DiscordId { get; set; }
        [BsonElement] public ulong OwnerId { get; set; }
        [BsonElement] public string? IconUrl{ get; set; }
        [BsonElement] public string? IconId{ get; set; }
        [BsonElement] public ulong EveryoneRoleId{ get; set; }
        [BsonElement] public string? BannerUrl{ get; set; }
        [BsonElement] public string? BannerId{ get; set; }
        [BsonElement] public string? Name{ get; set; }
        [BsonElement] public IEnumerable<string>? Features{ get; set; }
        [BsonElement] public string? Description{ get; set; }
        [BsonElement] public bool Available{ get; set; }

        public GuildEntity(IGuild guild)
        {
            DiscordId = guild.Id;
            Available = guild.Available;
            Description = guild.Description;
            Features = guild.Features.ToList();
            Name = guild.Name;
            BannerId = guild.BannerId;
            BannerUrl = guild.BannerUrl;
            EveryoneRoleId = guild.EveryoneRole.Id;
            IconId = guild.IconId;
            IconUrl = guild.IconUrl;
            OwnerId = guild.OwnerId;
        }
        
        
    }
}