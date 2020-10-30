using System;
using Module.Data.Models;

namespace Persistence.Models
{
    public class SettingEntity : EntityBase<Guid>
    {

        public SettingEntity()
        {
            Id = Guid.NewGuid();
            Key = "";
            Value = "";
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}