using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoWellAdvanced.Data;
using DoWellAdvanced.Models;
using Microsoft.AspNetCore.Identity;

namespace DoWellAdvanced.Controllers
{
    public class SpreadsheetsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SpreadsheetsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Spreadsheets
        public async Task<IActionResult> Index(string searchString, int? tagId)
        {
            var spreadsheets = _context.Spreadsheets
                .Include(s => s.SpreadsheetTags)
                    .ThenInclude(st => st.Tag)
                .Include(s => s.User)
                .Where(s => s.IsVisible); // Toon alleen zichtbare items

            // Filter op tag indien opgegeven
            if (tagId.HasValue)
            {
                spreadsheets = spreadsheets.Where(s =>
                    s.SpreadsheetTags.Any(st => st.TagId == tagId));
            }

            // Filter op zoekterm indien opgegeven
            if (!String.IsNullOrEmpty(searchString))
            {
                spreadsheets = spreadsheets.Where(s =>
                    s.Title.Contains(searchString));
            }

            // Voorbereid ViewBag voor dropdown
            ViewBag.Tags = new SelectList(_context.Tags.Where(t => t.IsVisible), "Id", "Name");

            return View(await spreadsheets.ToListAsync());
        }

        // GET: Spreadsheets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spreadsheet = await _context.Spreadsheets
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (spreadsheet == null)
            {
                return NotFound();
            }

            return View(spreadsheet);
        }

        // GET: Spreadsheets/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Spreadsheets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,CreatedAt,IsVisible,UserId")] Spreadsheet spreadsheet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(spreadsheet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", spreadsheet.UserId);
            return View(spreadsheet);
        }

        // GET: Spreadsheets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spreadsheet = await _context.Spreadsheets.FindAsync(id);
            if (spreadsheet == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", spreadsheet.UserId);
            return View(spreadsheet);
        }

        // POST: Spreadsheets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,CreatedAt,IsVisible,UserId")] Spreadsheet spreadsheet)
        {
            if (id != spreadsheet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(spreadsheet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpreadsheetExists(spreadsheet.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", spreadsheet.UserId);
            return View(spreadsheet);
        }

        // GET: Spreadsheets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spreadsheet = await _context.Spreadsheets
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (spreadsheet == null)
            {
                return NotFound();
            }

            return View(spreadsheet);
        }

        // POST: Spreadsheets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var spreadsheet = await _context.Spreadsheets.FindAsync(id);
            if (spreadsheet != null)
            {
                // Soft delete - maak alleen onzichtbaar
                spreadsheet.IsVisible = false;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SpreadsheetExists(int id)
        {
            return _context.Spreadsheets.Any(e => e.Id == id);
        }
    }
}
