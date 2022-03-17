## Dependency Injection in ASP.NET Core 6

- COURSE OVERVIEW:
  - Steve Gordon. Current version: .NET 6.0. C# 10. Visual Studio 2022.
  - Follow along with a "before" solution. Tennis Booking application.
    - U: member@example.com   P: password
    - U: admin@example.com    P: password

- REGISTERING & INJECTING SERVICES:
  - Patterns & principles:
    - Inversion of control:
    - Dependency Inversion principle:
  - Indentify design problems:
    - e.g.: Requirement to display current weather conditions.
    - Tightly-coupled code is harder to maintain. New requirements may require many classes to be updated.
  - Attack plan:
    - Clean up code. Invert control. Apply dependency injection:
      - High-level modules should not depend on low-level modules. Both should depend upon abstractions.
      - "Abstractions should not depend upon details. Details should depend upon abstractions."
    - Refactor existing code:
      - Reduce coupling in code. Extract an interface.
    - Inverting control with constructor injection:
      - With constructor injection we define the list of required dependencies as parameters of the constructor for a class.
    - Inversion of control:
      - Inverting control of dependency creation.
      - An external component creates dependencies.
      - Combines with the dependency inversion principle to achieve loose coupling.
      - NOTE: "readonly:" Prevents methods from accidently assigning a different value to this dependency after the class is instantiated.
      - And resolving runtime dependicies.
      - Missing or misconfigured servoce registrations may not be apparent until runtime when ASP.NET Core attempts to resolve them.
  - Registering services:
    - IServiceCollection: ServiceA, ServiceB.
      ```csharp
        var services = builder.Services;
        services.AddTransient<TService, TImplementation>
      ```
      - Adds a transient service of the type specified in TService woth an impletation of type specified in TImplementation to the specified IService collection.
      - NOTE: Transient lifetime is a safe default until more is learned about service lifetimes.
    - Order of service registration:
      - Generally services can be registered in any other.
      - An exception to this is when intentionally registering multiple implementations of the same abstraction.
  - Inject framework dependencies:
  - Review the benefits:
    - Promotes loose coupling of components.
    - Promotes logical abstraction of components.
    - Supports unit testing.
    - Cleaner, more readable code.
  - Improved testing:
    - Can manually constract classes under test after applying inversion of control, providing fakes, mocks, or stubs.

