using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mediCenter.Data;
using mediCenter.Models;

namespace mediCenter.Controllers
{
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Patients
        public async Task<IActionResult> Index(string searchPhone)
        {
            var patients = from p in _context.Patients.Include(p => p.Prescriptions)
                           select p;

            if (!string.IsNullOrEmpty(searchPhone))
            {
                patients = patients.Where(p => p.PhoneNumber.Contains(searchPhone));
            }

            return View(await patients.OrderBy(p => p.LastName).ToListAsync());
        }

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.Drug)
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.Doctor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (patient == null) return NotFound();

            return View(patient);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Patients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,BirthDate,PhoneNumber,Email,Address,Gender,BloodType,Allergies")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    patient.CreatedAt = DateTime.UtcNow;
                    patient.UpdatedAt = DateTime.UtcNow;
                    _context.Add(patient);

                    // Check if context is saving
                    var result = await _context.SaveChangesAsync();
                    Console.WriteLine($"Rows affected: {result}");

                    TempData["Success"] = "Patient created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving patient: {ex.Message}");
                    TempData["Error"] = $"Error saving patient: {ex.Message}";
                }
            }

            // Debug: Log validation errors
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }

                // Add validation errors to TempData for display
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["Error"] = $"Validation errors: {errors}";
            }

            return View(patient);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        // POST: Patients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,BirthDate,PhoneNumber,Email,Address,Gender,BloodType,Allergies,CreatedAt")] Patient patient)
        {
            if (id != patient.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    patient.UpdatedAt = DateTime.UtcNow;
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Patient updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null) return NotFound();

            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Patient deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // Add this to PatientsController for testing
        public async Task<IActionResult> TestDb()
        {
            try
            {
                var count = await _context.Patients.CountAsync();
                return Content($"Database connected. Patient count: {count}");
            }
            catch (Exception ex)
            {
                return Content($"Database error: {ex.Message}");
            }
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }
    }
}