using Haskap.GlobalExceptionHandler;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        // using static System.Net.Mime.MediaTypeNames;
        context.Response.ContentType = Text.Plain;

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

        var exception = exceptionHandlerPathFeature?.Error;

        var exceptionMessage = exception?.Message /* is not null ? stringLocalizer[errorMessage] : null */ ;
        var exceptionStackTrace = exception?.StackTrace;
        var errorEnvelope = Envelope.Error(exceptionMessage, exceptionStackTrace, exception?.GetType().ToString());

        if (exception is UnauthorizedAccessException)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }

        app.Logger.LogError($"{JsonSerializer.Serialize(errorEnvelope)}{ Environment.NewLine}" +
            $"=====================================================================================================================");

        if (app.Environment.IsDevelopment() == false)
        {
            errorEnvelope.ExceptionStackTrace = null;
        }
        await context.Response.WriteAsync(JsonSerializer.Serialize(errorEnvelope));

        //if (exceptionHandlerPathFeature?.Path == "/")
        //{
        //    await context.Response.WriteAsync(" Page: Home.");
        //}
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
