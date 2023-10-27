using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using ExecutingDevice;
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
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        
        
        HttpClient client = new HttpClient(clientHandler);
        
        var response = await client.GetAsync($"https://192.168.150.3:44304/Gateway/GetStatus?deviceName={deviceName}");
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

        string dataStr = "";
        using (Aes myAes = Aes.Create())
        {
            AesFunction aesFunction = new AesFunction(myAes.Key, myAes.IV);
            dataStr = aesFunction.DecryptStringFromBytes(deviceData.data);
        }
        _logger.LogInformation($"{DateTime.UtcNow} | {deviceData.name}: {dataStr}");

        return new JsonResult(Ok());
    }
    
    private class DeviceData
    {
        public string name { get; set; }
        public byte[] data { get; set; }
    }
    
    private class DeviceStatus
    {
        public string deviceName { get; set; }
        public string status { get; set; }
    }
}