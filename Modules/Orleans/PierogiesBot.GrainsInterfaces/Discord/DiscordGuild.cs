using System;
using Orleans.Concurrency;

namespace PierogiesBot.GrainsInterfaces.Discord
{
    [Serializable]
    [Immutable]
    public class DiscordGuild
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
    }
}