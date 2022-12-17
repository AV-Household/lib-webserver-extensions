using AV.Household.WebClient.Sample.API;
using Type = AV.Household.WebClient.Sample.API.Type;

var client = new HttpClient();
client.BaseAddress = new Uri("https://www.boredapi.com");
var sampleClient = new SampleClient(client)
    {
        JsonSerializerSettings =
        {
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
        }
    };

var activity = await sampleClient.ApiActivityAsync(type: Type.Busywork, participants:3).ConfigureAwait(false);

Console.WriteLine($"Response is {activity.Key} : {activity.Activity}");

