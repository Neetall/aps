using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using ProductionScheduling.Api.Services;
using ProductionScheduling.Application;

var builder = WebApplication.CreateBuilder(args);


// MVC Controller

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

builder.Services.AddOpenApi();

// APS Service
builder.Services.AddProductionScheduling();
builder.Services.AddScoped<ISchedulingService, SchedulingService>();

var app = builder.Build();

app.MapOpenApi();

app.MapGet(
    "/swagger",
    ([FromServices] IWebHostEnvironment environment) =>
        Results.Content(
            """
            <!doctype html>
            <html>
            <head>
                <meta charset="utf-8" />
                <meta name="viewport" content="width=device-width, initial-scale=1" />
                <title>ProductionScheduling API</title>
                <link rel="stylesheet" href="https://unpkg.com/swagger-ui-dist@5/swagger-ui.css" />
                <style>
                    body { margin: 0; background: #ffffff; }
                    .topbar { display: none; }
                </style>
            </head>
            <body>
                <div id="swagger-ui"></div>
                <script src="https://unpkg.com/swagger-ui-dist@5/swagger-ui-bundle.js"></script>
                <script>
                    window.onload = function() {
                        SwaggerUIBundle({
                            url: "/openapi/v1.json",
                            dom_id: "#swagger-ui",
                            deepLinking: true,
                            presets: [
                                SwaggerUIBundle.presets.apis
                            ]
                        });
                    };
                </script>
            </body>
            </html>
            """,
            "text/html"));

app.MapControllers();


app.Run();
