using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BeverageShop.MVC.Models;
using BeverageShop.MVC.Services;

namespace BeverageShop.MVC.Controllers;

public class HomeController : Controller
{
    private readonly BeverageApiService _beverageService;

    public HomeController(BeverageApiService beverageService)
    {
        _beverageService = beverageService;
    }

    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)] // Cache 60 giây
    public async Task<IActionResult> Index()
    {
        try
        {
            // Get all beverages from API
            var allBeverages = await _beverageService.GetBeveragesAsync();
            
            // Group by category for home page sections
            ViewBag.CoffeeBeverages = allBeverages
                .Where(b => b.Category?.Contains("phê") == true || b.Category?.Contains("Cafe") == true || b.Category?.Contains("Cà phê") == true)
                .Take(6)
                .ToList();
                
            ViewBag.TeaBeverages = allBeverages
                .Where(b => b.Category?.Contains("Trà") == true || b.Category?.Contains("Tea") == true)
                .Take(6)
                .ToList();
                
            ViewBag.JuiceBeverages = allBeverages
                .Where(b => b.Category?.Contains("ép") == true || b.Category?.Contains("Sinh") == true || b.Category?.Contains("Juice") == true)
                .Take(6)
                .ToList();

            return View();
        }
        catch (Exception)
        {
            // If API fails, return view with empty data
            ViewBag.CoffeeBeverages = new List<Beverage>();
            ViewBag.TeaBeverages = new List<Beverage>();
            ViewBag.JuiceBeverages = new List<Beverage>();
            return View();
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
