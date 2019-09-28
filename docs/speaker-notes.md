# Speaker notes

## 01 Introduction

- Personal introduction - each attendee
- Introduction to .NET Core presentation
- Introduction to ASP.NET Core presentation
- Check if environments are set up correctly:
    - Use [Prerequisites](00-prerequisites.md) document as guideline
    - Latest Visual Studio 2019 Preview is installed with ASP.NET and .NET Core workloads
    - Latest Blazor extension is installed in Visual Studio Extensions
    - Latest .NET Core SDK is installed - use `dotnet --info` to check
    - Git client is installed - use `git --version` to check
    - Latest Blazor templates installed - use `dotnet new` to check
    - Postman installed
- Alternative for Visual Studio 2019 Preview:
    - Latest Visual Studio Code is installed
    - C# for Visual Studio Code extension
- Each attendee should have a GitHub account
- Define individual or team organization
- Add attendees to Slack channel

## 02 Tools and templates

- `dotnet` CLI
    - `dotnet --info`
    - `dotnet new`
    - `dotnet new console`
    - `dotnet run`
    - `code .` - if Visual Studio Code is installed
    - `dotnet new web`
    - `dotnet new webapi`
    - `dotnet new mvc --auth Individual`
- Visual Studio 2019
    - New project dialog
    - Filters and options
    - Templates
    - ASP.NET Core templates
- Templates to analyze
    - Console app
    - Empty ASP.NET Core
    - ASP.NET Web API
    - ASP.NET MVC - with Individual auth
    - SPA - Angular or React
    - Worker
    - gRPC
    - Server-side Blazor
    - Client-side Blazor
- Explain:
    - Folder / namespace structure
    - Dependencies
    - `Program` and `Startup`
    - `appsettings.json`
    - `wwwroot` folder
    - `.csproj` file
    - Controllers - base `Controller`, attribute routing, `IActionResult`, `async`, model binding, model validation, view models...
    - Entity Framework Core, `DbContext`, migrations
    - Views, Razor syntax, `_Layout` file, `_ViewStart`, `_ViewImports`, tag helpers, `ViewData` / `ViewBag`
    - ASP.NET Core Identity - scaffold identity, register, login, 2FA

## 03 Choosing a domain

- Discuss about the domain to create
- Choose very simple domain - up to 4-5 entities
- Explain user stories
- Define user stories for the domain

## 04 Project initialization

- Introduction to REST APIs presentation
- Create a new `TimeTracker` ASP.NET Core Web API project
- Reorganize repository folders - move code to `src` sub-folder
- Add to git source control - using Team Explorer
- Git synchronize
- Create an empty GitHub repository
    - Don't enable README, .gitignore and LICENSE
- Publish to new repository from VS
- Add `README.md` and `LICENSE` file to root folder - https://choosealicense.com/
- Run the app - F5
    - Explain other options
    - IIS Express, etc.
- Make a request from Postman to `/weatherforecast`

## 05 Domain models and database

### Domain models
- Revisit selected domain and user stories
- Create `User`, `Client`, `Project` and `TimeEntry` classes with properties
- Add `[Required]` attribute where necessary

### Database and EF Core

- Add a new empty file in the Web API project root - `app.db`
- Modify `appsettings.json` - add connection string `"ConnectionStrings": { "DefaultConnection": "DataSource=app.db" }`
- Explain installing NuGet packages via CLI `dotnet add package ...` and VS NuGet package manager
- Add `Microsoft.EntityFrameworkCore.Sqlite` NuGet package
- Add `Microsoft.EntityFrameworkCore.Tools`
- Add `TimeTrackerDbContext` with necessary `DbSet<T>` properties

### Migrations

