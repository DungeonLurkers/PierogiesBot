using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace PierogiesBot.Discord.TypeReaders
{
    public class TimeZoneInfoTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            try
            {
                var tzInfo = TimeZoneInfo.FindSystemTimeZoneById(input);
                return Task.FromResult(TypeReaderResult.FromSuccess(tzInfo));
            }
            catch (Exception e)
            {
                return Task.FromResult(TypeReaderResult.FromError(e));
            }
        }
    }
}