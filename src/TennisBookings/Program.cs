#region Global Usings
global using Microsoft.AspNetCore.Identity;

global using TennisBookings;
global using TennisBookings.Data;
global using TennisBookings.Domain;
global using TennisBookings.Extensions;
global using TennisBookings.Configuration;
global using TennisBookings.Services.Bookings;
global using TennisBookings.Services.Unavailability;
global using TennisBookings.Services.Bookings.Rules;
global using TennisBookings.Services.Notifications;
global using TennisBookings.Services.Time;
global using TennisBookings.Services.Courts;
global using Microsoft.EntityFrameworkCore;
#endregion

using Microsoft.Data.Sqlite;
using TennisBookings.BackgroundService;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using TennisBookings.Shared.Services.Weather;
using TennisBookings.Shared.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using TennisBookings.Services.Membership;
using TennisBookings.Services.Greetings;
using TennisBookings.Caching;
using TennisBookings.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.Configure<ClubConfiguration>(builder.Configuration.GetSection("ClubSettings"));
services.Configure<FeaturesConfiguration>(builder.Configuration.GetSection("Features"));
services.Configure<BookingConfiguration>(builder.Configuration.GetSection("CourtBookings"));
services.Configure<BookingConfiguration>(builder.Configuration.GetSection("CourtBookings"));
services.TryAddSingleton<IBookingConfiguration>(s =>
	s.GetRequiredService<IOptions<BookingConfiguration>>().Value);

services.Configure<MembershipConfiguration>(builder.Configuration.GetSection("Membership"));
services.AddTransient<IMembershipAdvertBuilder, MembershipAdvertBuilder>();
services.AddSingleton<IMembershipAdvert>(s =>
{
	var builder = s.GetRequiredService<IMembershipAdvertBuilder>();
	// Using the builder instance to create the membership advert.
	builder.WithDiscount(10m);
	var advert = builder.Build();
	// As the service implementation.
	return advert;
});

//3.)
services.TryAddSingleton<GreetingService>();
services.TryAddSingleton<IHomePageGreetingService>(s => s.GetRequiredService<GreetingService>());
services.TryAddSingleton<ILoggedInUserGreetingService>(s => s.GetRequiredService<GreetingService>());
//2.) Overload with pre-instantiated (single) instance.
//var greetingService = new GreetingService(builder.Environment);
//services.TryAddSingleton<IHomePageGreetingService>(greetingService);
//services.TryAddSingleton<ILoggedInUserGreetingService>(greetingService);
//1.)
//services.TryAddSingleton<IHomePageGreetingService, GreetingService>();
//services.TryAddSingleton<ILoggedInUserGreetingService, GreetingService>();

// Explicit registration. It works with only a single type cached.
// services.TryAddSingleton<IDistributedCache<UserGreeting>, DistributedCache<UserGreeting>>();
// Passing an open generic. Argument provided at runtime.
services.TryAddSingleton(typeof(IDistributedCache<>), typeof(DistributedCache<>));

services.TryAddEnumerable(new ServiceDescriptor[]
{
	ServiceDescriptor.Scoped<IUnavailabilityProvider, ClubClosedUnavailabilityProvider>(),
	ServiceDescriptor.Scoped<IUnavailabilityProvider, UpcomingHoursUnavailabilityProvider>(),
	ServiceDescriptor.Scoped<IUnavailabilityProvider, OutsideCourtUnavailabilityProvider>(),
	ServiceDescriptor.Scoped<IUnavailabilityProvider, CourtBookingUnavailabilityProvider>()
});

services.AddBookingRules();
services.AddSingleton<IWeatherForecaster, RandomWeatherForecaster>();

//// e.g.: Creates a new instance of a ServiceDescriptor using its constructor.
//var serviceDescriptor1 = new ServiceDescriptor(typeof(IWeatherForecaster),
//	typeof(RandomWeatherForecaster), ServiceLifetime.Singleton);
//// services.Add(serviceDescriptor1);
//// e.g.: Uses a static factory method on the ServiceDescriptor.
//var serviceDescriptor2 = ServiceDescriptor.Describe(typeof(IWeatherForecaster),
//	typeof(RandomWeatherForecaster), ServiceLifetime.Singleton);
//// services.Add(serviceDescriptor2);
//// e.g.: Singleton static static factory.
//var serviceDescriptor3 = ServiceDescriptor.Singleton(typeof(IWeatherForecaster),
//	typeof(RandomWeatherForecaster));
//// services.Add(serviceDescriptor3);
//// e.g.: Generic singleton static static factory.
//var serviceDescriptor4 = ServiceDescriptor.Singleton<IWeatherForecaster,
//	RandomWeatherForecaster>();
//// services.Add(serviceDescriptor4);

services.AddScoped<ICourtBookingService, CourtBookingService>();
services.AddSingleton<IUtcTimeService, TimeService>();
services.AddScoped<IBookingService, BookingService>();
services.AddScoped<ICourtService, CourtService>();
services.AddScoped<ICourtBookingManager, CourtBookingManager>();
services.AddSingleton<INotificationService, EmailNotificationService>();

services.AddScoped<ICourtMaintenanceService, CourtMaintenanceService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/Bookings");
    options.Conventions.AuthorizePage("/BookCourt");
    options.Conventions.AuthorizePage("/FindAvailableCourts");
    options.Conventions.Add(new PageRouteTransformerConvention(new SlugifyParameterTransformer()));
});

#region InternalSetup
using var connection = new SqliteConnection("Filename=:memory:");
//using var connection = new SqliteConnection("Filename=test.db");
connection.Open();

// Add services to the container.
builder.Services.AddDbContext<TennisBookingsDbContext>(options => options.UseSqlite(connection));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<TennisBookingsUser, TennisBookingsRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<TennisBookingsDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

builder.Services.AddHostedService<InitialiseDatabaseService>();

builder.Services.ConfigureApplicationCookie(options =>
{
	options.AccessDeniedPath = "/identity/account/access-denied";
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
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
app.MapRazorPages();

app.Run();
