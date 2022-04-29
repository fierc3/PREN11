using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
// using System.DrawingCore.Bitmap; 
public class PlantIdApiHandler
{
    public static async Task<string> SendToPlantIdAsync(byte[] imageArray)
    {

        string url = "https://api.plant.id/v2/identify";
        string api_key = "LYsccirRB9jadHsq8e2j7ddsDOLihh0LiITrGgkKDtu0oa8uLC";

        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(url);

        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("Api-Key", api_key);

        List<string> images = new List<string>();
        //byte[] imageArray = System.IO.File.ReadAllBytes("C:/Users/stefa/OneDrive/Pictures/plant.png");
        string base64ImageRepresentation = Convert.ToBase64String(imageArray);
        images.Add(base64ImageRepresentation);

        JObject payload = new JObject(
            new JProperty("images",
                new JArray(
                    from i in images
                    select new JValue(i)
                )
            ),
            new JProperty("plant_details", new JArray(new JValue("common_names")))
        );

        var stringContent = new StringContent(JsonConvert.SerializeObject(payload, Formatting.Indented));
        var response = await client.PostAsync(url, stringContent);
        var result = await response.Content.ReadAsStringAsync();
        Console.WriteLine(response.ToString());
        Console.WriteLine(result);
        dynamic resultJson = JObject.Parse(result);

        string finalPlant = "";
        float finalProbability = 0;
        foreach (var suggestion in resultJson.suggestions)
        {
            string plant = "unknown";
            if (suggestion.plant_details.common_names.Type == JTokenType.Array)
            {
                plant = suggestion.plant_details.common_names[0];
            }
            else
            {
                plant = suggestion.plant_details.common_names;
            }
            
            float probability = suggestion.probability;


            Console.WriteLine("Plant name: " + plant);
            Console.WriteLine("Probability: " + probability);
            if(probability > finalProbability)
            {
                finalPlant = plant;
                finalProbability = probability;
            }
        }
        Console.WriteLine("FINAL RESULT FOR PLANT IS: " + finalPlant + " | " + finalProbability);
        return finalPlant;
    }

    public static string SendToPlantId(byte[] imageArray)
    {
        var asyncResult = SendToPlantIdAsync(imageArray);
        return asyncResult.GetAwaiter().GetResult();
    }
}
