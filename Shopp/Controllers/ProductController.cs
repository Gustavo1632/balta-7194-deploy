using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shopp.Models;
using Shopp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Shop.Controllers
{
    [Route("products")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get([FromServices] DataContext context)
        {
            var products = await context.Products.Include(x => x.Category).AsNoTracking().ToListAsync();
            return products;

        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetById(int id, [FromServices] DataContext context)
        {
            var product = await context.Products.Include(x => x.Category).AsNoTracking().
                FirstOrDefaultAsync(x => x.Id == id);
            return product;
        }

        [HttpGet]
        [Route("categories/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetByCategory([FromServices] DataContext context, int id)
        {
            var products = await context.Products.Include(x => x.Category).
                AsNoTracking().Where(x => x.CategoryId == id).ToListAsync();
            return products;

        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Product>>> Post([FromBody] Product model,
                                                     [FromServices] DataContext context)
        {
            try
            {
                //Verifica se os dados são validos
                if (ModelState.IsValid)
                {
                    context.Products.Add(model);
                    await context.SaveChangesAsync();
                    return Ok(model);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível criar o Produto" });
            }
        }
    }
}
