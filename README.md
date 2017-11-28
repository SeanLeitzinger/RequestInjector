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
    config.ModelMetadataDetailsProviders.Add(new CustomMetadataProvider());
    config.ModelBinderProviders.Insert(0, new QueryModelBinderProvider(provider));
})
.AddJsonOptions(options =>
{
    options.SerializerSettings.Converters.Add(new RequestHandlerConverter<IRequest>(provider));
});
```
