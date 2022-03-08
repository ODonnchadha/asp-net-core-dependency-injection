## Dependency Injection in ASP.NET Core 6

- Course Overview:
  - Steve Gordon. Current version: .NET 6.0. C# 10. Visual Studio 2022.
  - Follow along with a "before" solution. Tennis Booking application.
    - U: member@example.com   P: password
    - U: admin@example.com    P: password

- Registering and Injecting Services:
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
    - 
  - Review the benefits:

- The Microsoft Dependency Injection Container
- Registering More Complex Service
- Injecting and Resolving Dependencies
- Beyond the Built-in Container