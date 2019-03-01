#r "Newtonsoft.Json"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{
    log.LogInformation("C# HTTP trigger function processed a request.");

    System.Text.StringBuilder sb = new System.Text.StringBuilder();
    System.IO.StringWriter sw = new System.IO.StringWriter(sb);
    using (var writer = new Newtonsoft.Json.JsonTextWriter(sw))
    {
    writer.Formatting = Newtonsoft.Json.Formatting.Indented;

    //writer.WriteStartObject();
    writer.WriteStartArray();

    writer.WriteStartObject();
    writer.WritePropertyName("id");
    writer.WriteValue(1);
    writer.WritePropertyName("name");
    writer.WriteValue("Type 1");
    writer.WritePropertyName("price");
    writer.WriteValue(100.23);
    writer.WriteEndObject();

    writer.WriteStartObject();
    writer.WritePropertyName("id");
    writer.WriteValue(2);
    writer.WritePropertyName("name");
    writer.WriteValue("Type 2");
    writer.WritePropertyName("price");
    writer.WriteValue(241.52);
    writer.WriteEndObject();

    writer.WriteStartObject();
    writer.WritePropertyName("id");
    writer.WriteValue(3);
    writer.WritePropertyName("name");
    writer.WriteValue("Type 3");
    writer.WritePropertyName("price");
    writer.WriteValue(520.89);
    writer.WriteEndObject();

    writer.WriteEndArray();
    //writer.WriteEnd();
    }

    return (ActionResult)new OkObjectResult(sb.ToString());
}
