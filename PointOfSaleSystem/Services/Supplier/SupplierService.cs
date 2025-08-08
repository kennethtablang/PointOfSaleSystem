using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Suppliers;
using PointOfSaleSystem.Interfaces.Supplier;

namespace PointOfSaleSystem.Services.Supplier
{
    public class SupplierService : ISupplierService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SupplierService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SupplierReadDto>> GetAllAsync()
        {
            var suppliers = await _context.Suppliers.AsNoTracking().ToListAsync();
            return _mapper.Map<IEnumerable<SupplierReadDto>>(suppliers);
        }

        public async Task<SupplierReadDto?> GetByIdAsync(int id)
        {
            var supplier = await _context.Suppliers.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
            return _mapper.Map<SupplierReadDto?>(supplier);
        }

        public async Task<SupplierReadDto> AddAsync(SupplierCreateDto dto)
        {
            var supplier = _mapper.Map<Models.Suppliers.Supplier>(dto);
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
            return _mapper.Map<SupplierReadDto>(supplier);
        }

        public async Task<bool> UpdateAsync(int id, SupplierUpdateDto dto)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return false;

            _mapper.Map(dto, supplier);
            _context.Entry(supplier).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return false;

            _context.Suppliers.Remove(supplier);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
