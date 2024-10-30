using Abp.Application.Services.Dto;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RMALMS.IoC
{
    public interface IWorkScope : ITransientDependency
    {
        IRepository<TEntity, TPrimaryKey> GetRepo<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>;
        IQueryable<TEntity> GetAll<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>;
        IRepository<TEntity, Guid> GetRepo<TEntity>() where TEntity : class, IEntity<Guid>;
        IQueryable<TEntity> GetAll<TEntity>() where TEntity : class, IEntity<Guid>;

        Task<TEntity> InsertAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<Guid>;
        Task<Guid> InsertAndGetIdAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<Guid>;
        Task<TEntity> UpdateAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<Guid>;

        Task<TEntity> InsertAsync<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
        Task<TPrimaryKey> InsertAndGetIdAsync<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
        Task<TEntity> UpdateAsync<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
        Task<IEnumerable<TEntity>> UpdateRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity<Guid>;

        Task<List<TEntityDto>> InsertUpdateAndDelete<TEntity, TEntityDto, TPrimaryKey>(List<TEntityDto> entities, IQueryable<TPrimaryKey> existIds, bool includeUpdate = true) 
            where TEntity : class, IEntity<TPrimaryKey>
            where TEntityDto : class, IEntityDto<TPrimaryKey>;

        Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<Guid>;
        Task DeleteAsync<TEntity>(Guid id) where TEntity : class, IEntity<Guid>;
        Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IEntity<Guid>;
    }
}
