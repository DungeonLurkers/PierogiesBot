using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Orleans;
using PierogiesBot.Commons.Dtos;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;

namespace PierogiesBot.Grains.Data
{
    public abstract class EntityGrainBase<TEntity> : Grain where TEntity : EntityBase
    {
        private readonly IRepository<TEntity> _repository;
        private readonly IMapper _mapper;

        protected EntityGrainBase(IRepository<TEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        protected async Task<TDto?> FindById<TDto>(string id) where TDto : IFindEntityDto
        {
            var rule = await _repository.GetByIdAsync(id);
            
            return rule is not null ? _mapper.Map<TDto>(rule) : default;
        }

        protected async Task<IEnumerable<TDto>> Find<TDto>() where TDto : IFindEntityDto
        {
            var rules = await _repository.GetAll();
            
            return rules.Select(x => _mapper.Map<TDto>(x));
        }

        protected async Task<string> Create(ICreateEntityDto ruleDto)
        {
            var rule = _mapper.Map<TEntity>(ruleDto);
            return await _repository.InsertAsync(rule);
        }

        protected async Task<string> Update(string id, IUpdateEntityDto ruleDto)
        {
            var rule = await _repository.GetByIdAsync(id);
            
            switch (rule)
            {
                case null:
                    return "";
                default:
                {
                    var updatedRule = _mapper.Map<TEntity>(ruleDto);

                    await _repository.UpdateAsync(updatedRule with {Id = id});

                    return id;
                }
            }
        }

        public async Task<string> Delete(string id)
        {
            var rule = await _repository.GetByIdAsync(id);

            switch (rule)
            {
                case null:
                    return "";
                default:
                {
                    await _repository.DeleteAsync(id);

                    return id;
                }
            }
        }
    }
}