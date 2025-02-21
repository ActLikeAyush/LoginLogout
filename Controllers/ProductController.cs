using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestAPIadminPortal.Data;
using TestAPIadminPortal.Models;
using TestAPIadminPortal.Models.Entites;

namespace TestAPIadminPortal.Controllers
{

    [Route("api/[Controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private ApplicationDbContext dbcontext;

        public ProductController(ApplicationDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }
        [HttpPost("addProduct")]
        public IActionResult AddProduct(Product product)
        {
            var AddProduct = new Product()
            {
                ProductName = product.ProductName,
                ProductDescription=product.ProductDescription,
                ProductImg=product.ProductImg,
                ProductPrice=product.ProductPrice
            };
            dbcontext.Products.Add(AddProduct);
            dbcontext.SaveChanges();

            return Ok(AddProduct);
        }


        [HttpGet("showProduct")]
        public IActionResult showProduct()
        {
            var allProducts = dbcontext.Products
                        .Select(e => new
                                        {
                                            e.ProductName,
                                            e.ProductDescription,
                                            e.ProductImg,
                                            e.ProductPrice
                                           }).ToList();
            return Ok(allProducts);
        }

    }
}
