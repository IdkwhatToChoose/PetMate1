using Microsoft.AspNetCore.Authentication.Cookies;

using PetMate.Helpers;
using PetMate.Model;

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
        options.Cookie.HttpOnly = false;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.Name = CookieAuthenticationDefaults.AuthenticationScheme;
    }).AddGoogle(options =>
    {
        options.ClientSecret = Environment.GetEnvironmentVariable("GClientSecret") ?? builder.Configuration["clientSecret"];
        options.ClientId = Environment.GetEnvironmentVariable("clientID") ?? builder.Configuration["clientID"];
        options.Scope.Add("https://www.googleapis.com/auth/calendar.readonly");
        options.SaveTokens = true;
    });

builder.Services.AddDistributedMemoryCache(); // Required for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//builder.Services.AddHsts(options =>
//{
//    options.Preload = true;
//    options.IncludeSubDomains = true;
//    options.MaxAge = TimeSpan.FromDays(80);
//    options.ExcludedHosts.Add("localhost");
//});

//builder.Services.AddHttpsRedirection(options =>
//{
//    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
//    options.HttpsPort = 5001;
//});

Stripe.StripeConfiguration.ApiKey = builder.Configuration["StripeSecret"];


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

app.UseSession();

//app.UseCookiePolicy(new CookiePolicyOptions() { Secure = CookieSecurePolicy.Always });  

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
