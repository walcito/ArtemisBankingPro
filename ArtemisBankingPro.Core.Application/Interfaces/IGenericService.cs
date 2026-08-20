namespace ArtemisBankingPro.Core.Application.Interfaces
{
    public interface IGenericService<T> where T : class
    {
        Task<IEnumerable<TDto>> GetAllAsync<TDto>() where TDto : class;
        Task<TDto?> GetByIdAsync<TDto>(int id) where TDto : class;
        Task<(bool Success, string Message, int? Id)> CreateAsync<TCreateDto>(TCreateDto dto) where TCreateDto : class;
        Task<(bool Success, string Message)> UpdateAsync<TUpdateDto>(int id, TUpdateDto dto) where TUpdateDto : class;
        Task<(bool Success, string Message)> DeleteAsync(int id);
    }
}