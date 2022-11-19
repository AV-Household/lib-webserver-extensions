using AV.Household.WebClient.Sample.API;

var sampleClient = new SampleClient(new HttpClient(){BaseAddress = new Uri("https://www.boredapi.com")})
    {
        JsonSerializerSettings =
        {
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
        }
    };

var activity = await sampleClient.ActivityAsync(key: 5881028).ConfigureAwait(false);

Console.WriteLine($"Response is {activity.Key} : {activity.Activity}");

