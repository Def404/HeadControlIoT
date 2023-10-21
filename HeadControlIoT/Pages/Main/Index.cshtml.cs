using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExecutingDevice.Pages.Main;

public class Index : PageModel
{
	public void OnGet()
	{
		
	}
	
	public async Task<IActionResult> OnPostRunAllDeviceAsync()
	{

		HttpClient client = new HttpClient();
		var response = client.PostAsync("https://localhost:7023/Gateway/PostRunAllDevices", null);
		return RedirectToPage();
	}
}