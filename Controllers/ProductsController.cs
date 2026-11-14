using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using p48_lessons_asp.Data;
using p48_lessons_asp.Data.Entity;
using p48_lessons_asp.DTO.Product;
using p48_lessons_asp.Models;
using p48_lessons_asp.Models.ProductRequestModels;
using p48_lessons_asp.Services.FluentValidation;
using p48_lessons_asp.Services.Slug;

namespace p48_lessons_asp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IValidator<CreateProductModel> _validator;
        private readonly ISlugService _slugService;
        private readonly IMapper _mapper;

        public ProductsController(
            DataContext dataContext, 
            IValidator<CreateProductModel> validator, 
            ISlugService slugService, 
            IMapper mapper)
        {
            _dataContext = dataContext;
            _validator = validator;
            _slugService = slugService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _dataContext.Products.ToListAsync();
                if (products == null || products.Count == 0)
                {
                    return StatusCode(404, new ResponseModel<object>
                    {
                        StatusCode = 404,
                        Message = "Products not found"
                    });
                }
                return StatusCode(200, new ResponseModel<List<Data.Entity.Product>>
                {
                    StatusCode = 200,
                    Message = "Ok",
                    Data = products
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<object>
                {
                    StatusCode = 500,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequest? model)
        {
            if (model == null)
            {
                return StatusCode(400, new ResponseModel<object>
                {
                    StatusCode = 400,
                    Message = "Product data is required"
                });
            }

            Dictionary<string, string> validationResult = new();
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                validationResult["nameError"] = "Product name is required";
            }
            if (model.Price <= 0)
            {
                validationResult["priceError"] = "Product price must be over zero";
            }
            if (validationResult.Count != 0)
            {
                return StatusCode(400, new ResponseModel<object>
                {
                    StatusCode = 400,
                    Message = "Validation error",
                    Data = validationResult
                });
            }

            try
            {
                var newProduct = new Data.Entity.Product
                {
                    Name = model.Name,
                    Slug = _slugService.GetSlug(model.Name),
                    Price = model.Price,
                    Description = model.Description,
                    CategoryId = model.CategoryId
                };

                _dataContext.Products.Add(newProduct);
                await _dataContext.SaveChangesAsync();

                
                if (model.productImage != null)
                {
                    var file = model.productImage;
                    var productImage = new ProductImage
                    {
                        Url = file.FileName,
                        ProductId = newProduct.Id
                    };

                    _dataContext.ProductImages.Add(productImage);
                    await _dataContext.SaveChangesAsync();
                }

                return StatusCode(201, new ResponseModel<Data.Entity.Product>
                {
                    StatusCode = 201,
                    Message = "Product successfully created with image",
                    Data = newProduct
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<object>
                {
                    StatusCode = 500,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }
    }
}
