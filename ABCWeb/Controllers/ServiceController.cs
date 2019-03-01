using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Claims;
using System.Threading.Tasks;
using ABCWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ABCWeb.Controllers
{
  [Authorize]
  public class ServiceController : Controller
  {
    private IConfiguration configuration;

    public ServiceController(IConfiguration config)
    {
      configuration = config;
    }

    // GET: Service
    public ActionResult Index()
    {
      return View(new List<ServiceRequest>());
    }

    // GET: Service/Details/5
    public ActionResult Details(int id)
    {
      return View();
    }

    // GET: Service/Create
    public async Task<ActionResult> Create()
    {
      var serviceTypes = await GetServiceTypes();
      if (serviceTypes == null)
        return RedirectToAction("error", "home");
      ViewBag.Types = serviceTypes.Select(c => new SelectListItem(c.Name, c.Id.ToString()));
      ViewBag.Prices = "[" + string.Join(',', serviceTypes.Select(c => $"[{c.Id},{c.Price}]")) + "]";
      return View(new ServiceRequest { Price = serviceTypes[0].Price });
    }

    private async Task<List<ServiceType>> GetServiceTypes()
    {
      List<ServiceType> serviceTypes = new List<ServiceType>();

      string json = null;// = HttpContext.Session.GetString("serviceTypes");
      try
      {
        if (string.IsNullOrEmpty(json))
        {
          var httpClient = new HttpClient();
          var response = await httpClient.GetAsync(configuration.GetValue<string>("Functions:ServiceType"));

          response.EnsureSuccessStatusCode();
          json = await response.Content.ReadAsStringAsync();
        }

        JsonSerializer s = new JsonSerializer();
        var list = s.Deserialize<List<ServiceType>>(new JsonTextReader(new StringReader(json)));

        foreach (var type in list)
        {

          if (!string.IsNullOrEmpty(type.Name) && type.Price != 0
            && type.Id != 0)
          {
            serviceTypes.Add(type);
          }
          else
            throw new InvalidDataException("Error getting types.");
        }
        if (serviceTypes.Count == 0)
          throw new InvalidDataException();

        //HttpContext.Session.SetString("serviceTypes", json);
      }
      catch
      {
        return null;
      }

      return serviceTypes;
    }

    // POST: Service/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(ServiceRequest service)
    {
      try
      {
        var serviceTypes = await GetServiceTypes();
        if (serviceTypes == null)
          return RedirectToAction("error", "home");
        ViewBag.Types = serviceTypes.Select(c => new SelectListItem(c.Name, c.Id.ToString()));
        ViewBag.Prices = "[" + string.Join(',', serviceTypes.Select(c => $"[{c.Id},{c.Price}]")) + "]";

        if (ModelState.GetValidationState("mail") == ModelValidationState.Valid
             && ModelState.GetValidationState("ServiceType") == ModelValidationState.Valid)
        {
          string content = "OK!";
          var httpClient = new HttpClient();
          var userClaims = (ClaimsIdentity)User.Identity;

          string message = $@"{{
  ""userId"": ""{userClaims.FindFirst(ClaimTypes.NameIdentifier).Value}"",
  ""price"": {serviceTypes.First(c => c.Id == service.ServiceType).Price},
  ""clientId"": ""{service.Mail}"",
  ""userName"": ""{userClaims.FindFirst("name").Value}"",
  ""userMail"": ""{userClaims.FindFirst("preferred_username").Value}""
}}";
          var stringContent = new StringContent(message);
          stringContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
          var response = await httpClient.PostAsync(configuration.GetValue<string>("Functions:OrderRequest"),
           stringContent);

          response.EnsureSuccessStatusCode();
          content = await response.Content.ReadAsStringAsync();

          TempData["message"] = content;
          return View("Created");
        }
      }
      catch(Exception ex)
      {
        ModelState.AddModelError("status", "Error sending the request");
      }
      return View(service);
    }

    // GET: Service/Edit/5
    public ActionResult Edit(int id)
    {
      return View();
    }

    // POST: Service/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, IFormCollection collection)
    {
      try
      {
        // TODO: Add update logic here

        return RedirectToAction(nameof(Index));
      }
      catch
      {
        return View();
      }
    }

    // GET: Service/Delete/5
    public ActionResult Delete(int id)
    {
      return View();
    }

    // POST: Service/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, IFormCollection collection)
    {
      try
      {
        // TODO: Add delete logic here

        return RedirectToAction(nameof(Index));
      }
      catch
      {
        return View();
      }
    }
  }
}