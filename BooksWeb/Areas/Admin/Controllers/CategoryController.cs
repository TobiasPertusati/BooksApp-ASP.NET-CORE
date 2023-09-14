using Books.DataAccess.Data;
using Books.DataAccess.Repository.IRepository;
using Books.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BooksWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Category> categoriesList = _unitOfWork.Category.GetAll().ToList();
            return View(categoriesList);
        }

        #region Crear
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category newCategory)
        {
            //ACA PUEDO HACER MIS PROPIAS VALIDACIONES
            List<Category> categories = _unitOfWork.Category.GetAll().ToList();
            Category? existe = categories.Where(nombre => nombre.Name == newCategory.Name).FirstOrDefault();
            if (existe != null)
            {
                ModelState.AddModelError("Name", "A category with that name already exists");
            }
            // ACA PREGUNTO SI EL MODELO ES VALIDO...
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(newCategory);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }
        #endregion
        #region Editar
        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id == 0)
                return NotFound();

            Category? modifyCategory = _unitOfWork.Category.Get(c => c.Id == Id);

            if (modifyCategory == null)
                return NotFound();

            return View(modifyCategory);
        }
        [HttpPost]
        public IActionResult Edit(Category modifyCategory)
        {
            //List<Category> categories = _dbContext.Categories.ToList();
            //Category? existe = categories.Where(c => c.Name == modifyCategory.Name).FirstOrDefault();
            //if (existe != null && existe.Id != modifyCategory.Id)
            //{
            //    ModelState.AddModelError("Name", "A category with that name already exists");
            //}
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(modifyCategory);
                _unitOfWork.Save();
                TempData["success"] = "Category updated successfully";

                return RedirectToAction("Index", "Category");
            }
            return View();
        }
        #endregion
        #region Eliminar
        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
                return NotFound();

            Category? deleteCategory = _unitOfWork.Category.Get(c => c.Id == Id);

            if (deleteCategory == null)
                return NotFound();

            return View(deleteCategory);
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? Id)
        {
            Category? category = _unitOfWork.Category.Get(c => c.Id == Id);
            if (category == null)
                return NotFound();

            _unitOfWork.Category.Remove(category);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";

            return RedirectToAction("Index", "Category");
        }
        #endregion
    }
}
