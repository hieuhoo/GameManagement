using GameManagement.Auth;
using GameManagement.Components;
using GameManagement.Components.State;
using GameManagement.Data;
using GameManagement.Repository;
using GameManagement.Repository.IRepository;
using GameManagement.Service;
using GameManagement.Service.IService;
using GameManagement.Services;
using GameManagement.Share.Extension;
using GameManagement.SpecialComponent.ExtensionClass;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

// 👉 Thêm EF Core với connection string từ appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAntDesign();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//cấu hình service vào đây
var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
builder.Services.Configure<JwtOptions>(jwtSection);

var jwtOptions = jwtSection.Get<JwtOptions>()
    ?? throw new InvalidOperationException("Thiếu cấu hình Jwt trong appsettings.json.");

if (string.IsNullOrWhiteSpace(jwtOptions.Key) ||
    Encoding.UTF8.GetByteCount(jwtOptions.Key) < 32)
{
    throw new InvalidOperationException("Jwt:Key phải có ít nhất 32 bytes.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<ProtectedLocalStorage>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGameCompanyService, GameCompanyService>();
builder.Services.AddScoped<IGameTypeService, GameTypeService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserOtpHistoryService, UserOtpHistoryService>();
builder.Services.AddScoped<IUserPasswordHistoryService, UserPasswordHistoryService>();
builder.Services.AddScoped<IUserLockHistoryService, UserLockHistoryService>();
builder.Services.AddScoped<IGameDiscountService, GameDiscountService>();
builder.Services.AddScoped<IGameRedeemCodeService, GameRedeemCodeService>();
builder.Services.AddScoped<IUserWalletService, UserWalletService>();
builder.Services.AddScoped<IWalletTransactionHistoryService, WalletTransactionHistoryService>();
builder.Services.AddScoped<IUserGameWishlistService, UserGameWishlistService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<IUserGameLibraryService, UserGameLibraryService>();
builder.Services.AddScoped<IUserGameReviewService, UserGameReviewService>();
builder.Services.AddScoped<IUserGameReviewHistoryService, UserGameReviewHistoryService>();


//cấu hình repo vào đây
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGameCompanyRepository, GameCompanyRepository>();
builder.Services.AddScoped<IGameTypeRepository, GameTypeRepository>();
builder.Services.AddScoped<IUserOtpHistoryRepository, UserOtpHistoryRepository>();
builder.Services.AddScoped<IUserPasswordHistoryRepository, UserPasswordHistoryRepository>();
builder.Services.AddScoped<IUserLockHistoryRepository, UserLockHistoryRepository>();
builder.Services.AddScoped<IGameDiscountRepository, GameDiscountRepository>();
builder.Services.AddScoped<IGameRedeemCodeRepository, GameRedeemCodeRepository>();
builder.Services.AddScoped<IUserWalletRepository, UserWalletRepository>();
builder.Services.AddScoped<IWalletTransactionHistoryRepository, WalletTransactionHistoryRepository>();
builder.Services.AddScoped<IUserGameWishlistRepository, UserGameWishlistRepository>();
builder.Services.AddScoped<IUserGameLibraryRepository, UserGameLibraryRepository>();
builder.Services.AddScoped<IUserGameReviewRepository, UserGameReviewRepository>();


builder.Services.AddScoped<WishlistState>();
// cấu hình extension
GlobalVariant.UploadFolder = Path.Combine(
    builder.Environment.WebRootPath,
    "Upload"
);

if (!Directory.Exists(GlobalVariant.UploadFolder))
{
    Directory.CreateDirectory(GlobalVariant.UploadFolder);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
