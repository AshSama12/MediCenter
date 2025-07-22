using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mediCenter.Data;
using mediCenter.Models;

namespace mediCenter.Controllers
{
    public class PrescriptionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrescriptionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Prescriptions
        public async Task<IActionResult> Index(string searchPhone)
        {
            var prescriptions = _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Drug)
                .Include(p => p.Doctor)
                .OrderByDescending(p => p.PrescribedDate)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchPhone))
            {
                prescriptions = prescriptions.Where(p => p.Patient.PhoneNumber.Contains(searchPhone));
            }

            return View(await prescriptions.ToListAsync());
        }

        // GET: Prescriptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Drug)
                .Include(p => p.Doctor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (prescription == null) return NotFound();

            return View(prescription);
        }

        // Add this method to improve the display in dropdowns
        private async Task<int> GetDefaultDoctorIdAsync()
        {
            var defaultDoctor = await _context.Users.FirstOrDefaultAsync(u => u.Role == "Doctor");
            return defaultDoctor?.Id ?? 1; // Return 1 if no doctor found (you should have at least one doctor)
        }

        private void PopulateDropdowns(Prescription prescription = null)
        {
            ViewData["PatientId"] = new SelectList(
                _context.Patients.Select(p => new
                {
                    Id = p.Id,
                    FullName = p.FirstName + " " + p.LastName + " (" + p.PhoneNumber + ")"
                }),
                "Id",
                "FullName",
                prescription?.PatientId
            );

            var drugs = _context.Drugs.Where(d => d.IsActive).Select(d => new
            {
                Id = d.Id,
                NameWithManufacturer = d.Name + " (" + d.Manufacturer + ") - LKR " + d.PackPrice.ToString("N2"),
                PackPrice = d.PackPrice,
                PackType = d.PackType
            }).ToList();

            ViewData["DrugId"] = new SelectList(drugs, "Id", "NameWithManufacturer", prescription?.DrugId);

            // For JavaScript calculations
            ViewData["DrugData"] = drugs.Select(d => new
            {
                id = d.Id,
                packPrice = d.PackPrice,
                packType = d.PackType
            }).ToList();
        }

        // GET: Prescriptions/Create
        public IActionResult Create(int? patientId)
        {
            PopulateDropdowns();

            var prescription = new Prescription();
            if (patientId.HasValue)
            {
                prescription.PatientId = patientId.Value;
                ViewBag.PatientName = _context.Patients
                    .Where(p => p.Id == patientId.Value)
                    .Select(p => p.FirstName + " " + p.LastName)
                    .FirstOrDefault();
            }

            return View(prescription);
        }

        // POST: Prescriptions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,DrugId,Dosage,Instructions,Quantity,Notes")] Prescription prescription)
        {
            if (ModelState.IsValid)
            {
                // Auto-assign doctor and dates
                prescription.DoctorId = await GetDefaultDoctorIdAsync();
                prescription.PrescribedDate = DateTime.UtcNow;
                prescription.CreatedAt = DateTime.UtcNow;
                prescription.IsCompleted = false;

                _context.Add(prescription);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Prescription created successfully!";
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdowns(prescription);
            return View(prescription);
        }

        // GET: Prescriptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null) return NotFound();

            PopulateDropdowns(prescription);
            return View(prescription);
        }

        // POST: Prescriptions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PatientId,DrugId,Dosage,Instructions,Quantity,Notes,IsCompleted,DoctorId,PrescribedDate,CreatedAt")] Prescription prescription)
        {
            if (id != prescription.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prescription);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Prescription updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrescriptionExists(prescription.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdowns(prescription);
            return View(prescription);
        }

        private bool PrescriptionExists(int id)
        {
            return _context.Prescriptions.Any(e => e.Id == id);
        }
    }
}