- MICROSOFT DEPENDENCY INJECTION CONTAINER:
  - How ASP.NET Core uses the container.
  - What to register with the DI container.
  - Service lifetimes.
  - The ASP.NET Core Request Lifecycle:
    - Client (HTTP) -> Kestrel (Ports) -> HttpContext -> Root Container -> Scope created for the current request.
    - HTTP Context is passed into ASP.NET Core which is processed via the configured application pipeline.
    - Several middleware components and the final endpoint that produces the response. (e.g.: Razor page.)
    - Services are required to activate components are then resolved from this scope.
    - By resolving services via the scope rather than the root service provider instances are only shared across the current ASP.NET request.
  - Built-in DI Container. Resolving needed services.
    - HTTP Request -> Endpoint Activation -> Resolve Srvices (Dependency Injection Container.)
    ```csharp
      Microsoft.Extensions.DependencyInjection
      Microsoft.AspNetCore.App
    ```
    - Dependency Injection Container = Inversion Of Control Container.
    - A Dependency Injection Container is not a requirement to apply dependency injection. Using one simplifies management of dependencies.
    - Services are registered with the container at startup, and resolved via the container at runtime. (When required.)
    - Container creates and disposes. Components: 
      - IServiceCollection: Registering and configuring services.
      - IServiceProvider: Resolving a service at runtime.
  - What to register? Identifying dependencies.
    - Locate "new" keyword useage. Is the object a dependency?
      - Are methods called on the type which are required for the consuming type to function?
    - Apply dependency inversion.
      - Accept the dependency via the constructor.
    - Register the service with the container.
      - Dependency graph: Layers of dependencies due to SRP.
      - Register all services in the dependency graph.
      - Use constructor injection to accept dependencies.
      - The Microsoft container will manage creation of the object graph.
      - Some useages of the 'new' keyword do not identfy a dependency.
      - POCO. Plain Ol' CLR Objects. Registering POCO classes is a misuse of the dependency injection pattern.
        - These are not depencies. They are used mainly as the inout or output from methods.
        - They can be created using the 'new' keyword.
      - Does the object creation affect testability of the class?
      - Primitive types & string should not be injected or registered in a dependency injection container.
      - Value types, structs, cannot be registered with the container.
    - Best practice for handing configuration: Use the built-in ASP.NET Core configuration system & options pattern.
    - Service Lifetime:
      - Registering lifetimes on the IServiceCollection.
      - The DI container tracks the instances it creates. Objects are disposed of or released for garbage collection once their lifetime ends.
      - The lifetime affects the creation and reuse of service instances. Choose wisely!
      - AddTransient<>(); A new instance every time the service is resolved.
        - Every dependent class receives its own unique instance when the dependency is injected by the container.
        - Methods on the instance are safe to mutate internal state. Without fear of access by other consumers and friends.
        - Most useful when the service contains mutable state. And is not considered thread safe. Small performance cost.
        - Memory must be allocated. Multiple objects are created. More work for the garbage collector.
        - Easiest to reason about. Safest choice.
      - AddSingleton<>();
        - One shared instance for the lifetime of the container (application.) No disposal or garbage collection. 
        - Can improve application performance, especially if the object is expensive to construct.
        - Must be thread-safe. Avoid mutable state.
        - Suited to: Functional stateless services. e.g.: Caches. 
        - Consider frequency of use vs. memory consumption. Possible to create memory leaks using the singleton lifetime.
        - Beware of singleton services where memory usage grows significantly over time.
        - If a service is used very infrequently, the singleton lifetime may not be appropriate.
      - AddScoped<>();
        - An instance per scope (request.) Similiar to singleton, but within the context of the scope.
        - The container creates a new instance per request. Not required to be thread-safe.
        - Components used in the request lifecycle receive the same dependency instance.
        - Useful if a service may be required by multiple consumers per request. e.g.: EF's DbContext.
        - DbContext change tracking works across a single request.
        - Should not be captured by singleton services.
      - Avoiding captive dependencies:
        - Ensure that the service lifetime is approprite. Consider the lifetime of dependencies.
        - Captive dependencies. May live longer than intended.
        - NOTE: A service should not depend upon a service with a lifetime shorter than its own.
        - e.g.: A scoped service with a transient service dependency means that a single instance of the transient service will life for the lifetime of the scopped service.
        - Side-effects of captured dependencies.
          - Accidental sharing of non-thread safe services between threads.
          - Objects living longer that their expected lifetime.
      - Scope Validation:
        - Enabled by default in development. ASPNETCORE_ENVIRONMENT.
        - Validates container scopes. e.g.: No scoped services are captured within singleton services.
        - Validation occurs at startup when the buld method is invoked. Causing a runtime InvalidOperationException.
        - Register both services with the same lifetime.
        - Configure extra options:
        ```csharp
          builder.Host.UseDefaultServiceProvider(options => {
            options.ValidateScopes = false;
          });
        ```
      - Disposal of Services:
        - IDisposable. Dispose pattern:
          - Disposable types:
          - A using block or using statement is leveraged to signal release of a disposable type.
          - The compiler generates code which class Dispose() as soon as the consuming code no longer needs it.
          - The DI container supports IDisposable types. Calls Dispose() on instances at the end of their lifetime.
          - Automatic for types owned by the container. User-created instances are not disposed. Lifetime is managed externally.
        - IAsyncDisposable. Supports asynchronous disposal of types. IServiceProvider supports disposal as asynchronous types.
          - .NET 6:
          - Added a new CreateAsyncScope extension method which returns a scope wrapped in AsyncSericeScope: IDisposable, IAsyncDisposable.
          - This wrapper determines if IServiceScope implements IAsyncDisposable.
        - Enjoy (& beware) the ValidateOnBuild(). Configurable on the ServicePoviderOptions.
          - Enabled by default in development. Triggered by builder.Build();
        - ASP.NET Core Component Activation: 
          - Controllers & Razor pages are activated per request and are not directly registered with the DI container by default.

