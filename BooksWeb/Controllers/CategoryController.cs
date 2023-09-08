using BooksWeb.Data;
using BooksWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BooksWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public CategoryController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            List<Category> categoriesList = _dbContext.Categories.ToList();
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
            List<Category> categories = _dbContext.Categories.ToList();
            Category? existe = categories.Where(nombre => nombre.Name == newCategory.Name).FirstOrDefault();
            if (existe != null)
            {
                ModelState.AddModelError("Name", "A category with that name already exists");
            }
            // ACA PREGUNTO SI EL MODELO ES VALIDO...
            if (ModelState.IsValid) 
            {
                _dbContext.Categories.Add(newCategory);
                _dbContext.SaveChanges();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index","Category");
            }
            return View();
        }
        #endregion
        #region Editar
        public IActionResult Edit(int? Id) 
        {
            if(Id == null || Id == 0)        
                return NotFound();
            
            Category? modifyCategory = _dbContext.Categories.FirstOrDefault(c => c.Id == Id);

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
                _dbContext.Categories.Update(modifyCategory);
                _dbContext.SaveChanges();
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

            Category? deleteCategory = _dbContext.Categories.FirstOrDefault(c => c.Id == Id);

            if (deleteCategory == null)
                return NotFound();

            return View(deleteCategory);
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? Id)
        {
            Category? category = _dbContext.Categories.Find(Id);
            if (category == null)
                return NotFound();

            _dbContext.Categories.Remove(category);
            _dbContext.SaveChanges();
            TempData["success"] = "Category deleted successfully";

            return RedirectToAction("Index", "Category");
        }
        #endregion
    }
}
