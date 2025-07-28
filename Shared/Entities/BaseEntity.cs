using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace Shared.Models
{
    public abstract class BaseEntity : BaseEntity<Guid>
    {
        protected BaseEntity() => Id = Guid.NewGuid();
    }

    public abstract class BaseEntity<TId> : IBaseEntity<TId>
    {
        public TId Id { get; set; } = default!;

        [NotMapped]
        public List<DomainEvent> DomainEvents { get; } = new();

        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsDeleted { get; set; }

        protected BaseEntity()
        {
            CreatedOn = DateTime.UtcNow;
            IsActive = true;
        }
    }

    public interface IEntity
    {
        List<DomainEvent> DomainEvents { get; }
    }

    public interface IEntity<TId> : IEntity
    {
        TId Id { get; }
    }

    public abstract class DomainEvent : IEvent
    {
        public DateTime TriggeredOn { get; protected set; } = DateTime.UtcNow;
    }

    public interface IEvent
    {
    }

    public interface IBaseEntity<TId> : IEntity<TId>
    {
        Guid CreatedBy { get; set; }
        DateTime CreatedOn { get; set; }
        bool IsActive { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
