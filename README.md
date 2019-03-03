# RequestInjector
Light weight library to enable directly injecting into requests in Asp.Net

### Usage
For more info see my post on [Request Injection](http://dotnetcultist.com/request-injection-in-asp-net-core/). This uses the built in 
Microsoft Dependency Injection. Extensions for other IoC containers can be added if people want it. Add the package 
to your project and then register your depdendencies. Mark your requests with the IRequest interface. Example using Scrutor:

```
services.Scan(scan => scan
              .FromAssembliesOf(typeof(IRequest), typeof(GetTestRequest))
              .AddClasses()
              .AsSelf()
              .WithScopedLifetime());
```

**After** your dependencies are registered, add the following code to the StartUp:

```
var provider = services.BuildServiceProvider();

services.AddMvc(config =>
{
    config.ModelMetadataDetailsProviders.Add(new RequestInjectorMetadataProvider());
    config.ModelBinderProviders.Insert(0, new RequestInjectorModelBinderProvider());
})
.AddJsonOptions(options =>
{
    options.SerializerSettings.Converters.Add(new RequestInjectorHandler<IRequest>(provider));
});