- REGISTERING MORE COMPLEX SERVICES:
  - Upcoming:
    - IServiceCollection extension methods. Advanced service requirements.
    - Review the options when using the built-in Microsoft container, and explore where we reach limitations.
    - Improving registration code organization.
  - Service Descriptors:
    - Contain information about registered services. We rarely nee to work directly with service descriptors.
    ```csharp
      public class ServiceDescriptor
      {
        public Type ImplementationType { get; }
        public Type ServiceType { get; }
        public ServiceLifetime Lifetime { get; }
        public object ImplementationInstance { get; }
        public Func<IServiceProvider, object> ImplementationFactory { get; }
      }
    ```
    - Created with (Try)AddTrasient(), etc... Used internally be the service provider to resolve services.
  - Duplicated service registrations.
    - Hit breakpoint. Run locals window for name/value. NOTE: Last registration wins RE: runtime resolve.
  - Add() versus TryAdd() behaviour.
    - Using the TryAdd() extension method: NOTE: Only registers a service if the service type does not already exist in the IServiceCollection.
    - TryAdd() methods are far more convenient in complex applications that contain many service registrations. Extent is more clear.
  - Registering an interface multiple times:
    - Multiple entires or registrations in a service collection. Last in is the preferred choice.
    - Replacing:
    - Looks for the first existing registration matching the service type.
    - If an entry is located it is removed. A new registration for the service type is then added.
    - Its specified implementation type will be used for the new registration.
    - Replacing only supports removing the first registration of a service type from IServiceCollection.
    - Rare, except for perhaps Framework or third-party registrations.
    ```csharp
      services.Replace(ServiceDescriptor.Singleton<IWeatherForecaster, RandomWeatherForecaster>());
      services.RemoveAll(IWeatherForecaster);
    ```
  - Why the container allows multiple service registrations.
  - Implement a rule pattern by registering multiple implementations of an interface.
    - Add new functionality with minimal changes to existing code. e.g.: BookingRuleProcessor.
  - IEnumerable<T> Dependencies:
    - The service provider resolves all registered implementations for the service type.
    - This only applies when the parameter is typed as IEnumerable.
  - Simple Extensibility:
    - Adding new rules: Add new ICourtBookingRule implementation. Rehgister it with IServiceCollection.
    - A nice example of SRP and seperation of concerns. Application is easier to maintain.
    - What happens if the same implentation is added twice?
      - The Add() methods are not idempotent. Recommended to avoid duplication and express intent more clearly.
      ```csharp
        services.TryAddEnumerable(ServiceDescriptor.Scoped<IUnavailabilityProvider, ClubClosedUnavailabilityProvider>());
        services.TryAddEnumerable(new ServiceDescriptor[]
        {
          ServiceDescriptor.Scoped<IUnavailabilityProvider, ClubClosedUnavailabilityProvider>(),
          ServiceDescriptor.Scoped<IUnavailabilityProvider, UpcomingHoursUnavailabilityProvider>()
        });
      ```
    - Registering services via implementation factory. Provide the complete control of the implentation type.
      - Signature:
        ```csharp
          public static void TryAddSingleton<TService>(this IServiceCollection services,
            Func<IServiceProvider, TService> implentationFactory) where TService: class;
        ```
      - Invoked at runtime. Has access to the built IServiceProvider. May resolve services. 
      - Responsible for returning a constructed instance of the service type.
      - Forwarding registerations.
    - Poor candidate for dependency injection. e.g.:
    ```csharp
    	public MembershipAdvert(decimal offerPrice, decimal discount)
      {
        OfferPrice = offerPrice;
        Saving = discount;
      }
      ```
      - Why register an implementtion against multiple services?
      - e.g.: NOTE: Two singleton instances due to unique registration.
      ```csharp
        services.TryAddSingleton<IHomePageGreetingService, GreetingService>();
        services.TryAddSingleton<ILoggedInUserGreetingService, GreetingService>();
      ```
      - Any instances created outside of the dependency injection container ar NOT automatically disposed of or released for garbage collection:
      ```csharp
        var greetingService = new GreetingService(builder.Environment);
        // Overload with pre-instantiated (single) instance.
        services.TryAddSingleton<IHomePageGreetingService>(greetingService);
        services.TryAddSingleton<ILoggedInUserGreetingService>(greetingService);
      ```
    - Registering open generic service. e.g.:
    ```csharp
      public interface ILogger<out TCategoryName> : ILogger
    ```
    - Create extension methods to improve readibility.
      - Grouping common functionality. Particularly beneficial in libraries to ensure correct registration of all required services.