- Explain what the db migrations are
- `dotnet-tools.json`
- `dotnet new tool-manifest` from the solution root
- `dotnet tool install dotnet-ef` to install EF Core tools
- From the project folder: `dotnet ef migrations add "InitialMigration" --output-dir "Data/Migrations"`
- Look at the migrations folder content
- `dotnet ef database update` to update the database
- Use [SQLite/SQL Server Compact Toolbox](https://marketplace.visualstudio.com/items?itemName=ErikEJ.SQLServerCompactSQLiteToolbox) VS extension to look at db content

### Data seeding

- Add `OnModelCreating` override to `DbContext`
- Use `modelBuilder.Entity<User>().HasData()` method to seed
- `dotnet ef migrations add "SeedData" --output-dir "Data/Migrations"`
- `dotnet ef database update`

## 06 Controllers and actions

### View Models

- Explain what View Models / Input Models are
- Create `XxModel`, `XxInputModel` pairs
- Add `FromXx` static methods to `XxModel` classes for data mapping
- Add `MapTo` methods to `XxInputModel` classes to map data to existing entity models
- Add `PagedList<T>` for paging

### Validation

- Explain validation and Data Annotations
- Explain Fluent Validation
- Install `FluentValidation.AspNetCore`
- Add validators for all input models
- Modify `Startup` class to initialize Fluent Validation - `services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UserInputModelValidator>());`

### Data access

- Explain using `DbContext` classes directly, Repository pattern, CQRS

### Controllers

- Create `UserController`
- Define and explain `[ApiController]`
- Explain attribute routing `[Route("/api/users")]`
- Explain base `Controller` class
- Explain dependency injection
- Explain how logging is done and `ILogger<T>`
- Explain HTTP verbs - GET, POST, PUT, DELETE...
- Add `GetById` method
- Add `GetPage` method with paging parameters
- Add `Delete` method
- Add `Create` POST method
- Add `Update` PUT method
- Add other controllers

### Postman collection

- Explain what are Postman collections
- Get into more details about requests
- Create Postman collection for API
- See how errors look like in Postman
- Define variable for `rootUrl`

### Error handling

- Implement `ErrorHandlingMiddleware` and include it in `Startup`

## 07 Securing APIs

- HTTPS everywhere
- OAuth 2.0 and OpenID Connect
- JWT Token auth
    - Add `Tokens:Issuer` and `Tokens:Key` settings to `appsettings.json`
    - Install `Microsoft.AspNetCore.Authentication.JwtBearer`
    - Create extension method `ServiceCollectionExtensions.AddJwtBearerAuthentication`
    - Add to `Startup`
        - `services.AddJwtBearerAuthentication(Configuration);`
        - `app.UseAuthentication();`
- Add `[Authorize]` and `[Authorize(Roles = "admin")]` to controllers and action methods
- Authorization server
    - Mention IdentityServer4, OpenIddict, Auth0, Okta...
    - Implement `JwtTokenGenerator`
    - Implement `DummyAuthController` with `/get-token` route
- Organize Postman collection into folders - *No Auth* folder
- Duplicate folder into *Bearer Token Auth*
- Define variables for `token` and `adminToken`
- Move admin requests into `Admin` sub-folder

## 08 Testing and documentation

- Explain unit and integration testing
- Add `TimeTracker.Tests` xUnit test project - into `tests` folder
- Unit testing
    - Add `UnitTests/UsersControllerTests`
    - Add `FakeLogger<T>`
    - Initialize `TimeTrackerDbContext` with In Memory database
    - Write tests for `GetById` method
    - Use VS Test Explorer to run tests
    - Write tests for `GetPage` method
    - Write tests for `Delete` method
- Integration testing
    - Add `IntegrationTests/UsersApiTests`
    - Initialize `TestServer` and test `HttpClient`
    - Initialize two tokens - admin and non-admin
    - Write tests for `Delete` functionality
- Documentation
    - Install `NSwag.AspNetCore`
    - Install `Microsoft.AspNetCore.Mvc.NewtonsoftJson`
    - Implement `ServiceCollectionExtensions.AddOpenApi()` method
    - Add to `Startup`
        - `services.AddOpenApi();`
        - `app.UseOpenApi();`
        - `app.UseSwaggerUi3();`
    - Change launch URL to `/swagger`
    - Run the app and play with the Swagger UI interface - auth, etc.
    - Add `[OpenApiIgnore]` for controllers to skip
    - Add XML documentation to Controllers and View Models
    - Enable XML generation in project properties
    - Rebuild and test in Swagger UI

## 09 Versioning, usage limiting and monitoring

- Versioning
    - Talk about API versioning
    - Different versioning strategies
    - Install `Microsoft.AspNetCore.Mvc.Versioning` and  `Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer` pre-release
    - Create copy of controllers in `Controllers/V1`
    - Update controller routes to include `/api/v{version:apiVersion}/`
    - Add `[ApiVersion("...")]` attribute to controllers
    - Update all `CreatedAtAction` method calls to include `version = "..."` in route parameters
    - Create `ServiceCollectionExtensions.AddVersioning` method
    - Refactor `AddOpenApi` method to support versioning - add `InitializeOpenApiDocumentOptions` private method
    - Call `service.AddVersioning()` from `Startup.ConfigureServices`
    - Test swagger UI output in browser
    - Modify Postman collection - add new folder for versioned calls
    - Fix integration tests in `UserApiTests` to include version number
- Limiting
    - Create `LimitingMiddleware`
    - Add to `Startup`
        - `app.UseMiddleware<LimitingMiddleware>();`
- Logging
    - Mention different logging libraries
    - Install `Serilog.AspNetCore` and `Serilog.Sinks.File`
    - Modify `Program` class to initialize logger
    - Run the app, make several requests, check the log file content
- Health checks
    - Talk about service status pages
    - Add to `Startup`
        - `services.AddHealthChecks();`
        - `endpoints.MapHealthChecks("/health");` - under `UseEndpoints`
    - Install `AspNetCore.HealthChecks.Sqlite`
    - Modify `Startup`
        - `services.AddHealthChecks().AddSqlite(Configuration.GetConnectionString("DefaultConnection"));`
    - Try defining health check SQL query above, see the *Unhealthy* response
    - Custom health checks - `IHealthCheck` interface
    - Install `AspNetCore.HealthChecks.UI`
    - Add to `Startup`
        - `services.AddHealthChecksUI();`
        - `app.UseHealthChecksUI();`
        - `endpoints.MapHealthChecks("/health", new HealthCheckOptions { Predicate = _ => true, ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });`
    - Add health check settings to `appsettings.json` file
    - Browse `/healthchecks-ui`
- Mention Azure and third-party services for monitoring

## 10 Blazor client

- Introduction to Blazor presentation
- Create new Blazor (client-side) app - `TimeTracker.Client`
    - Set it up as a startup project
- Authentication
    - Mention that this is a preview of Blazor - future breaking changes are possible
    - Explain how we are simulating login
    - Install `Microsoft.AspNetCore.Components.Authorization` NuGet package
    - Add `Models.UserModel` class
    - Create `Security.TokenAuthenticationStateProvider`
        - Explain what it does
    - Add `Extensions.SecurityExtensions`
        - Call it from `Startup.ConfigureServices`
    - Modify `App.razor` to add `<CascadingAuthenticationState>`
        - Explain
    - Modify the main `_Imports.razor`
        - add `@using System.Net.Http.Headers`
        - add `@using Microsoft.AspNetCore.Authorization`
        - add `@using Microsoft.AspNetCore.Components.Authorization`
    - Add `wwwroot/scripts/localStorage.js`
        - Include it in `wwwroot/index.html`
    - Add `@attribute [Authorize]` attribute to some pages
- Login page
    - Enable CORS in API's `Startup`
        - Explain Cors
        - `services.AddCors()` before `AddControllers()`
        - `appUseCors()` before `UseOpenApi()`
    - Modify `MainLayout.razor`
        - Use `<AuthorizeView>`
        - Add `Login` and `Logout` links
        - Implement `Logout` by setting token and user to `null`
    - Add `Config` class to hold all the configuration constants (e.g. URLs)
    - Add `Services.ApiService` with `GetAsync` method
        - Explain `HttpClient` and how we get token
        - Register `ApiService` in `Startup` - `services.AddScoped<ApiService>();`
    - Implement `Login.razor` page
        - Explain `EditForm` component and its `Model` property
        - Explain buttons and `onclick` handlers
        - Explain `Login` methods
        - Explain how errors are displayed
- Preparing for custom pages
    - Comment out `LimitingMiddleware`
    - Remove `Counter.razor` and `FetchData.razor`
    - Modify links in `NavMenu.razor`
    - Refactor `ApiService` - add `SendAuthorizedRequest` private method
    - Implement create, update and delete methods
- Adding pages for users
    - Create `Users.razor`
        - Explain routing, DI, `OnInitializedAsync`...
    - Implement `Shared/Pager.razor` component
        - Explain parameters and how they are passed
        - Explain usage of `PagedList<T>`
        - Explain `Loader` function
    - Add `Models` folder, copy models from API project and clean up
    - Add data annotations validation
    - Mention how this could be moved to shared project
    - Add create and edit page and explain it
    - Add delete user page
- Leave attendees to implement all pages for clients and projects, or copy-paste from existing ones
    - Explain loading lookups and using `InputSelect` component
    - Implement `Models.Lookup` class
- Adding pages for time entries
    - Only add/edit and delete
    - Explain date selection
- Home page dashboard page
    - Explain search
    - Add new API endpoint `TimeEntriesController.GetByUserAndMonth` and explain

## Show resources page

## Close up!
