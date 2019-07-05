# Blazor client

[Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/client) is a new project by Microsoft which started his life as an experiment by [Steve Sanderson](https://twitter.com/stevensanderson). It can be split into two parts - Blazor components and hosting model.

Blazor components are a new way to define encapsulated component. They can be shared between projects as NuGet packages and embedded into both ASP.NET Core server side applications as well as client side applications. Code-behind logic and event handlers are written in C#. Third-party vendors like [Telerik](https://www.telerik.com/blazor-ui), [DevExpress](https://www.devexpress.com/blazor/) and [Syncfusion](https://www.syncfusion.com/blazor-components) are already providing their own set of Blazor components.

Like mentioned above, Blazor components can be hosted on both client and server. What that means is that client-side Blazor enables running C# / .NET code in the browser. This is possible using WebAssembly standard. It means that you can write client-side web applications in .NET, without the need for JavaScript or frameworks like Aurelia, React, Vue or Angular. It's basically a replacement for them.

-------

Next: [Resources and further reading](99-resources.md)