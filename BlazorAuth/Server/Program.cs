using BlazorAuth.Server;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration);

builder.Services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.Name = ".BlazorTest.Auth";
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Add a fallback authorisation policy that will be invoked by the static assets middleware
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Static files are served after authentication and authorisation so that they are not publicly accessible
// The authorisation fallback policy will prevent non-authenticated users accessing them
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
// Require authorisation by default on all endpoints
app.MapRazorPages().RequireAuthorization();
app.MapControllers().RequireAuthorization();
app.MapFallbackToFile("index.html").RequireAuthorization();
app.MapLoginAndLogout().RequireAuthorization();

app.Run();
