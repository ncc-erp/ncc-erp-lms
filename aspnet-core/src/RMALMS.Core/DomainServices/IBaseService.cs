using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using RMALMS.IoC;
using RMALMS.Paging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RMALMS.DomainServices
{
    public interface IBaseService : IDomainService
    {
        Task<GridResult<EntityDTO>> Search<EntityDTO, TEntity, TPrimaryKey>(GridParam gridParam)
            where TEntity : class, IEntity<TPrimaryKey>
            where EntityDTO: class;
        Task<EntityDTO> Create<EntityDTO, TEntity, TPrimaryKey>(EntityDTO enity)
            where TEntity : class, IEntity<TPrimaryKey>
            where EntityDTO : class, IEntityDto<TPrimaryKey>;
        Task<EntityDTO> Update<EntityDTO, TEntity, TPrimaryKey>(EntityDTO enity) 
            where TEntity : class, IEntity<TPrimaryKey>
            where EntityDTO : class, IEntityDto<TPrimaryKey>;
        Task Delete<TEntity, TPrimaryKey>(TPrimaryKey enity)
            where TEntity : class, IEntity<TPrimaryKey>;
        Task<ListResultDto<EntityDTO>> GetAll<EntityDTO, TEntity, TPrimaryKey>()
             where TEntity : class, IEntity<TPrimaryKey>
            where EntityDTO : class;
        Task<EntityDTO> Get<EntityDTO, TEntity, TPrimaryKey>(TPrimaryKey id)
            where TEntity : class, IEntity<TPrimaryKey>
            where EntityDTO : class;

        Task<EntityDTO> CreateWithChildren<EntityDTO, TEntity, TPrimaryKey>(EntityDTO enity)
            where TEntity : class, IEntity<TPrimaryKey>
            where EntityDTO : class, IEntityDto<TPrimaryKey>;

        IWorkScope GetWorkScope();
        IRepository<TEntity, TPrimaryKey> GetRepository<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>;

        T GetService<T>() where T: class;
    }
}
