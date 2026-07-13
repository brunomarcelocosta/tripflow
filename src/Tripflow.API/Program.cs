using System.Text.Json.Serialization;
using Tripflow.API.Middlewares;
using Tripflow.Infra.Context.Middlewares;
using Tripflow.Infra.IoC.DI;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddOpenApi();

builder.Services.AddSwagger();
builder.Services.AddAutoMapper();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddTripflowSecurity();
builder.Services.AddUseCases(builder.Configuration);

builder.Services.AddRequestContexts();
builder.Services.AddKeycloakIntegration(builder.Configuration);
builder.Services.AddTripflowAuthentication(builder.Configuration);
builder.Services.AddTripflowAuthorization();
builder.Services.AddStorage(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthentication();
app.UseMiddleware<UserProvisioningMiddleware>();
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
