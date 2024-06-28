using System.Net;
using Newtonsoft.Json.Linq;
using Revenue.Exceptions;

namespace Revenue.Services;

public class ExchangeRateService
{
    private readonly HttpClient _httpClient;

    public ExchangeRateService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<double> GetExchangeRateAsync(string currencyCode)
    {
        string url = $"http://api.nbp.pl/api/exchangerates/rates/A/{currencyCode}/?format=json";
        HttpResponseMessage response = await _httpClient.GetAsync(url);

        if (response.StatusCode == (HttpStatusCode) 404)
        {
            throw new NotFoundException("Currency with such a code does not exist!");
        }
        
        string responseData = await response.Content.ReadAsStringAsync();
        JObject json = JObject.Parse(responseData);
        double exchangeRate = json["rates"][0]["mid"].Value<double>();

        return exchangeRate;
    }
    
}