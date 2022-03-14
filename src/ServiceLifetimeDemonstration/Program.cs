using ServiceLifetimeDemonstration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddScoped<IGuidService, GuidService>();
builder.Services.AddScoped<IGuidTrimmer, GuidTrimmer>();
builder.Services.AddScoped<DisposableService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseMiddleware<CustomMiddleware>();

app.MapRazorPages();

app.Run();
