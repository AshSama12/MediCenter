using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mediCenter.Data;
using mediCenter.Models;

namespace mediCenter.Controllers
{
    public class DrugsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DrugsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Drugs
        public async Task<IActionResult> Index()
        {
            var drugs = await _context.Drugs
                .Where(d => d.IsActive)
                .OrderBy(d => d.Name)
                .ToListAsync();
            return View(drugs);
        }

        // GET: Drugs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var drug = await _context.Drugs
                .Include(d => d.Prescriptions)
                    .ThenInclude(p => p.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (drug == null) return NotFound();

            return View(drug);
        }

        // GET: Drugs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Drugs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Manufacturer,Category,Price,StockQuantity,Dosage,SideEffects,ExpiryDate")] Drug drug)
        {
            if (ModelState.IsValid)
            {
                drug.CreatedAt = DateTime.UtcNow;
                drug.UpdatedAt = DateTime.UtcNow;
                drug.IsActive = true;
                _context.Add(drug);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Drug added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(drug);
        }

        // GET: Drugs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var drug = await _context.Drugs.FindAsync(id);
            if (drug == null) return NotFound();
            return View(drug);
        }

        // POST: Drugs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Manufacturer,Category,Price,StockQuantity,Dosage,SideEffects,ExpiryDate,CreatedAt,IsActive")] Drug drug)
        {
            if (id != drug.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    drug.UpdatedAt = DateTime.UtcNow;
                    _context.Update(drug);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Drug updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DrugExists(drug.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(drug);
        }

        // GET: Drugs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var drug = await _context.Drugs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (drug == null) return NotFound();

            return View(drug);
        }

        // POST: Drugs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var drug = await _context.Drugs.FindAsync(id);
            if (drug != null)
            {
                drug.IsActive = false; // Soft delete
                drug.UpdatedAt = DateTime.UtcNow;
                _context.Update(drug);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Drug deactivated successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DrugExists(int id)
        {
            return _context.Drugs.Any(e => e.Id == id);
        }
    }
}