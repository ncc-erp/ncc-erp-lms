using Abp.Application.Services.Dto;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.ObjectMapping;
using Abp.UI;
using RMALMS.Anotations;
using RMALMS.Authorization.Users;
using RMALMS.IoC;
using RMALMS.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RMALMS.Extension;
using System.Collections;

namespace RMALMS.DomainServices
{
    public class BaseService : DomainService, IBaseService
    {
        protected readonly IWorkScope _workScope;
        protected readonly IObjectMapper _objectMapper;
        public BaseService(IWorkScope workScope, IObjectMapper objectMapper)
        {
            this._workScope = workScope;
            this._objectMapper = objectMapper;

        }

        public IWorkScope GetWorkScope()
        {
            return _workScope;
        }

        async Task<EntityDTO> IBaseService.Create<EntityDTO, TEntity, TPrimaryKey>(EntityDTO entityDTO)
        {
            var repository = _workScope.GetRepo<TEntity, TPrimaryKey>();
            var entity = _objectMapper.Map<TEntity>(entityDTO);
            entity.Id = await repository.InsertAndGetIdAsync(entity);

            entityDTO.Id = entity.Id;

            return entityDTO;
        }

        async Task<EntityDTO> IBaseService.CreateWithChildren<EntityDTO, TEntity, TPrimaryKey>(EntityDTO entityDTO)
        {
            var repository = _workScope.GetRepo<TEntity, TPrimaryKey>();
            var entity = _objectMapper.Map<TEntity>(entityDTO);
            entityDTO.Id = await repository.InsertAndGetIdAsync(entity);

            var propertiesInfo = typeof(EntityDTO).GetAllProperties();
            foreach (var pro in propertiesInfo)
            {
                var proType = pro.Type();
                var mapToAttribute = pro.GetCustomAttributes(typeof(MappingToAttribute), false).FirstOrDefault();
                if (mapToAttribute != null)
                {
                    if (proType.IsGenericType)
                    {
                        Type[] typeParameters = proType.GetGenericArguments();
                        if (typeParameters.Length > 0)
                        {
                            var type = typeParameters[0];
                            // just do if subclass for EntityDto
                            if (type.IsSubclassOf(typeof(EntityDto<TPrimaryKey>)))
                            {
                                var list = (IEnumerable)pro.GetValue(entityDTO);
                                foreach (var i in list)
                                {
                                    CreateEntity<TPrimaryKey, EntityDTO>(type,
                                        i,
                                        mapToAttribute as MappingToAttribute,
                                        entityDTO.Id
                                    );
                                }
                            }
                        }
                    }
                    else if (proType.IsSubclassOf(typeof(EntityDto<TPrimaryKey>)))
                    {
                        // TODO make here
                        CreateEntity<TPrimaryKey, EntityDTO>(proType,
                            pro.GetValue(entityDTO),
                            mapToAttribute as MappingToAttribute, 
                            entityDTO.Id
                        );
                    }
                }
            }
            return entityDTO;
        }

        private void CreateEntity<TPrimaryKey, EntityDTO>(Type type, object o, MappingToAttribute mappingToAttribute, TPrimaryKey parentId)
        {
            Type toType = mappingToAttribute.MapTo;
            var typeProperties = type.GetAllProperties();
            foreach (var p in typeProperties)
            {
                var attribut = p.GetCustomAttribute<ParentAttrbute>();
                // set parent Id
                if (attribut != null && attribut.ParentType == typeof(EntityDTO))
                {
                    p.SetValue(o, parentId);
                    break;
                }
            }

            var mapperType = _objectMapper.GetType();
            MethodInfo method = mapperType.GetMethod("Map", new[] { typeof(object) });
            MethodInfo generic = method.MakeGenericMethod(toType);
            var entity = generic.Invoke(_objectMapper, new object[] { o });
            var repoType = typeof(IRepository<,>);
            Type[] typeArgs = { toType, typeof(TPrimaryKey) };
            var repoGenericType = repoType.MakeGenericType(typeArgs);
            var resolveMethod = IocManager.Instance.GetType().GetMethods()
                .Where(s => s.Name == "Resolve" && !s.IsGenericMethod && s.GetParameters().Length == 1 && s.GetParameters()[0].ParameterType == typeof(Type))
                .FirstOrDefault();
            //var resolveMethod = IocManager.Instance.GetType().GetMethod("Resolve", new [] { typeof(Type)});
            var repo = resolveMethod.Invoke(IocManager.Instance, new Type[] { repoGenericType });

            //IocManager.Instance.Resolve()

            ///set new Id here
            var insertAndGetIdMethod = repoGenericType.GetMethod("InsertAndGetIdAsync");
            var newId = insertAndGetIdMethod.Invoke(repo, new object[] { entity });
            var idProperty = type.GetPropertyEx("Id");
            if (idProperty != null)
            {
                idProperty.SetValue(o, newId);
            }
        }

        async Task IBaseService.Delete<TEntity, TPrimaryKey>(TPrimaryKey id)
        {
            var repository = _workScope.GetRepo<TEntity, TPrimaryKey>();
            await repository.DeleteAsync(id);
        }

        async Task<EntityDTO> IBaseService.Get<EntityDTO, TEntity, TPrimaryKey>(TPrimaryKey id)
        {
            var repository = _workScope.GetRepo<TEntity, TPrimaryKey>();
            var entity = await repository.GetAsync(id);
            if (entity == null)
            {
                throw new UserFriendlyException("The item doesnot exist");
            }
            return _objectMapper.Map<EntityDTO>(entity);
        }

        async Task<ListResultDto<EntityDTO>> IBaseService.GetAll<EntityDTO, TEntity, TPrimaryKey>()
        {
            var repository = _workScope.GetRepo<TEntity, TPrimaryKey>();
            var list = await repository.GetAllListAsync();
            var result = _objectMapper.Map<List<EntityDTO>>(list);
            return new ListResultDto<EntityDTO>
            {
                Items = result
            };
        }

        IRepository<TEntity, TPrimaryKey> IBaseService.GetRepository<TEntity, TPrimaryKey>()
        {
            return _workScope.GetRepo<TEntity, TPrimaryKey>();
        }

        Task<GridResult<EntityDTO>> IBaseService.Search<EntityDTO, TEntity, TPrimaryKey>(GridParam gridParam)
        {
            throw new NotImplementedException();
        }

        async Task<EntityDTO> IBaseService.Update<EntityDTO, TEntity, TPrimaryKey>(EntityDTO enityDTO)
        {
            var repository = _workScope.GetRepo<TEntity, TPrimaryKey>();
            var entity = repository.FirstOrDefault(enityDTO.Id);
            if (entity == null)
            {
                throw new UserFriendlyException("The record is already deleted");
            }

            _objectMapper.Map(enityDTO, entity);
            entity = await repository.UpdateAsync(entity);

            return enityDTO;
        }

        T IBaseService.GetService<T>()
        {
            //var repoType = typeof(IRepository<,>);
            //var resolveMethod = IocManager.Instance.GetType().GetMethods()
            //    .Where(s => s.Name == "Resolve" && !s.IsGenericMethod && s.GetParameters().Length == 1 && s.GetParameters()[0].ParameterType == typeof(Type))
            //    .FirstOrDefault();
            //var repo = resolveMethod.Invoke(IocManager.Instance, new Type[] { typeof(T) });
            //return repo as T;

            // Maybe use
            return IocManager.Instance.Resolve<T>();
        }
    }
}
