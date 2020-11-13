namespace Module.Data.Models
{
    public class CreateQuestionEntityDto
    {
        public string QuestionContent { get; set; } = "";
        public bool IsOpen { get; set; } = false;
        public bool IsMultipleChoice { get; set; } = false;
    }
}