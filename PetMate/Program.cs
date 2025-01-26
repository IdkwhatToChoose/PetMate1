using Microsoft.AspNetCore.Authentication.Cookies;
using PetMate.Helpers;

var builder = WebApplication.CreateBuilder(args);
var appass = builder.Configuration["appass"];

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IUserAndShelterManager, UserAndShelterManager>();
builder.Services.AddScoped<IFileManager, FileManager>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS only
    });
var app = builder.Build();


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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Calendar",
    pattern: "Calendar",
    defaults: new { controller = "Shelter", action = "Index" });


app.Run();
