using Microsoft.AspNetCore.Mvc;

namespace Life.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        _logger.LogInformation("WeatherForecast endpoint called at {Time}", DateTime.Now);
        
        var forecasts = Enumerable.Range(1, 5).Select(index => {
            var forecast = new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            };
            
            _logger.LogDebug("Generated forecast: {Date}, {Temp}°C, {Summary}", 
                forecast.Date, forecast.TemperatureC, forecast.Summary);
                
            return forecast;
        }).ToArray();

        _logger.LogInformation("Returning {Count} weather forecasts", forecasts.Length);
        return forecasts;
    }
}
