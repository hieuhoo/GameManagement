using GameManagement.Components;
using GameManagement.Data;
using GameManagement.Repository;
using GameManagement.Repository.IRepository;
using GameManagement.Service;
using GameManagement.Service.IService;
using GameManagement.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 👉 Thêm EF Core với connection string từ appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAntDesign();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//cấu hình service vào đây
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<ProtectedLocalStorage>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGameCompanyService, GameCompanyService>();
builder.Services.AddScoped<IGameTypeService, GameTypeService>();

//cấu hình repo vào đây
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGameCompanyRepository, GameCompanyRepository>();
builder.Services.AddScoped<IGameTypeRepository, GameTypeRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
