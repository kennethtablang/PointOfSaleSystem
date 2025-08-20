using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.Interfaces.Auth;
using PointOfSaleSystem.Interfaces.Inventory;
using PointOfSaleSystem.Interfaces.Sales;
using PointOfSaleSystem.Interfaces.Settings;
using PointOfSaleSystem.Interfaces.Supplier;
using PointOfSaleSystem.Models.Auth;
using PointOfSaleSystem.Services.Auth;
using PointOfSaleSystem.Services.Inventory;
using PointOfSaleSystem.Services.Sales;
using PointOfSaleSystem.Services.Settings;
using PointOfSaleSystem.Services.Supplier;
using System.Text;

namespace PointOfSaleSystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ---------- Services Configuration ----------

            builder.Logging.AddConsole();

            // Database Context
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        builder.Configuration["Jwt:Key"] ?? throw new Exception("JWT Key missing")))
                };

                // Add this to log errors when token fails
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"[JWT Error] Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    }
                };
            });

            // AutoMapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Application Services
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAuthLogService, AuthLogService>();
            builder.Services.AddScoped<ISystemLogService, SystemLogService>();
            builder.Services.AddScoped<IUserSessionService, UserSessionService>();

            //Sales
            builder.Services.AddScoped<IDiscountService, DiscountService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IReceiptLogService, ReceiptLogService>();
            builder.Services.AddScoped<IReturnTransactionService, ReturnTransactionService>();
            builder.Services.AddScoped<IReturnedItemService, ReturnedItemService>();
            builder.Services.AddScoped<IReceiptLogService, ReceiptLogService>();
            builder.Services.AddScoped<ISaleService, SaleService>();
            builder.Services.AddScoped<ISaleItemService, SaleItemService>();
            builder.Services.AddScoped<ISaleAuditTrailService, SaleAuditTrailService>();
            builder.Services.AddScoped<IVoidTransactionService, VoidTransactionService>();

            //Settings
            builder.Services.AddScoped<IBusinessProfileService, BusinessProfileService>();
            builder.Services.AddScoped<IVatSettingService, VatSettingService>();
            builder.Services.AddScoped<IDiscountSettingService, DiscountSettingService>();
            builder.Services.AddScoped<ICounterService, CounterService>();
            builder.Services.AddScoped<IReceiptSettingService, ReceiptSettingService>();

            // Inventory Services
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IUnitService, UnitService>();
            builder.Services.AddScoped<IProductUnitConversionService, ProductUnitConversionService>();
            builder.Services.AddScoped<IInventoryTransactionService, InventoryTransactionService>();
            builder.Services.AddScoped<IBadOrderService, BadOrderService>();
            builder.Services.AddScoped<IStockAdjustmentService, StockAdjustmentService>();
            builder.Services.AddScoped<IStockReceiveService, StockReceiveService>();

            //Supplier Services
            builder.Services.AddScoped<ISupplierService, SupplierService>();
            builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
            builder.Services.AddScoped<IStockAdjustmentService, StockAdjustmentService>();

            // Swagger / OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "POS System API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new string[] {}
                    }
                });
            });

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("FrontendPolicy", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            builder.Services.AddControllers();

            var app = builder.Build();

            // ---------- Middleware & Seeding ----------

            // Seed Identity Roles + Admin
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    await DbInitializer.SeedRolesAndAdminAsync(services);
                    Console.WriteLine("Identity roles and admin seeded successfully.");

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Seed Error] {ex.Message}");
                }
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("FrontendPolicy");

            app.UseAuthentication(); // Must come before UseAuthorization
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
