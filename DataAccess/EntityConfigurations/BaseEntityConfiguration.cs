using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Models;
using System.Security.Principal;

namespace DataAccess.EntityConfigurations
{
    public abstract class BaseEntityConfiguration<TEntity, T> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity<T>, IEntity
    {
        private readonly string? _schema;
        private readonly string _tableName;

        protected BaseEntityConfiguration(string? schema, string? tableName = null)
        {
            _schema = schema;
            _tableName = string.IsNullOrEmpty(tableName) ? typeof(TEntity).Name.Pluralize() : tableName;
        }

        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            _ = _schema ?? throw new ArgumentNullException($"schema is null for entity [{typeof(TEntity).Name}]");

            builder
                .ToTable(_tableName, _schema);
        }
    }

}
