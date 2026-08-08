var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly;

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehaviour<,>));
});
builder.Services.AddMarten(cnfg => {
    cnfg.Connection(builder.Configuration.GetConnectionString("MartenConnection")!);
}).UseLightweightSessions();

if (builder.Environment.IsDevelopment())
{
    builder.Services.InitializeMartenWith<CatalogInitialData>();
}

builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("MartenConnection")!);

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddCarter();

var app = builder.Build();

app.MapHealthChecks("/health", new HealthCheckOptions { 
 ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapCarter();

app.UseExceptionHandler(_ => { });
//app.UseExceptionHandler(exceptionHandlerApp =>
//{
//    exceptionHandlerApp.Run(async context =>
//    {
//        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>()?.Error;
//        if (exceptionHandlerPathFeature == null)
//        {
//            return;
//        }


//        var problemDetails = new ProblemDetails
//        {
//            Status = StatusCodes.Status500InternalServerError,
//            Title = exceptionHandlerPathFeature.Message,
//            Detail = exceptionHandlerPathFeature.StackTrace
//        };

//       var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
//        logger.LogError(exceptionHandlerPathFeature, exceptionHandlerPathFeature.Message);

//        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
//        context.Response.ContentType = "application/json";

//        await context.Response.WriteAsJsonAsync(problemDetails);
//    });
//});

app.Run();
