using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using RMALMS.IoC;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS
{
    public class ApplicationBaseService : ApplicationService
    {
        protected readonly IWorkScope _ws;
        public ApplicationBaseService()
        {
            _ws = IocManager.Instance.Resolve<IWorkScope>();
        }
    }

    public class CrudApplicationBaseService<TEntity, TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput> : AsyncCrudAppService<TEntity, TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput>
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
    {
        public readonly IWorkScope WorkScope;
        public CrudApplicationBaseService(IRepository<TEntity, TPrimaryKey> repository) 
            : base(repository)
        {
            WorkScope = IocManager.Instance.Resolve<IWorkScope>();
        }
        
    }
}
