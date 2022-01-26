using IdentityApp.Authorization;
using IdentityApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Register Authorization handlers.
builder.Services.AddScoped<IAuthorizationHandler,
                      InvoiceOwnerAuthorizationHandler>();

builder.Services.AddSingleton<IAuthorizationHandler,
                      InvoiceAdminAuthorizationHandler>();

builder.Services.AddSingleton<IAuthorizationHandler,
                      InvoiceManagerAuthorizationHandler>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // We are going to set a new password policy where we require an alpha numberic string of minimum 6 characters.
    //we make sure the password should contain a digit.
    options.Password.RequireDigit = true;
    //we also make sure the password contains a lowercase character
    options.Password.RequireLowercase = true;
    // we make sure the password if alpha numeric
    options.Password.RequireNonAlphanumeric = true;
    // additionally, make sure the password as an uppercase character.
    options.Password.RequireUppercase = true;

    // Let us now edit the Lockout settings.
    // we set the maximum number of failed access attempts before a user is locked out to 3 times
    options.Lockout.MaxFailedAccessAttempts = 3;
    // Let's set that our application will lockout users for 3 minutes
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
    // we also set that our lockout mechanism should work for newly created users
    options.Lockout.AllowedForNewUsers = true;

    // Let's also set user specific settings.
    // We set that each user in this application should have a unique email address.
    options.User.RequireUniqueEmail = true;
});

var app = builder.Build();
// we create a scope to run the database seeding process
using (var scope = app.Services.CreateScope())
{
    // we then extract the registered services from the scope
    var services = scope.ServiceProvider;
    // our database context is apart of these registered services
    var context = services.GetRequiredService<ApplicationDbContext>();
    // we run migrations if any or create the database is it does not exist already.
    context.Database.Migrate();

    // read our secrets configuration for the seed account password.
    var seedUserPass = builder.Configuration.GetValue<string>("SeedUserPass");

    // initialize the database with roles, accounts and sample invoices
    // accounts will be seeded with the password we set in our secrets json configuration.
    await SeedData.Initialize(services, seedUserPass);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
