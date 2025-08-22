using CSE3200.Domain;
using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using CSE3200.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace CSE3200.Web.Areas.Admin.Controllers
{
    [Area("Admin"),Authorize(Roles ="Admin , Field Representative")]
 
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IProductService productService,
            ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Add()
        {
            return View(new AddProductModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Add(AddProductModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _productService.AddProduct(new Product
                    {
                        Name = model.Name,
                        Category = model.Category,
                        Price = model.Price,
                        Rating = model.Rating,
                        Details = model.Details,
                        Quantity = model.Quantity
                    });

                    TempData["SuccessMessage"] = "Product added successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding product");
                    ModelState.AddModelError("", $"Error adding product: {ex.Message}");
                }
            }
            return View(model);
        }

        public IActionResult Edit(Guid id)
        {
            try
            {
                var product = _productService.GetProduct(id);
                if (product == null)
                {
                    _logger.LogWarning("Product not found with ID: {Id}", id);
                    return NotFound();
                }

                return View(new EditProductModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Category = product.Category,
                    Price = product.Price,
                    Rating = product.Rating,
                    Details = product.Details,
                    Quantity = product.Quantity
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading product for edit with ID: {Id}", id);
                TempData["ErrorMessage"] = "Error loading product details";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(EditProductModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _productService.UpdateProduct(new Product
                    {
                        Id = model.Id,
                        Name = model.Name,
                        Category = model.Category,
                        Price = model.Price,
                        Rating = model.Rating,
                        Details = model.Details,
                        Quantity = model.Quantity
                    });

                    TempData["SuccessMessage"] = "Product updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating product with ID: {Id}", model.Id);
                    ModelState.AddModelError("", $"Error updating product: {ex.Message}");
                }
            }
            return View(model);
        }

        public IActionResult DeleteConfirmation(Guid id)
        {
            try
            {
                var product = _productService.GetProduct(id);
                if (product == null)
                {
                    _logger.LogWarning("Product not found for deletion with ID: {Id}", id);
                    return NotFound();
                }

                return View(new DeleteProductModel
                {
                    Id = product.Id,
                    Name = product.Name
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading product for deletion with ID: {Id}", id);
                TempData["ErrorMessage"] = "Error loading product for deletion";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _productService.DeleteProduct(id);
                TempData["SuccessMessage"] = "Product deleted successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID: {Id}", id);
                TempData["ErrorMessage"] = $"Error deleting product: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));

        }
        [HttpGet]


        [HttpPost]
        public JsonResult GetProductsJson([FromBody] ProductListModel model)
        {
            try
            {
                _logger.LogInformation("Fetching products for DataTables. Page: {PageIndex}, Size: {PageSize}, Search: {SearchValue}",
                    model.PageIndex, model.PageSize, model.Search.Value);

                var (data, total, totalDisplay) = _productService.GetProducts(
                    model.PageIndex,
                    model.PageSize,
                    model.FormatSortExpression("Name", "Category", "Price", "Rating", "Quantity", "Id"),
                    model.Search);

                return Json(new
                {
                    draw = model.Draw,
                    // Important for DataTables
                    recordsTotal = total,
                    recordsFiltered = totalDisplay,
                    data = data.Select(p => new
                    {
                        name = HttpUtility.HtmlEncode(p.Name),
                        category = HttpUtility.HtmlEncode(p.Category),
                        price = p.Price.ToString("C"),
                        rating = p.Rating.ToString("0.0"),
                        quantity = p.Quantity.ToString(),
                        id = p.Id.ToString()
                    }).ToArray()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products for DataTables");
                return Json(DataTables.EmptyResult);
            }
        }
        [HttpGet("CheckStock/{productId}/{quantity}")]
        public JsonResult CheckStock(Guid productId, int quantity)
        {
            try
            {
                var product = _productService.GetProduct(productId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found" });
                }

                var inStock = product.Quantity >= quantity;
                return Json(new
                {
                    success = true,
                    inStock = inStock,
                    available = product.Quantity,
                    message = inStock ? "" : $"Only {product.Quantity} available in stock"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking stock");
                return Json(new { success = false, message = "Error checking stock" });
            }
        }

        [HttpGet("GetPrice/{productId}")]
        public JsonResult GetPrice(Guid productId)
        {
            try
            {
                var product = _productService.GetProduct(productId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found" });
                }
                return Json(new { success = true, price = product.Price });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product price");
                return Json(new { success = false, message = "Error getting price" });
            }
        }
    }
}
