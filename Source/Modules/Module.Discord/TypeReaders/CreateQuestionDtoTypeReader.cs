using System;
using System.Threading.Tasks;
using Discord.Commands;
using Module.Data.Models;

namespace Module.Discord.TypeReaders
{
    public class CreateQuestionDtoTypeReader : TypeReader
    {
        public override  Task<TypeReaderResult> ReadAsync(ICommandContext context, [Remainder] string input, IServiceProvider services)
        {
            
            try
            {
                var args = input.Split(';', StringSplitOptions.RemoveEmptyEntries);

                CreateQuestionEntityDto question;
                if (!input.Contains(';'))
                {
                    var questionContent = input;
                    
                    question = new CreateQuestionEntityDto
                    {
                        QuestionContent = questionContent
                    };
                } 
                else if (args.Length < 3)
                {
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Wrong arguments!"));
                }
                else
                {
                    var questionContent = args[0];
                    var isOpen = bool.Parse(args[1]);
                    var isMultiChoice = bool.Parse(args[2]);

                    question = new CreateQuestionEntityDto
                    {
                        QuestionContent = questionContent ?? "",
                        IsOpen = isOpen,
                        IsMultipleChoice = isMultiChoice
                    };
                }

                return Task.FromResult(TypeReaderResult.FromSuccess(question));
            }
            catch (Exception e)
            {
                return Task.FromResult(TypeReaderResult.FromError(e));
            }
        }
    }
}