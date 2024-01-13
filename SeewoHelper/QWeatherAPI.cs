using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SeewoHelper;

public static class QWeatherAPI
{
    public static HttpClient WeatherClient = new HttpClient(new HttpClientHandler()
        {
            AutomaticDecompression = System.Net.DecompressionMethods.GZip 
        })
    { 
        BaseAddress = new Uri("https://devapi.qweather.com/v7/") 
    };
    public static async Task<QWeatherResponse<AirQuality>?> GetAirQuality(string key, string location)
    {
        var rawResponse = await (await WeatherClient.GetAsync($"air/now?key={key}&location={location}")).Content.ReadAsStringAsync();
        if (rawResponse == null)
            return null;
        return JsonSerializer.Deserialize<QWeatherResponse<AirQuality>?>(rawResponse.Trim());
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
