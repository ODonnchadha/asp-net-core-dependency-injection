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

- Registering More Complex Service
- Injecting and Resolving Dependencies
- Beyond the Built-in Container