using Books.DataAccess.Repository.IRepository;
using Books.Models;
using Books.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BooksWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> productsList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();


            return View(productsList);
        }
        //Crear
        public IActionResult Upsert(int? id)
        {
            VMProduct productVM = new()
            {
                ddlCategory = _unitOfWork.Category.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                // create
                return View(productVM);
            }
            else
            {
                // update
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }
        }
        [HttpPost]
        public IActionResult Upsert(VMProduct VMProduct, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                //Si el modelo no es valido lleno el ddl devuelta para no tener un error
                VMProduct.ddlCategory = _unitOfWork.Category.GetAll()
                    .Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    });
                return View(VMProduct);
            }
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string productPath = Path.Combine(wwwRootPath, @"images\product");

                if (!string.IsNullOrEmpty(VMProduct.Product.ImageUrl))
                {
                    var oldImagePath = Path.Combine(wwwRootPath, VMProduct.Product.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                VMProduct.Product.ImageUrl = @"\images\product\" + fileName;
            }
            if (VMProduct.Product.Id == 0)
            {
                _unitOfWork.Product.Add(VMProduct.Product);
                TempData["success"] = "Product added succesfully";
            }
            else
            {
                _unitOfWork.Product.Update(VMProduct.Product);
                TempData["success"] = "Product updated succesfully";
            }
            _unitOfWork.Save();
            return RedirectToAction("Index", "Product");
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> productsList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new {data =  productsList});
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToDelete = _unitOfWork.Product.Get(u=> u.Id == id);
            if (productToDelete == null)
            {
                return Json(new {success =  false, message = "Error while deleting"});
            }
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,productToDelete.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Remove(productToDelete);
            _unitOfWork.Save();
            
            return Json(new {success =  true, message = "Delete Successful"});
        }
        #endregion
    }
}
