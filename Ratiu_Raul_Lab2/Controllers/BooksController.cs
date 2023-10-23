﻿using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Index()
        {
            var libraryContext = _context.Books.Include(b => b.Author);
            var authorFullName = _context.Authors.Select(x => x.LastName + " " + x.FirstName);
            ViewData["AuthorID"] = new SelectList(authorFullName);
            return View(await libraryContext.ToListAsync());
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
        public async Task<IActionResult> Create([Bind("ID,AuthorID,Title,Price")] Book book)
        {
            var authorFullName = _context.Authors.Select(x => x.LastName + " " + x.FirstName);
            ViewData["AuthorID"] = new SelectList(authorFullName);
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,AuthorID,Title,Price")] Book book)
        {
            var authorFullName = _context.Authors.Select(x => x.LastName + " " + x.FirstName);
            ViewData["AuthorID"] = new SelectList(authorFullName);
            if (id != book.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var authorFullName = _context.Authors.Select(x => x.LastName + " " + x.FirstName);
            ViewData["AuthorID"] = new SelectList(authorFullName);

            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Author)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (book == null)
            {
                return NotFound();
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
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return (_context.Books?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
