using IdentityModel.Client;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System.Net.Http.Headers;

var requestUrl = "https://api.nvoconsolidation.com/rates/api/Quotes";
var authority = "https://nvogo.b2clogin.com/tfp/nvogo.onmicrosoft.com/B2C_1_ROPC/";
var scope = "https://nvogo.onmicrosoft.com/rates-api/rates-api";
var apimSubscriptionKey = "<YOUR-SUBSCRIPTION-KEY>";
var clientId = "<YOUR-CLIENT-ID>";
var userName = "<YOUR-USERNAME>";
var password = "<YOUR-PASSWORD>";

// Request Json 
var request = JsonConvert.SerializeObject(
    new
    {
        reference = "REFSAMPLE",
        userName = "APIM DEVELOPER",
        userEmail = "example@systemdev.cloud",
        pickupLocation = new
        {
            countryCode = "NL",
            postalCode = "2992",
            description = "Barendrecht"
        },
        originCode = "NLRTM",
        destinationCode = "SGSIN",
        cargoReadyDate = "2024-08-13T10:00:00.000Z",
        prepaidCollect = "Prepaid",
        dangerousGoods = false,
        exportExaDoc = false,
        packingLines = new object[] {
            new {
              quantity = 1,
              packageTypeCode = "PLT",
              isStackable = false,
              grossWeightKg = 9999.99,
              volumeM3 = 30.0,
              goodsDescription = "COMPUTER SCREENS"
            },
            new {
              quantity = 1,
              packageTypeCode = "BAG",
              isStackable = false,
              grossWeightKg = 999,
              goodsDescription = "CHIPS",
              dimensions = new object[] {
                new {
                    length = 100,
                    width = 100,
                    height = 100
                }
              }
            }
        }
    }
);

Console.WriteLine("Retrieving Access token from Azure Ad B2c");

var scopes = new[] { scope };

var app = PublicClientApplicationBuilder.Create(clientId)
      .WithB2CAuthority(authority)
      .Build();

Console.WriteLine("Initializing HttpClient and setting token");

var apiClient = new HttpClient();
apiClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apimSubscriptionKey);

try
{
    AuthenticationResult result = await app
        .AcquireTokenByUsernamePassword(scopes, userName, password)
        .ExecuteAsync();

    apiClient.SetBearerToken(result.AccessToken);
}
catch (MsalException ex)
{
    Console.Error.WriteLine($"Error acquiring token: {ex.Message}");
}

// call api
HttpResponseMessage response;
using (var content = new StringContent(request))
{
    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
    response = await apiClient.PostAsync(requestUrl, content);
    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine(response.ToString());
    }
    else
    {
        var apiResponse = await response.Content.ReadAsStringAsync();
        Console.WriteLine(apiResponse);
        Console.WriteLine();
        Console.WriteLine("Press any key to exit");
        Console.ReadLine();
    }
}
