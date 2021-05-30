
using MediatR;

namespace PierogiesBot.Data.Models.EntityChanged
{
    public abstract record EntityChangedNotification<T> : INotification where T : EntityBase;

    public record AddEntity<T>(T NewEntity) : EntityChangedNotification<T> where T : EntityBase;
    
    public record UpdateEntity<T>(T UpdatedEntity) : EntityChangedNotification<T> where T : EntityBase;
    
    public record RemoveEntity<T>(string Id) : EntityChangedNotification<T> where T : EntityBase;
}