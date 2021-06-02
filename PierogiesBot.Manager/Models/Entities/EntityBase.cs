using System;

namespace PierogiesBot.Manager.Models.Entities
{
    public class EntityBase
    {
        public EntityBase(Guid Id)
        {
            this.Id = Id;
        }

        public Guid Id { get; init; }

        public void Deconstruct(out Guid Id)
        {
            Id = this.Id;
        }
    }
}