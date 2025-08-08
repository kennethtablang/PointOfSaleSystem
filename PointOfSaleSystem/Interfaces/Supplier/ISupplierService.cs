
using PointOfSaleSystem.DTOs.Suppliers;

namespace PointOfSaleSystem.Interfaces.Supplier
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierReadDto>> GetAllAsync();
        Task<SupplierReadDto?> GetByIdAsync(int id);
        Task<SupplierReadDto> AddAsync(SupplierCreateDto supplier);
        Task<bool> UpdateAsync(int id, SupplierUpdateDto supplier);
        Task<bool> DeleteAsync(int id);
    }
}
