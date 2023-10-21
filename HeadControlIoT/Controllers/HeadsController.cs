using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace HeadController.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class HeadsController : ControllerBase
{
    private readonly ILogger<HeadsController> _logger;
    private readonly IConfiguration _config;

    public HeadsController(ILogger<HeadsController> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    [HttpGet(Name = "GetDeviceStatus")]
    public async Task<IActionResult> GetStatus(string deviceName)
    {
        HttpClient client = new HttpClient();
        var response = await client.GetAsync($"https://localhost:7023/Gateway/GetStatus?deviceName={deviceName}");
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            var deviceStatus = JsonSerializer.Deserialize<DeviceStatus>(content);
        
            _logger.LogInformation($"{DateTime.UtcNow} | {deviceStatus.deviceName} status: {deviceStatus.status}");
            return new JsonResult(new{deviceStatus.deviceName, deviceStatus.status});
        }
        else
        {
            _logger.LogInformation($"{DateTime.UtcNow} | It was not possible to get the {deviceName} status");
            return new JsonResult(response.StatusCode);
        }
        
    }

    [HttpPost(Name ="PostDeviceData")]
    public async Task<IActionResult> PostDeviceData()
    {
        string body = string.Empty; 
        using (var reader = new StreamReader(Request.Body))
        {
            body = await reader.ReadToEndAsync();
        }
        var deviceData = JsonSerializer.Deserialize<DeviceData>(body);
        _logger.LogInformation($"{DateTime.UtcNow} | {deviceData.name}: {deviceData.data}");

        return new JsonResult(new {});
    }
    
    /*[HttpPost(Name = "PostChangeStatus")]
    public async Task<IActionResult> PostChangeStatus(string deviceName, int status)
    {
        var newStatus = status switch {
            0 => Status.STOP,
            1 => Status.RUN
        };
        
        
        _logger.LogInformation($"{DateTime.UtcNow} | POST status: {deviceName} --> {newStatus}");
        
        return new JsonResult(HttpStatusCode.OK);
    }*/
    
    private class DeviceData
    {
        public string name { get; set; }
        public string data { get; set; }
    }
    
    private class DeviceStatus
    {
        public string deviceName { get; set; }
        public string status { get; set; }
    }
}