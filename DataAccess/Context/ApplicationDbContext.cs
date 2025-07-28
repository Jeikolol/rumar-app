using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Application.Common.Interfaces;
using Shared.Application.Common.Models;
using Shared.Entities.Identity;
using Shared.Infrastructure;
using Shared.Infrastructure.Auth;
using Shared.Infrastructure.Helpers;
using Shared.Models;
using System.Data;
using System.Reflection;

namespace DataAccess.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, 
                        ApplicationRole, string,
                        IdentityUserClaim<string>, IdentityUserRole<string>,
                        IdentityUserLogin<string>, ApplicationRoleClaim,
                        IdentityUserToken<string>>
    {
        private readonly DatabaseSettings _dbSettings;
        public ICurrentUser CurrentUser;
        public IDbConnection Connection => Database.GetDbConnection();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IOptions<DatabaseSettings> dbSettings,
        ICurrentUser currentUser
            )
            : base(options)
        {
            _dbSettings = dbSettings.Value;
            CurrentUser = currentUser;
        }

        /// <summary>
        /// Access table entity with no tracking for fast performance.
        /// </summary>
        /// <typeparam name="T">A class of Domain Entity.</typeparam>
        /// <returns>IQuerable.</returns>
        public IQueryable<T> Table<T>()
            where T : class, IEntity
        {
            return Set<T>()
                .AsNoTracking();
        }

        /// <summary>
        /// Access table entity with tracking for row update.
        /// </summary>
        /// <typeparam name="T">A class of Domain Entity.</typeparam>
        /// <returns>IQuerable.</returns>
        public IQueryable<T> TableForUpdate<T>()
            where T : class, IEntity
        {
            return Set<T>()
                .AsTracking();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();

            optionsBuilder.UseDatabase(_dbSettings.DBProvider, _dbSettings.ConnectionString);
        }

        public async Task<bool> EntityHasDependencies(string schema, string tableName, Guid id)
        {
            const string sqlProcedure = @"EXEC Usp_Tables_Dependencies @table_schema, @table_name, @id, @hasResults OUTPUT
                                      SELECT @hasResults;";

            var sqlData = new
            {
                @table_schema = schema,
                @table_name = tableName,
                id,
                @hasResults = false
            };

            return (await Connection.QueryAsync<bool>(sqlProcedure, sqlData))
                .FirstOrDefault();
        }

        public async Task<long> GenerateNextAtomicSequenceAsync<TEntity>()
       where TEntity : IEntity, IHaveAtomicSequence
        {
            var nameEntity = PluralizerHelper.Pluralize<TEntity>();

            string sqlProcedure = $@"
                DECLARE @tmpSequence TABLE(NewSequence BIGINT);
		        IF EXISTS (SELECT * FROM [Sequence].{nameEntity})
		        BEGIN
			        UPDATE sequence.{nameEntity}
			        SET Sequence = Sequence + 1
			        OUTPUT INSERTED.Sequence INTO @tmpSequence;
			        SELECT NewSequence FROM @tmpSequence;
		        END
		        ELSE
		        BEGIN
			        INSERT INTO Sequence.{nameEntity} (Sequence)
			        OUTPUT INSERTED.Sequence INTO @tmpSequence
			        VALUES (1)

			    SELECT NewSequence FROM @tmpSequence;
		        END;
            ";

            long sequence = (await Connection.QueryAsync<long>(sqlProcedure))
                .FirstOrDefault();

            return sequence;
        }

        public async Task<long> GenerateNextAtomicSequenceAsync<TEntity>(Guid accountingTransactionTypeId)
            where TEntity : IEntity, IHaveAtomicSequence
        {
            var tableNamePluralized = PluralizerHelper.Pluralize<TEntity>();

            string sqlProcedure = $@"
                DECLARE @tmpSequence TABLE(NewSequence BIGINT);
		        IF EXISTS (SELECT * FROM [Sequence].{tableNamePluralized} WHERE AccountingTransactionTypeId = '{accountingTransactionTypeId}')
		        BEGIN
                    UPDATE sequence.{tableNamePluralized}
                    SET Sequence = Sequence + 1
                    OUTPUT INSERTED.Sequence INTO @tmpSequence (NewSequence)
                    WHERE AccountingTransactionTypeId = '{accountingTransactionTypeId}'
                    SELECT NewSequence FROM @tmpSequence;
		        END
		        ELSE
		        BEGIN
			        INSERT INTO Sequence.{tableNamePluralized} (Sequence, AccountingTransactionTypeId)
			        OUTPUT INSERTED.Sequence INTO @tmpSequence
			        VALUES (1, '{accountingTransactionTypeId}')

			    SELECT NewSequence FROM @tmpSequence;
		        END;
            ";

            return (await Connection.QueryAsync<long>(sqlProcedure)).FirstOrDefault();
        }

        public async Task<IEnumerable<T>> Filter<T>(decimal amount, LogicOperator opt, string schema, string columnToFilter)
         where T : class, IEntity
        {
            string nameEntity = PluralizerHelper.Pluralize<T>();
            string logicOperator = FilterHelper.GetLogicOpt(opt);
            string sqlCommand = $@"
            SELECT *
            FROM [{schema}].[{nameEntity}]
            WHERE {columnToFilter} {logicOperator} @Amount
            ";

            return await Connection.QueryAsync<T>(sqlCommand, new { Amount = amount });
        }
    }
}
