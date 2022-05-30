using Microsoft.AspNetCore.Mvc;
using Polly;

namespace PollyRateLimit.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    /// <summary>
    /// 因為policy的關係, 多個請求需要共用資源, 用key分類限制
    /// </summary>
    private static readonly Policy<IEnumerable<WeatherForecast>>? Policy =
        Polly.Policy.RateLimit<IEnumerable<WeatherForecast>>(1, TimeSpan.FromMinutes(1))
            .WithPolicyKey("key");

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Policy!.Execute(() => WeatherForecasts());
    }

    private static WeatherForecast[] WeatherForecasts()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
}