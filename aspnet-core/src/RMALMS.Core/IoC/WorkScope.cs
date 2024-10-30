using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Abp;
using Abp.Application.Services.Dto;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;

namespace RMALMS.IoC
{
    public class WorkScope : AbpServiceBase, IWorkScope
    {
        async Task<Guid> IWorkScope.InsertAndGetIdAsync<TEntity>(TEntity entity)
        {
            return await (this as IWorkScope).InsertAndGetIdAsync<TEntity, Guid>(entity);
        }

        async Task<TPrimaryKey> IWorkScope.InsertAndGetIdAsync<TEntity, TPrimaryKey>(TEntity entity)
        {
            var repo = (this as IWorkScope).GetRepo<TEntity, TPrimaryKey>();
            UpdateTenantId<TEntity, TPrimaryKey>(entity);
            return await repo.InsertAndGetIdAsync(entity);
        }

        async Task<TEntity> IWorkScope.InsertAsync<TEntity>(TEntity entity)
        {
            return await (this as IWorkScope).InsertAsync<TEntity, Guid>(entity);
        }

        async Task<TEntity> IWorkScope.InsertAsync<TEntity, TPrimaryKey>(TEntity entity)
        {
            var repo = (this as IWorkScope).GetRepo<TEntity, TPrimaryKey>();
            UpdateTenantId<TEntity, TPrimaryKey>(entity);
            return await repo.InsertAsync(entity);
        }

        IQueryable<TEntity> IWorkScope.GetAll<TEntity, TPrimaryKey>()
        {
            return (this as IWorkScope).GetRepo<TEntity, TPrimaryKey>().GetAll();
        }

        IQueryable<TEntity> IWorkScope.GetAll<TEntity>()
        {
            return (this as IWorkScope).GetAll<TEntity, Guid>();
        }

        IRepository<TEntity, TPrimaryKey> IWorkScope.GetRepo<TEntity, TPrimaryKey>()
        {
            var repoType = typeof(IRepository<,>);
            Type[] typeArgs = { typeof(TEntity), typeof(TPrimaryKey) };
            var repoGenericType = repoType.MakeGenericType(typeArgs);
            var resolveMethod = IocManager.Instance.GetType().GetMethods()
                .Where(s => s.Name == "Resolve" && !s.IsGenericMethod && s.GetParameters().Length == 1 && s.GetParameters()[0].ParameterType == typeof(Type))
                .FirstOrDefault();
            var repo = resolveMethod.Invoke(IocManager.Instance, new Type[] { repoGenericType });
            return repo as IRepository<TEntity, TPrimaryKey>;
        }

        IRepository<TEntity, Guid> IWorkScope.GetRepo<TEntity>()
        {
            return (this as IWorkScope).GetRepo<TEntity, Guid>();
        }

        private void UpdateTenantId<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            var tenantEntity = entity as IMayHaveTenant;
            if (tenantEntity != null)
                tenantEntity.TenantId = CurrentUnitOfWork.GetTenantId();
        }

        public async Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<Guid>
        {
            await (this as IWorkScope).GetRepo<TEntity, Guid>().DeleteAsync(entity);
        }

        public async Task DeleteAsync<TEntity>(Guid id) where TEntity : class, IEntity<Guid>
        {
            await (this as IWorkScope).GetRepo<TEntity, Guid>().DeleteAsync(id);
        }

        public async Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IEntity<Guid>
        {
            await (this as IWorkScope).GetRepo<TEntity, Guid>().DeleteAsync(predicate);
        }

        public async Task<List<TEntityDto>> InsertUpdateAndDelete<TEntity, TEntityDto, TPrimaryKey>(List<TEntityDto> entitiesDto, IQueryable<TPrimaryKey> existIds, bool includeUpdate = true)
            where TEntity : class, IEntity<TPrimaryKey>
            where TEntityDto : class, IEntityDto<TPrimaryKey>
        {
            var repo = (this as IWorkScope).GetRepo<TEntity, TPrimaryKey>();

            var insertItemDtos = entitiesDto.Where(s => s.Id.Equals(default(TPrimaryKey)));
            
            // update all tenant here
            foreach (var itemDto in insertItemDtos)
            {
                var item = ObjectMapper.Map<TEntity>(itemDto);
                UpdateTenantId<TEntity, TPrimaryKey>(item);
                item.Id = await repo.InsertAndGetIdAsync(item);
                itemDto.Id = item.Id;
            }

            var entityIds = entitiesDto.Select(s => s.Id);

            // update item
            if (includeUpdate)
            {
                var updateItemDtos = entitiesDto.Where(s => existIds.Contains(s.Id)).ToLookup(s => s.Id, s => s);
                var updateIds = updateItemDtos.Select(s => s.Key);
                var updateItems = repo.GetAll().Where(s => updateIds.Contains(s.Id));
                foreach (var item in updateItems)
                {
                    var itemDto = updateItemDtos[item.Id].FirstOrDefault();
                    ObjectMapper.Map(itemDto, item);
                    await repo.UpdateAsync(item);
                }
            }

            // delete item
            var deletedIds = existIds.Where(s => !entityIds.Contains(s)).ToList();
            await repo.DeleteAsync(s => deletedIds.Contains(s.Id));
            return entitiesDto;
        }

        async Task<TEntity> IWorkScope.UpdateAsync<TEntity>(TEntity entity)
        {
            return await (this as IWorkScope).UpdateAsync<TEntity, Guid>(entity);
        }

        async Task<TEntity> IWorkScope.UpdateAsync<TEntity, TPrimaryKey>(TEntity entity)
        {
            var repo = (this as IWorkScope).GetRepo<TEntity, TPrimaryKey>();
            UpdateTenantId<TEntity, TPrimaryKey>(entity);
            return await repo.UpdateAsync(entity);
        }

        async Task<IEnumerable<TEntity>> IWorkScope.UpdateRangeAsync<TEntity>(IEnumerable<TEntity> entities)
        {
            var updatedEntities = new List<TEntity>();
            foreach (var entity in entities)
            {
                updatedEntities.Add(await (this as IWorkScope).GetRepo<TEntity, Guid>().UpdateAsync(entity));
            }

            return updatedEntities;
        }
    }
}
