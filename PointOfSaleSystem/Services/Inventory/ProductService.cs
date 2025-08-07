using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Inventory;
using PointOfSaleSystem.Interfaces.Inventory;
using PointOfSaleSystem.Models.Inventory;

namespace PointOfSaleSystem.Services.Inventory
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductViewDto>> GetAllAsync()
        {
            var products = await _context.Products
                .Where(p => p.IsActive)
                .Include(p => p.Category)
                .Include(p => p.Unit)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductViewDto>>(products);
        }

        public async Task<ProductViewDto?> GetByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Unit)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (product == null) return null;
            return _mapper.Map<ProductViewDto>(product);
        }

        public async Task<ProductViewDto> CreateAsync(ProductCreateDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            // Map Base64 image
            if (!string.IsNullOrEmpty(dto.ImageBase64))
            {
                product.ImageData = Convert.FromBase64String(dto.ImageBase64);
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Load navigation for mapping
            await _context.Entry(product).Reference(p => p.Category).LoadAsync();
            await _context.Entry(product).Reference(p => p.Unit).LoadAsync();

            return _mapper.Map<ProductViewDto>(product);
        }

        public async Task<bool> UpdateAsync(ProductUpdateDto dto)
        {
            var product = await _context.Products.FindAsync(dto.Id);
            if (product == null) return false;

            // Map updated fields
            _mapper.Map(dto, product);

            // Handle image update
            if (dto.ImageBase64 != null)
            {
                product.ImageData = Convert.FromBase64String(dto.ImageBase64);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            product.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
