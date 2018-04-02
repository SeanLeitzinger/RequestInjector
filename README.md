# RequestInjector
Light weight library to enable directly injecting into requests in Asp.Net

### Usage
For more info see my post on [Request Injection](http://dotnetcultist.com/request-injection-in-asp-net-core/). This uses the built in 
Microsoft Dependency Injection. Extensions for other IoC containers can be added if people want it. Add the package 
to your project and then register your depdendencies. Example using Scrutor:

```
services.Scan(scan => scan
              .FromAssembliesOf(typeof(IRequest), typeof(GetTestRequest))
              .AddClasses()
              .AsSelf()
              .WithTransientLifetime());
```

**After** your dependencies are registered, add the following code to the StartUp:

```
var provider = services.BuildServiceProvider();

services.AddMvc(config =>
{
    config.ModelMetadataDetailsProviders.Add(new RequestInjectionMetadataProvider());
    config.ModelBinderProviders.Insert(0, new QueryModelBinderProvider(provider));
})
.AddJsonOptions(options =>
{
    options.SerializerSettings.Converters.Add(new RequestInjectionHandler<IRequest>(provider));
});

Finally, add the middleware below to your Configure method of your start up. This wraps the provider in a scope for the request:

app.Use(async (context, next) =>
{
    var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

    using (var scope = scopeFactory.CreateScope())
    {
        context.Items.Add("scope", scope);

        await next.Invoke();
    }
});
```
