using AutoMapper;
using backend.Core.Context;
using backend.Core.Dtos;
using backend.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductsController> _logger;
        private readonly IMapper _mapper;

        public ProductsController(ApplicationDbContext dbContext, ILogger<ProductsController> logger, IMapper mapper)
        {
            _context = dbContext;
            _logger = logger;
            _mapper = mapper;
        }

        //Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Please make sure required data is entered correctly", errors = ModelState.Values.SelectMany(v => v.Errors.SelectMany(e => e.ErrorMessage)) });
            }

            //check if category exists with this ID
            var categoryExists = await _context.Categories.AnyAsync(c => c.ID == dto.CategoryID);
            if (!categoryExists)
            {
                return NotFound(new { message = "Relevant Category is not found" });
            }

            Product product = _mapper.Map<Product>(dto);

            try
            {
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product created successfully, ProductName:{product_Name}, ID: {product_ID}", product.Name, product.ID);
                //returns 201 response
                return CreatedAtAction(nameof(ProductsController.GetProduct), new { id = product.ID }, new { message = "Product is Added successfully", productId = product.ID });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "While adding product Database Error occured ");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "While Adding product a Database Error Occured" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "While adding product An Error occured ");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "While Adding product an Error Occured" });
            }
        }
        //Read All Products

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductGetDTO>>> GetAll()
        {
            // You can optimize the get all performance through take and skip methods.

            var dbProducts = await _context.Products.ToListAsync();
            if (!dbProducts.Any())
            {
                return NotFound(new { message = "No Product Found" });
            }
            try
            {
                //convert to ProductGetDTO
                var productsDto = _mapper.Map<List<ProductGetDTO>>(dbProducts);
                _logger.LogInformation("All Products were fetched, Product List:{products}", productsDto.Select(p => p.Name));
                return Ok(new { data = productsDto, message = "Successfully fetched products" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "while fetching Products List Database Error occured products:{Products}", dbProducts.Select(p => p.Name));
                return StatusCode(statusCode: StatusCodes.Status500InternalServerError, new { message = "While fetching the Products List an error occured", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "While fetching Products List An Error occured products:{Products}", dbProducts.Select(p => p.Name));
                return StatusCode(statusCode: StatusCodes.Status500InternalServerError, new { message = "While fetching the products List an error occured", error = ex.Message });
            }
        }
        //Read a product
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductGetDTO>> GetProduct([FromRoute] int id)
        {
            var product = await _context.Products.SingleOrDefaultAsync(p => p.ID == id);
            if (product == null)
            {
                return BadRequest(new { message = "Product not found" });
            }

            try
            {
                var productdto = _mapper.Map<ProductGetDTO>(product);
                _logger.LogInformation("Product requested is fetched, Product Name:{product} and Product Id: {ProductId}", product.Name, product.ID);
                return Ok(new { data = productdto, message = "Product Successfully fetched" });

            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "while fetching the Product, Database Error occured, Product Name:{product} and Product Id: {ProductId}", product.Name, product.ID);
                return StatusCode(statusCode: StatusCodes.Status500InternalServerError, new { message = "Database Error occured while fetching Product", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "while fetching the Product, Database Error occured, , Product Name:{product} and Product Id: {ProductId}", product.Name, product.ID);
                return StatusCode(statusCode: StatusCodes.Status500InternalServerError, new { message = "Database Error occured while fetching Product", error = ex.Message });
            }


        }

        //Update a product
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] ProductCreateDTO dto, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid Data Provided", errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            //check if category exists with this ID
            var categoryExists = await _context.Categories.AnyAsync(c => c.ID == dto.CategoryID);
            if (!categoryExists)
            {
                return NotFound(new { message = "Relevant Category is not found" });
            }

            try
            {
                var productFromDb = await _context.Products.SingleOrDefaultAsync(P => P.ID == id);
                if (productFromDb == null)
                {
                    return NotFound(new { message = "No Product Exists with this Id" });
                }

                _mapper.Map(dto, productFromDb);
                productFromDb.UpdateDate = DateTime.UtcNow;

                _context.Products.Update(productFromDb);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Proudct Updated Successfully Product Id:{ProductId}", productFromDb.ID);
                return Ok(new { message = "Product Successfully Updated", data = productFromDb });

            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "while updating the Product, Database Error occured");
                return StatusCode(statusCode: StatusCodes.Status500InternalServerError, new { message = "While updating the product, A Database error Occured " });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "while updating the Product, Internal Server Error occured");
                return StatusCode(statusCode: StatusCodes.Status500InternalServerError, new { message = "While updating the product, An Internal error Occured " });
            }

        }

        //Delete a product
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not Found" });
            }
            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Product successfully Removed Id: {Id}, Name: {Name}", product.ID, product.Name);
                return Ok(new { message = "Product removed successfully" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation("While removing product Database Error occured, Id: {Id}, Name: {Name}", product.ID, product.Name);
                return StatusCode(statusCode: StatusCodes.Status500InternalServerError, new { message = "While removing product Database Error occured", data = product, error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogInformation("While removing product Database Error occured: {Id}, Name: {Name}", product.ID, product.Name);
                return StatusCode(statusCode: StatusCodes.Status500InternalServerError, new { message = "While removing product, Internal Error occured", data = product, error = ex.Message });
            }
        }
    }
}
