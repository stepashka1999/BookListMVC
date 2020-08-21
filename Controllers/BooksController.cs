using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForBeginersMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForBeginersMVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        [BindProperty]
        public Book Book { get; set; }
        public BooksController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            if (id == null) Book = new Book();
            else Book = await _dbContext.Books.FindAsync(id);

            if (Book == null) return NotFound();

            return View(Book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert()
        {
            if(!ModelState.IsValid)
            {
                return View(Book);
            }

            if(Book.Id == 0)
            {
                await _dbContext.Books.AddAsync(Book);
            }
            else
            {
                _dbContext.Books.Update(Book);
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        #region API Calls

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _dbContext.Books.ToListAsync()});
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var bookToDelete = await _dbContext.Books.FindAsync(id);
            if(bookToDelete == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            _dbContext.Books.Remove(bookToDelete);
            await _dbContext.SaveChangesAsync();

            return Json(new { success = true, message = "Delete successful"});
        }


        #endregion
    }
}
