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
                .Include(s => s.User)
                .Include(s => s.SpreadsheetTags)
                    .ThenInclude(st => st.Tag)
                .Where(s => s.IsVisible);

            if (tagId.HasValue)
            {
                spreadsheets = spreadsheets.Where(s =>
                    s.SpreadsheetTags.Any(st => st.TagId == tagId));
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                spreadsheets = spreadsheets.Where(s =>
                    s.Title.Contains(searchString));
            }

            ViewBag.Tags = new SelectList(_context.Tags.Where(t => t.IsVisible), "Id", "Name");
            return View(await spreadsheets.ToListAsync());
        }

        // GET: Spreadsheets/Create
        public IActionResult Create()
        {
            ViewBag.Tags = new MultiSelectList(_context.Tags.Where(t => t.IsVisible), "Id", "Name");
            return View();
        }

        // POST: Spreadsheets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title")] Spreadsheet spreadsheet, int[] selectedTags)
        {
            // Verwijder ModelState checks voor andere properties
            ModelState.Remove("UserId");
            ModelState.Remove("User");
            ModelState.Remove("SpreadsheetTags");

            if (ModelState.IsValid)
            {
                spreadsheet.UserId = _userManager.GetUserId(User);
                spreadsheet.CreatedAt = DateTime.Now;
                spreadsheet.IsVisible = true;
                spreadsheet.SpreadsheetTags = new List<SpreadsheetTag>();

                _context.Add(spreadsheet);
                await _context.SaveChangesAsync();

                if (selectedTags != null)
                {
                    foreach (var tagId in selectedTags)
                    {
                        _context.SpreadsheetTags.Add(new SpreadsheetTag
                        {
                            SpreadsheetId = spreadsheet.Id,
                            TagId = tagId
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Tags = new MultiSelectList(_context.Tags.Where(t => t.IsVisible), "Id", "Name");
            return View(spreadsheet);
        }

        // GET: Spreadsheets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spreadsheet = await _context.Spreadsheets
                .Include(s => s.User)           // Zorg dat User geladen wordt
                .Include(s => s.SpreadsheetTags)
                    .ThenInclude(st => st.Tag)  // Laad ook de tags
                .FirstOrDefaultAsync(s => s.Id == id);

            if (spreadsheet == null)
            {
                return NotFound();
            }

            ViewBag.Tags = new MultiSelectList(
                _context.Tags.Where(t => t.IsVisible),
                "Id",
                "Name",
                spreadsheet.SpreadsheetTags.Select(st => st.TagId)
            );
            return View(spreadsheet);
        }

        // POST: Spreadsheets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title")] Spreadsheet spreadsheet, int[] selectedTags)
        {
            if (id != spreadsheet.Id)
            {
                return NotFound();
            }

            ModelState.Remove("UserId");
            ModelState.Remove("User");
            ModelState.Remove("SpreadsheetTags");

            if (ModelState.IsValid)
            {
                try
                {
                    // Haal de bestaande spreadsheet op met alle gerelateerde data
                    var existingSpreadsheet = await _context.Spreadsheets
                        .Include(s => s.SpreadsheetTags)
                        .FirstOrDefaultAsync(s => s.Id == id);

                    if (existingSpreadsheet == null)
                    {
                        return NotFound();
                    }

                    // Update alleen de titel, behoud andere waarden
                    existingSpreadsheet.Title = spreadsheet.Title;

                    // Update tags
                    _context.SpreadsheetTags.RemoveRange(existingSpreadsheet.SpreadsheetTags);
                    if (selectedTags != null)
                    {
                        foreach (var tagId in selectedTags)
                        {
                            _context.SpreadsheetTags.Add(new SpreadsheetTag
                            {
                                SpreadsheetId = existingSpreadsheet.Id,
                                TagId = tagId
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
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
            }

            ViewBag.Tags = new MultiSelectList(_context.Tags.Where(t => t.IsVisible), "Id", "Name");
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
                .Include(s => s.SpreadsheetTags)
                    .ThenInclude(st => st.Tag)
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
                spreadsheet.IsVisible = false;  // Soft delete
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