- INJECTING & RESOLVING DEPENDENCIES:
  - Upcoming:
    - Locations. Constructor injection. Action injection. Middleware injection. Razor view injection. 
    - Minimal API handler injection. Background/hosted service injection. (Manual scope creation.)
  - Service Resolution Mechanisms:
    - IServiceProvider. Services & their dependencies must be registered with the DI container.
    - When creating an instance of a service from the container, its dependencies will also be resolved from the container.
  - (Static) ActivatorUtilities:
    - Can create objects that are not registered directly into the DI container.
    - Create an object via its constructor. Arguments can be supplied directly or resolved from an IServiceProvider.
    - Used to activate framework components. e.g.: Controllers, tag helpers, model binders.
    - SHould not be called by application code.
  - Constructor Injection:
    - Controllers, Razor Page Models, ViewComponents, TagHelpers, Filters, Middleware, application classes.
    - Constructor rules:
      - Assign default values for arguments not provided by the container.
      - When services are resolved, a public constructor is required.
      - Only a single application constructor can exist for services resolved via ActivatorUtilities (framework components such as controllers.)
    - NOTE: Services resolved from the container directly can support multiple application constructors.
    - Injecting dependencies into controllers:
      - ASP.NET Core activates (creates) a new controller instance per request. So, action injection.
    - Injecting into action methods:
      - Choosing: Depends on how widely used a dependency is used within a controller. COnstructor injection is most common.
      - Action injection may be more efficient if a dependency is only needed by a single action.
      - Such cases may indicate that a controller has too many responsibilities. Split actions across more focused controllers.
      ```csharp
        [Route("Maintenance/Upcoming")]
        public async Task<ActionResult> UpcomingMaintenance(
          [FromServices] ICourtMaintenanceService courtMaintenanceService)
      ```
    - DI with Minimal APIs.
    - Injecting services into middleware.
      - Middleware activation differs from framework components such as controllers.
      - NOTE: Each middleware component is constructed once when the application starts.
        - Constructor dependencies are therefore resolved from the root container. So singletons within the application.
        - Dependencies are captured for the application's lifetime.
        - Avoid injecting scoped or transient services via consructor injection.
      - The Invoke/InvokeAsync method is onvoked once per request. Parameters are resolved from the request scope.
    - Middleware DI:
      - Constructor:
        - Called once when the application starts. Supports only singleton services.
        - Scoped or transient services will be captured and may not behave correctly.
      - Invoke/InvokeAsync:
        - Runs once per request. Services are activated and resolved from the request scope.
        - Supports all service lifetimes.
    - Injecting services into factory-based middleware. Factory-based components implement IMiddleware interface.
      - Resolved by an IMiddlewareFactory on a per-request basis. 
      - Transient & scoped services may be injected via their constructors. Rarely used.
    - Injecting dependencies into Razor views.
      - Take care to avoid the overuse of view injection. Avoid mixing concerns by inclusing business logic in Razor views.
      - Accessing injected static configuration data inside views can be convenient.
      - Ensure injected services are specific to the concern of view rendering:
        - Localizing content. Populating lists.
      - Pass state via page models, injecting dependencies into models rather than views.
    - Injecting dependencies into Hosted Services:
      - Manually creating scopes.
      - At startup, the StartAsync method is called on all registered IHostedService implementations.
      - Implementations of IHostedService are created when the application starts.
      - Instances live until the application exits.
      - Services injected into their constructors are captured for the life of the application.
        - Only singleton services are safe to be injected in this manner.
      - However, the constructor accepts an IServiceScopeFactory. Registered independently via the framework.
        - This blocks. Avoid anything long-running.
      = Service locator pattern: Resolving services directly from the provider. This is considered a bad practice. Prefer injection.
      - Manual scope creation:
        - Only required outside of the ASP.NET Core request cycle. Hosted services are the main example where this is necessary.
        - Scopes must be disposed once they are no longer required to release scoped resources.

- BEYOND THE BUILT-IN CONTAINER:
  - [Scrutor](https://github.com/khellang/Scrutor): (1) Support for assembly scanning and (2) applying the decorator pattern.