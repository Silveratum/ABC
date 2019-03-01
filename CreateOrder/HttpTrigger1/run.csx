#r "Newtonsoft.Json"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Data.SqlClient;

public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{
    log.LogInformation("C# HTTP trigger function processed a request.");

    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    dynamic data = JsonConvert.DeserializeObject(requestBody);

    string userId = data.userId;
    string clientId = data.clientId;
    string userName = data.userName;
    string userMail = data.userMail;
    double price = data.price;


    try
    {
        var str = Environment.GetEnvironmentVariable("abc_sql");
        decimal id = 0;
        using (SqlConnection conn = new SqlConnection(str))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO orders values(@userId, @clientId, @userName, @userMail, @price);SELECT @@IDENTITY", conn);
            cmd.Parameters.AddWithValue("userId", userId);
            cmd.Parameters.AddWithValue("clientId", clientId);
            cmd.Parameters.AddWithValue("userName", userName);
            cmd.Parameters.AddWithValue("userMail", userMail);
            cmd.Parameters.AddWithValue("price", price);
            id = (decimal)cmd.ExecuteScalar();
        }
        return (ActionResult)new OkObjectResult($"Hello, {userName} your Order Id is {id}"); 
    }
    catch(Exception ex)
    {
        log.LogError(ex.ToString());
        return new BadRequestObjectResult("Information is wrong or incomplete.");
    }
}
