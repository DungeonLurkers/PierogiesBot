using System.ComponentModel.DataAnnotations;

namespace PierogiesBot.Settings
{
    public class DatabaseSettings
    {
        [DataType(DataType.Url, ErrorMessage = "ConnectionString is not a valid url!")]
        public string ConnectionString { get; set; }
    }
}