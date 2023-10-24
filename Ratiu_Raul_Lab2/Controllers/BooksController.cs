using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ratiu_Raul_Lab2.Data;
using Ratiu_Raul_Lab2.Models;

namespace Ratiu_Raul_Lab2.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryContext _context;

        public BooksController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index(string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";


            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            var books = from b in _context.Books
                        select b;

            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Title.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "title_desc":
                    books = books.OrderByDescending(b => b.Title);
                    break;
                case "Price":
                    books = books.OrderBy(b => b.Price);
                    break;
                case "price_desc":
                    books = books.OrderByDescending(b => b.Price);
                    break;
                default:
                    books = books.OrderBy(b => b.Title);

                    break;
            }
            int pageSize = 2;
            var authorFullName = _context.Authors.Select(x => x.LastName + " " + x.FirstName);
            ViewData["AuthorID"] = new SelectList(authorFullName);

            return View(await PaginatedList<Book>.CreateAsync(books.Include(b => b.Author).AsNoTracking(), pageNumber ??
           1, pageSize));

        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var authorFullName = _context.Authors.Select(x => x.LastName + " " + x.FirstName);
            ViewData["AuthorID"] = new SelectList(authorFullName);

            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(s => s.Orders)
                .ThenInclude(e => e.Customer)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (book == null)
            {
                return NotFound();
            }


            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            var authorFullName = _context.Authors.Select(x => x.LastName + " " + x.FirstName);
            ViewData["AuthorID"] = new SelectList(authorFullName);
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Author,Price")] Book book)
        {
            try
            {
                var authorFullName = _context.Authors.Select(x => x.LastName + " " + x.FirstName);
                ViewData["AuthorID"] = new SelectList(authorFullName);
                if (ModelState.IsValid)
                {
                    _context.Add(book);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /* ex*/) { ModelState.AddModelError("", "Unable to save changes. " + "Try again, and if the problem persists "); }

            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var authorFullName = _context.Authors.Select(x => x.LastName + " " + x.FirstName);
            ViewData["AuthorID"] = new SelectList(authorFullName);
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var bookToUpdate = await _context.Books.FirstOrDefaultAsync(s => s.ID == id);
            var authorFullName = _context.Authors.Select(x => x.LastName + " " + x.FirstName);
            ViewData["AuthorID"] = new SelectList(authorFullName);
            if (await TryUpdateModelAsync<Book>(
                bookToUpdate,
                "",
                s => s.Author, s => s.Title, s => s.Price))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists");
                }
            }
            return View(bookToUpdate);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            var authorFullName = _context.Authors.Select(x => x.LastName + " " + x.FirstName);
            ViewData["AuthorID"] = new SelectList(authorFullName);

            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .AsNoTracking()
                .Include(b => b.Author)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (book == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "Delete failed. Try again";
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var authorFullName = _context.Authors.Select(x => x.LastName + " " + x.FirstName);
            ViewData["AuthorID"] = new SelectList(authorFullName);

            if (_context.Books == null)
            {
                return Problem("Entity set 'LibraryContext.Books'  is null.");
            }
            var book = await _context.Books.FindAsync(id);
            if (book == null) { return RedirectToAction(nameof(Index)); }
            try
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {

                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private bool BookExists(int id)
        {
            return (_context.Books?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
