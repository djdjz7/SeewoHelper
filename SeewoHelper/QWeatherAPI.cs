using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SeewoHelper;

public static class QWeatherAPI
{
    private static HttpClient _weatherClient = new HttpClient(
        new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.GZip }
    )
    {
        BaseAddress = new Uri("https://devapi.qweather.com/v7/")
    };

    public static async Task<QWeatherResponse<AirQuality>?> GetAirQualityAsync(
        string key,
        string location
    )
    {
        try
        {
            var rawResponse = await (
                await _weatherClient.GetAsync($"air/now?key={key}&location={location}")
            ).EnsureSuccessStatusCode().Content.ReadAsStringAsync();
            if (rawResponse == null)
                return null;
            return JsonSerializer.Deserialize<QWeatherResponse<AirQuality>?>(rawResponse.Trim());
        }
        catch { return null; }
    }

    public static async Task<QWeatherResponse<CurrentWeather>?> GetCurrentWeatherAsync(
        string key,
        string location
    )
    {
        try
        {
            var rawResponse = await (
                await _weatherClient.GetAsync($"weather/now?key={key}&location={location}")
            ).EnsureSuccessStatusCode().Content.ReadAsStringAsync();
            if (rawResponse == null)
                return null;
            return JsonSerializer.Deserialize<QWeatherResponse<CurrentWeather>?>(rawResponse.Trim());
        }
        catch { return null; }
    }
}

public class QWeatherResponse<T>
{
    public string code { get; set; }
    public string updateTime { get; set; }
    public string fxLink { get; set; }
    public T? now { get; set; }
    public Station[]? station { get; set; }
    public Refer? refer { get; set; }
}

public class AirQuality
{
    public string pubTime { get; set; }
    public string aqi { get; set; }
    public string level { get; set; }
    public string category { get; set; }
    public string primary { get; set; }
    public string pm10 { get; set; }
    public string pm2p5 { get; set; }
    public string no2 { get; set; }
    public string so2 { get; set; }
    public string co { get; set; }
    public string o3 { get; set; }
}

public class Refer
{
    public string[] sources { get; set; }
    public string[] license { get; set; }
}

public class Station
{
    public string pubTime { get; set; }
    public string name { get; set; }
    public string id { get; set; }
    public string aqi { get; set; }
    public string level { get; set; }
    public string category { get; set; }
    public string primary { get; set; }
    public string pm10 { get; set; }
    public string pm2p5 { get; set; }
    public string no2 { get; set; }
    public string so2 { get; set; }
    public string co { get; set; }
    public string o3 { get; set; }
}

public class CurrentWeather
{
    public string obsTime { get; set; }
    public string temp { get; set; }
    public string feelsLike { get; set; }
    public string icon { get; set; }
    public string text { get; set; }
    public string wind360 { get; set; }
    public string windDir { get; set; }
    public string windScale { get; set; }
    public string windSpeed { get; set; }
    public string humidity { get; set; }
    public string precip { get; set; }
    public string pressure { get; set; }
    public string vis { get; set; }
    public string cloud { get; set; }
    public string dew { get; set; }
}
