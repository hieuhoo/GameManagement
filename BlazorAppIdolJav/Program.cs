using BlazorAppIdolJav.Components;
using BlazorAppIdolJav.Data;
using BlazorAppIdolJav.Repository;
using BlazorAppIdolJav.Repository.IRepository;
using BlazorAppIdolJav.Service;
using BlazorAppIdolJav.Service.IService;
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
builder.Services.AddScoped<IActressService, ActressService>();
builder.Services.AddScoped<IUserService, UserService>();
//cấu hình repo vào đây
builder.Services.AddScoped<IActressRepository, ActressRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

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
