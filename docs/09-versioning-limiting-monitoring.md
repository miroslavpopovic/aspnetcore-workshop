# Versioning, usage limiting, logging and monitoring

In this part, we'll cover few more things that helps us tighten up our APIs.

## Versioning

Whenever we make a change in our API, that would break some existing API client, we need to introduce a new version. Old versions still need to function, until all API clients switch to the newer one. There are various strategies for implementing API versioning:
- URL
    - /api/v2/games/
- Query string
    - /api/games?api-version=2
- Custom request header
    - api-version: 2
- Accept header
    - Accept: application/json;v=2

There's a NuGet package that supports all of those strategies: `Microsoft.AspNetCore.Mvc.Versioning`. It usually works really well with NSwag, however, it's still [not working for ASP.NET Core 3.0](https://github.com/microsoft/aspnet-api-versioning/issues/499).

Implementing versioning manually is possible, but setting up correct version numbers for Swagger documentation using NSwag is harder. To avoid complicating things in here, we'll just skip versioning part for now.

**TODO:** Update this part when `Microsoft.AspNetCore.Mvc.Versioning` starts working with ASP.NET Core 3.0.

## Usage limiting

If your API has many API clients, sooner or later, one of them will try to abuse it, intentionally or unintentionally. One way to abuse the API would be to create a big number of requests in a short period of time.

To prevent that, we can implement usage limiting per token. Let's implement a simple middleware that will save the last access time for each token and save it to in-memory dictionary.

```c#
public class LimitingMiddleware
{
    private static readonly IDictionary<string, DateTime> TokenAccess = new Dictionary<string, DateTime>();

    private readonly RequestDelegate _next;
    private readonly ILogger<LimitingMiddleware> _logger;

    public LimitingMiddleware(RequestDelegate next, ILogger<LimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var request = context.Request;
        var path = request.Path.HasValue ? request.Path.Value : string.Empty;

        if (path.ToLowerInvariant().Contains("/api/"))
        {
            var token = request
                .Headers["Authorization"]
                .FirstOrDefault()?
                .ToLowerInvariant()
                .Replace("bearer ", "");

            if (token != null)
            {
                if (!TokenAccess.ContainsKey(token))
                    TokenAccess.Add(token, DateTime.UtcNow);
                else
                {
                    var lastAccess = TokenAccess[token];
                    TokenAccess[token] = DateTime.UtcNow;

                    if (lastAccess.AddSeconds(5) >= DateTime.UtcNow)
                    {
                        const string message = "Token limit reached, operation cancelled";

                        _logger.LogInformation(message);

                        var problem = new ProblemDetails
                        {
                            Type = "https://yourdomain.com/errors/limit-reached",
                            Title = "Limit reached",
                            Detail = message,
                            Instance = "",
                            Status = StatusCodes.Status429TooManyRequests
                        };

                        var result = JsonConvert.SerializeObject(
                            problem,
                            new JsonSerializerSettings
                            {
                                ContractResolver = new CamelCasePropertyNamesContractResolver()
                            });

                        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                        context.Response.ContentType = "application/json";

                        await context.Response.WriteAsync(result);

                        return;
                    }
                }
            }
        }

        await _next(context);
    }
}
```

We won't get into the details of the implementation. Just know that it will work on API URLs to check when the current token used for the request has accessed the API last time. If the time is under 5 seconds, an HTTP `409 Too Many Requests` response is returned.

To enable this middleware, you need to add the line to `Startup` class' `Configure` method, just after the `ErrorHandlingMiddlewareLine`:

```c#
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<LimitingMiddleware>();
```

Now you can test the behavior using Postman.

![Too many requests](images/postman-too-many-requests.png)

Note that the implementation above will not work correctly in distributed scenarios - when you have multiple instances of your API. Reason - we are using in-memory dictionary. It will be in a memory of the machine that's processing the request. All other machine won't have a copy. For that case, take a look at [ASP.NET Core caching](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/response?view=aspnetcore-3.0#other-caching-technology-in-aspnet-core).

## Logging and monitoring

Once your application is up and running, you need to somehow track its behavior in order to keep it healthy. There are a number of ways to do that. The most simple one would be to periodically look into application logs for any signs of unexpected behavior - or you could automate that with tools for log analysis.

### Logging

Thanks to libraries like NLog, Serilog and similar, we can define log output to go to file, database, some third party service, tools like Elasticsearch, etc. They have NuGet packages for integration with ASP.NET Core logging. For now, let's add [Serilog](https://github.com/serilog/serilog-aspnetcore) and enable logging to a file. Install the `Serilog.AspNetCore` and `Serilog.Sinks.File` (pre-release) NuGet packages.

Modify your `Program` class to look like this:

```c#
public class Program
{
    public static int Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.File(
                "./logs/time-tracker.log",
                fileSizeLimitBytes: 1_000_000,
                rollOnFileSizeLimit: true,
                shared: true,
                flushToDiskInterval:TimeSpan.FromSeconds(1))
            .CreateLogger();

        try
        {
            Log.Information("Starting web host");
            CreateHostBuilder(args).Build().Run();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .UseSerilog();
}
```

That's it. Serilog will now be used each time you log something using ASP.NET Core's `ILogger` and `ILogger<T>` interfaces. It is also configured to catch any unexpected exception while starting a host and log it as fatal error. All logs will go to `./logs/time-tracker.log` file. If you run the application now and make few requests, you'll see that the file is created and filled in with log entries.

You can configure multiple log outputs (called sinks in Serilog). For a list of available sinks, take a look at [Serilog wiki](https://github.com/serilog/serilog/wiki/Provided-Sinks).

### Health checks

ASP.NET Core has a notion of health checks. To understand what they are, you can look at status dashboards of popular services like [Azure](https://status.azure.com/en-us/status), [Google Cloud](https://status.cloud.google.com/), [Facebook](https://developers.facebook.com/status/dashboard/), [Twitter](https://api.twitterstat.us/), [Dropbox](https://status.dropbox.com/)... Basically, a number of different health checks are participating in forming the overall status image of the service.

Health checks can be a simple ping responses that just return HTTP `200 OK` when called, to indicate that the server is live, or they can do complex queries and requests themselves to form a response containing more in-depth information.

Health checks are also especially useful in containerized scenarios. Container tools like Docker can use health checks to decide whether the service in container is running or not, and perform decisions like whether to restart the container, based on the response.

To use health checks, you need to add all the services that health checks require. Add the following like to your `Startup.ConfigureServices` method:

```c#
services.AddHealthChecks();
```

Adding ping health check is simple. Just modify the `app.UseEndpoints`  call in your `Startup.Configure` method to look like this:

```c#
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/health");
});
```

If you run your application now and navigate to `/health` endpoint, you should get *Healthy* as a response. Even this simple check can help with Docker containers, for instance.

Health checks can also be used to validate the database status. There's an open-source project called [`AspNetCore.Diagnostics.HealthChecks`](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks) that contains a number of different health checks, SQLite db check among others. Let's install the package with SQLite check `AspNetCore.HealthChecks.Sqlite`.

Modify `Startup.ConfigureServices` method to include the new health check too:

```c#
services.AddHealthChecks()
    .AddSqlite(Configuration.GetConnectionString("DefaultConnection"));
```

If you run the application, and browse `/health` the response should again be *Healthy*, even though we have a new health check. If all health checks are passed, the response will be *Healthy* by default.

If we modify the `AddSqlite` call like this, it will return *Unhealthy*, since there is no table named *something* in the database:

```c#
services.AddHealthChecks()
    .AddSqlite(Configuration.GetConnectionString("DefaultConnection"), "select name from something");
```

This proves that SQLite health check works. Remove the SQL query from the method call above.

You can also create your own custom health checks, by implementing `IHealthCheck` interface and registering the health check class. As a sample, look into the source code of [`SqliteHealthCheck` on GitHub](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/master/src/HealthChecks.Sqlite/SqliteHealthCheck.cs).

Wouldn't it be nice to have a status page like Azure, Facebook and other services? There's a way to add it without much effort via `AspNetCore.HealthChecks.UI` NuGet package, which is also a part of `AspNetCore.Diagnostic.HealthChecks` project. Install that package and modify the `Startup` class as follows:

```c#
// At the end of ConfigureServices method:
services.AddHealthChecksUI();
```

*NOTE: Unfortunately, the line above is causing issue with .NET Core Preview 7. It's related to Entity Framework Core (again) and the underlined Sqlite database used for saving health checks. You can read the rest of this section, but won't be able to implement it yet if using Preview 7.*

```c#
// Modify app.UseEndpoints, and add UI middleware before it:
app.UseHealthChecksUI();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/health", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
});
```

Also, add the following settings to `appsettings.json` file:

```json
"HealthChecksUI": {
"HealthChecks": [
    {
    "Name": "HTTP-Api-Basic",
    "Uri": "/health"
    }
],
"EvaluationTimeOnSeconds": 10,
"MinimumSecondsBetweenFailureNotifications": 60
}
```

If you browse `/healthchecks-ui` now, you'll get the status page.

![Health checks status page](images/healthchecks-status.png)

*TODO: Unfortunately, `AspNetCore.HealthChecks.UI` is using EF Core under the cover, and it's more or less broken in Preview 6, hence the error message in screenshot above.*

### Other monitoring types

There are other things you can monitor in your database. For instance, doing performance tracking to see if your application is having some performance issues. Also, usage tracking - e.g. to track which token is using which resource and how often.

If you are using Azure cloud, it has a number of services ready for monitoring, like Azure Monitor, Application Insights, Log Analytics, etc.

There are also third party monitoring services like:
- https://newrelic.com/
- https://stackify.com/
- https://www.monitis.com/
- https://www.runscope.com/
- ...

With this part, we have completed the API features. In the next part, we'll investigate how to consume our API from client-side Blazor.

-------

Next: [Blazor client](10-blazor-client.md)
