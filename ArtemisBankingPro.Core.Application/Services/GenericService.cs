using AutoMapper;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Interfaces;

namespace ArtemisBankingPro.Core.Application.Services
{
    public class GenericService<TEntity> : IGenericService <TEntity> where TEntity : class
    {
        protected readonly IGenericRepository<TEntity> _repository;
        protected readonly IMapper _mapper;

        public GenericService(IGenericRepository<TEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public virtual async Task<IEnumerable<TDto>> GetAllAsync<TDto>() where TDto : class
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<TDto>>(entities);
        }

        public virtual async Task<TDto?> GetByIdAsync<TDto>(int id) where TDto : class
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return _mapper.Map<TDto>(entity);
        }

        public virtual async Task<(bool Success, string Message, int? Id)> CreateAsync<TCreateDto>(TCreateDto dto) where TCreateDto : class
        {
            var entity = _mapper.Map<TEntity>(dto);
            var result = await _repository.AddAsync(entity);

            var idProperty = typeof(TEntity).GetProperty("Id");
            int? id = idProperty != null ? (int?)idProperty.GetValue(result) : null;

            return (true, "Registro creado exitosamente.", id);
        }

        public virtual async Task<(bool Success, string Message)> UpdateAsync<TUpdateDto>(int id, TUpdateDto dto) where TUpdateDto : class
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return (false, "Registro no encontrado.");

            _mapper.Map(dto, entity);
            await _repository.UpdateAsync(entity);
            return (true, "Registro actualizado exitosamente.");
        }

        public virtual async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return (false, "Registro no encontrado.");

            await _repository.DeleteAsync(entity);
            return (true, "Registro eliminado exitosamente.");
        }
    }
}