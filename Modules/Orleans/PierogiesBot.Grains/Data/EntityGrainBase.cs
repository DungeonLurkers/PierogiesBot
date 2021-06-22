using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Orleans;
using PierogiesBot.Commons.Dtos;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;
using PierogiesBot.GrainsInterfaces.Data;

namespace PierogiesBot.Grains.Data
{
    public abstract class EntityGrainBase<TEntity> : Grain, IEntityGrain where TEntity : EntityBase
    {
        protected IRepository<TEntity> Repository { get; }
        protected IMapper Mapper { get; }

        protected EntityGrainBase(IRepository<TEntity> repository, IMapper mapper)
        {
            Repository = repository;
            Mapper = mapper;
        }

        protected async Task<TDto?> FindById<TDto>(string id) where TDto : IFindEntityDto
        {
            var rule = await FindById(id);
            
            return rule is not null ? Mapper.Map<TDto>(rule) : default;
        }
        
        protected async Task<TEntity?> FindById(string id) => await Repository.GetByIdAsync(id);

        public async Task<IEnumerable<TDto>> Find<TDto>() where TDto : IFindEntityDto
        {
            var rules = await Repository.GetAll();
            
            return rules.Select(x => Mapper.Map<TDto>(x));
        }

        protected async Task<IEnumerable<TEntity>> Find() => await Repository.GetAll(); 

        protected async Task<string> Create(ICreateEntityDto ruleDto)
        {
            var rule = Mapper.Map<TEntity>(ruleDto);
            return await Repository.InsertAsync(rule);
        }

        protected async Task<string> Update(string id, IUpdateEntityDto ruleDto)
        {
            var rule = await Repository.GetByIdAsync(id);
            
            switch (rule)
            {
                case null:
                    return "";
                default:
                {
                    var updatedRule = Mapper.Map<TEntity>(ruleDto);

                    await Repository.UpdateAsync(updatedRule with {Id = id});

                    return id;
                }
            }
        }

        public async Task<string> Delete(string id)
        {
            var rule = await Repository.GetByIdAsync(id);

            switch (rule)
            {
                case null:
                    return "";
                default:
                {
                    await Repository.DeleteAsync(id);

                    return id;
                }
            }
        }
    }
}