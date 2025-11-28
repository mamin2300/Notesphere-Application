using Microsoft.EntityFrameworkCore;
using Notesphere.Operations.PlannerServices;
using Notesphere.Services.DashboardRepository;
using Notesphere.Services.NotesphereDataAccessLayer;
using Notesphere.Services.NotesRepository;
using Notesphere.Services.PlannerRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//register DbContext with SQLite
builder.Services.AddDbContext<NotesphereDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteConnection")));

//Interface 
builder.Services.AddScoped<INotesService, NotesRepository>();
builder.Services.AddScoped<IPlannerService, PlannerRepository>();
builder.Services.AddScoped<PlannerService>();
builder.Services.AddScoped<ConflictDetector>();
builder.Services.AddScoped<RecurrenceEngine>();
builder.Services.AddScoped<IDashboardServices, DashboardRepository>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
