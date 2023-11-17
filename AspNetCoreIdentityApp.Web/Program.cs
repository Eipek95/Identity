using AspNetCoreIdentityApp.Core.OptionsModels;
using AspNetCoreIdentityApp.Core.PermissionsRoot;
using AspNetCoreIdentityApp.Repository.Models;
using AspNetCoreIdentityApp.Repository.Seeds;
using AspNetCoreIdentityApp.Web.ClaimProvider;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Requirements;
using AspNetCoreIdentityApp.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"), options =>
    {
        options.MigrationsAssembly("AspNetCoreIdentityApp.Repository");
    });
});
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.FromMinutes(30);//varsay�lan de�er
});

builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));//wwwroot veya projenin herhamgi bir yerinden bir klas�re eri�mek istersek bu yolla kolayl�kla ula�abiliriz 


builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddIdentityWithExt();//identity ayarlar�n�n yap�ld�g� class
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IClaimsTransformation, UserClaimProvider>();
builder.Services.AddScoped<IAuthorizationHandler, ExhangeExpireRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ViolonceRequirementHandler>();


builder.Services.AddAuthorization(policy =>
{
    policy.AddPolicy("AnkaraPolicy", policy =>
    {
        policy.RequireClaim("city", "ankara");
        //policy.RequireRole("admin");
        //policy.RequireClaim("city", "ankara","mmanisa");//sayfay� ankara ve manisada ya�ayanlar g�r�nt�leyebilir
    });

    policy.AddPolicy("ExchangePolicy", policy =>
    {
        policy.AddRequirements(new ExhangeExpireRequirement());
    });

    policy.AddPolicy("VioloncePolicy", policy =>
    {
        policy.AddRequirements(new ViolonceRequirement() { ThresholdAge = 18 });
    });


    policy.AddPolicy("OrderPermissionReadOrDelete", policy =>
    {
        //policy.RequireClaim("permission", Permissions.Order.Read, Permissions.Order.Delete, Permissions.Stock.Delete); -->e�er virg�lle verirsek veya anlam�na gelir herhangi birine sahip olan sayfaya eri�ebilir biz veya de�il ve istiyoruz ayr� ayr� yazar�z o zaman
        policy.RequireClaim("permission", Permissions.Order.Read);
        policy.RequireClaim("permission", Permissions.Order.Delete);
        policy.RequireClaim("permission", Permissions.Stock.Delete);
    });


    policy.AddPolicy("Permissions.Order.Read", policy =>
    {
        policy.RequireClaim("permission", Permissions.Order.Read);
    });


    policy.AddPolicy("Permissions.Order.Delete", policy =>
    {
        policy.RequireClaim("permission", Permissions.Order.Delete);
    });


    policy.AddPolicy("Permissions.Stock.Delete", policy =>
    {
        policy.RequireClaim("permission", Permissions.Stock.Delete);
    });
});


builder.Services.ConfigureApplicationCookie(opt =>
{
    var cookieBuilder = new CookieBuilder();
    cookieBuilder.Name = "UdemyAppCookie";

    opt.LoginPath = new PathString("/Home/SignIn");
    opt.LogoutPath = new PathString("/Member/Logout");
    opt.AccessDeniedPath = new PathString("/Member/AccessDenied");
    opt.Cookie = cookieBuilder;
    opt.ExpireTimeSpan = TimeSpan.FromDays(60);
    opt.SlidingExpiration = true;//60 i�inde kullan�c� login olursa s�re tekran uzat�r.E�er ki kullan�c� 60 g�n boyunca herhangi bir �ekilde giri� yapmazsa login hesab�na y�nlendirilir.
});


var app = builder.Build();
using (var scope = app.Services.CreateScope())///uygulama aya�a kalkarken buras� bir kere �al��acak 
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();//rolemanager scopelar bitti�i an memoryden d��ecek

    await PermissionSeed.Seed(roleManager);
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
