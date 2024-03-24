using Microsoft.AspNetCore.Mvc;
using SimpleProductOrder.Dto;
using SimpleProductOrder.Models;
using SimpleProductOrder.Services;
using AutoMapper;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SimpleProductOrder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        private readonly ProductService _productService;

        private readonly ReviewService _reviewService;

        private readonly IMapper _mapper;

        public ProductController(CategoryService categoryService, IMapper mapper, ProductService productService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Product>))]
        public IActionResult GetProducts()
        {
            var products = _mapper.Map<List<ProductDto>>(_productService.GetProducts());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(products);
        }

        /// <summary>
        /// Get product with selected id
        /// </summary>
        [HttpGet("{productId}")]
        [ProducesResponseType(200, Type = typeof(Product))]
        [ProducesResponseType(400)]
        public IActionResult GetProduct(int productId)
        {
            if (!_productService.ProductExists(productId))
                return NotFound();

            var product = _mapper.Map<ProductDto>(_productService.GetProduct(productId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(product);
        }

        /// <summary>
        /// Get all reviews of selected product
        /// </summary>
        [HttpGet("{productId}/Reviews")]
        [ProducesResponseType(200, Type = typeof(List<Review>))]
        [ProducesResponseType(404)]
        public IActionResult GetProductReviews(int productId)
        {
            var reviews = _productService.GetReviewsOfProduct(productId);

            if (reviews == null || !reviews.Any())
            {
                return NotFound();
            }

            return Ok(reviews);
        }

        /// <summary>
        /// Get all products of selected category
        /// </summary>
        [HttpGet("{categoryId}/Category")]
        [ProducesResponseType(200, Type = typeof(List<Review>))]
        [ProducesResponseType(404)]
        public IActionResult GetProductsWithCategory(int categoryId)
        {
            var products = _productService.GetProductsByCategory(categoryId);

            if (products == null || !products.Any())
            {
                return NotFound();
            }

            return Ok(products);
        }

        /// <summary>
        /// Add product
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/Product
        ///     {
        ///          "name": "string",
        ///          "price": 0,
        ///          "weight": 0,
        ///          "description": "string",
        ///          "qty": 0
        ///     }
        ///
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateProduct([FromQuery] int categoryId, [FromBody] ProductDto productCreate)
        {
            if (productCreate == null)
                return BadRequest(ModelState);

            var category = _productService.GetProducts()
                .Where(c => c.Name.Trim().ToUpper() == productCreate.Name.TrimEnd().ToUpper())
                .FirstOrDefault();

            var id = _categoryService.GetCategory(categoryId);

            if (id == null)
            {
                ModelState.AddModelError("", $"category id:{categoryId} not found");
                return StatusCode(422, ModelState);
            }


            if (category != null)
            {
                ModelState.AddModelError("", "category already exists");
                return StatusCode(422, ModelState);
            }


            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var productMap = _mapper.Map<Product>(productCreate);

            productMap.Category = _categoryService.GetCategory(categoryId);

            if (!_productService.CreateProduct(productMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        /// <summary>
        /// Delete product
        /// </summary>
        [HttpDelete("{productId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteProduct(int productId)
        {
            if (!_productService.ProductExists(productId))
            {
                return NotFound();
            }

            var product = _productService.GetProduct(productId);
            if (product == null)
            {
                ModelState.AddModelError("", "Something went wrong while deleting product");
                return BadRequest(ModelState);
            }

            if (_productService.OrderProductExists(productId) == true)
            {
                ModelState.AddModelError("", "Cannot delete product that is a part of order");
                return BadRequest(ModelState);
            }

            _productService.DeleteProduct(product);

            return Ok("Successfully deleted");

        }

        /// <summary>
        /// Update product details
        /// </summary>
        [HttpPut("{productId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateProduct(int productId, [FromBody] ProductDto productDto)
        {
            if (productDto == null || productId != productDto.Id)
                return BadRequest(ModelState);

            var existingProduct = _productService.GetProduct(productId);

            if (existingProduct == null)
                return NotFound();

            var category = _categoryService.GetCategory(existingProduct.Category.Id);

            if (category == null)
            {
                ModelState.AddModelError("", $"Category with ID {existingProduct.Category.Id} not found");
                return StatusCode(422, ModelState);
            }

            _mapper.Map(productDto, existingProduct);
          
            existingProduct.Category = category;
            
            if (!_productService.UpdateProduct(existingProduct))
            {
                ModelState.AddModelError("", "Something went wrong while updating");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}