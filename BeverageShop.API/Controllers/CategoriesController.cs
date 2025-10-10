using Microsoft.AspNetCore.Mvc;
using BeverageShop.API.Models;
using BeverageShop.API.Data;

namespace BeverageShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<Category>> GetCategories()
        {
            return Ok(BeverageShopData.GetCategories());
        }
    }
}
