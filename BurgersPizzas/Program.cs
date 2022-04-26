using BurgersPizzas.DataAccess;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.AddRazorPages();

// Setup EF connection
// https://stackoverflow.com/a/43098152/1385857
// https://medium.com/executeautomation/asp-net-core-6-0-minimal-api-with-entity-framework-core-69d0c13ba9ab
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration["Data:SchoolLocal:ConnectionString"]));

// added from MVC template
//services.AddMvc();
// https://stackoverflow.com/a/58772555/1385857
builder.Services.AddMvc(option => option.EnableEndpointRouting = false);

WebApplication app = builder.Build();

// https://stackoverflow.com/a/71258326/1385857
// https://stackoverflow.com/a/71461320/1385857
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseAuthorization();

//app.MapRazorPages();

app.Run();
