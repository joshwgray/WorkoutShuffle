using System.Reflection;
using System.Text.Json.Serialization;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    /*app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "WorkoutGenerator API V1");
    });*/

    app.MapScalarApiReference();


}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();