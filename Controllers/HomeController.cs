using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ElMaherQuranSchool.Models;

using Microsoft.EntityFrameworkCore;
using ElMaherQuranSchool.Data;
using Microsoft.AspNetCore.Hosting;

namespace ElMaherQuranSchool.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _hostEnvironment;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
    {
        _logger = logger;
        _context = context;
        _hostEnvironment = hostEnvironment;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalStudents = await _context.Students.CountAsync();
        ViewBag.TotalHalaqas = await _context.Halaqas.CountAsync();
        ViewBag.TotalTeachers = await _context.Teachers.CountAsync();
        ViewBag.TotalMemorizers = await _context.Students.CountAsync(s => s.TotalMemorizedPages > 50);

        ViewBag.Teachers = await _context.Teachers
            .OrderBy(t => t.SortOrder)
            .ThenBy(t => t.Name)
            .ToListAsync();

        ViewBag.Halaqas = await _context.Halaqas
            .Include(h => h.Teacher)
            .Include(h => h.Students)
            .OrderBy(h => h.Name)
            .ToListAsync();

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetParentData(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return Json(new { success = false });

        var parentStudents = await _context.Students
            .Where(s => s.ParentPhone == phone)
            .ToListAsync();

        if (!parentStudents.Any())
            return Json(new { success = false });
        var parentHalaqaIds = parentStudents.Where(s => s.HalaqaId.HasValue).Select(s => s.HalaqaId.GetValueOrDefault()).Distinct().ToList();

        var halaqas = await _context.Halaqas
            .Include(h => h.Teacher)
            .Include(h => h.Students)
                .ThenInclude(s => s.SessionRecords)
                    .ThenInclude(sr => sr.Session)
            .Where(h => parentHalaqaIds.Contains(h.Id))
            .ToListAsync();

        var halaqatList = halaqas.Select(h => {
            var myStudentsInHalaqa = h.Students.Where(s => s.ParentPhone == phone).ToList();
            var myStudentsNames = string.Join(" و ", myStudentsInHalaqa.Select(s => s.Name.Split(' ').FirstOrDefault()));

            return new {
                id = h.Id,
                name = h.Name,
                studentNames = myStudentsNames,
                sheikh = h.Teacher != null ? h.Teacher.Name : "غير محدد",
                students = h.Students.Select(stu => new {
                    id = stu.Id,
                    name = stu.Name,
                    gender = stu.Gender.ToString(),
                    awjoh = stu.TotalMemorizedPages,
                    targetPages = h.TargetPages,
                    points = stu.PointProgress,
                    isMyChild = stu.ParentPhone == phone,
                    attendance = stu.SessionRecords.Any() ? (stu.SessionRecords.Count(r => r.IsPresent) * 100) / stu.SessionRecords.Count : 100,
                    profileImageUrl = stu.ProfileImageUrl,
                    sessions = stu.SessionRecords.OrderByDescending(sr => sr.Session.SessionDate).Take(14).Select(sr => new {
                        date = sr.Session.SessionDate.ToString("yyyy/MM/dd"),
                        isPresent = sr.IsPresent,
                        score = sr.AttendanceScore,
                        memorizationScore = sr.MemorizationScore,
                        note = sr.TeacherNote
                    }).ToList()
                }).ToList()
            };
        }).ToList();

        var names = parentStudents.Select(s => s.Name.Split(' ').FirstOrDefault()).ToList();
        var namesText = names.Count > 1 ? string.Join(" و ", names) : names.FirstOrDefault();
        
        string prefix = "الطلاب";
        if (parentStudents.Count == 1)
        {
            prefix = parentStudents[0].Gender == ElMaherQuranSchool.Models.Gender.Female ? "الطالبة" : "الطالب";
        }
        
        var parentName = $"مرحبا ولي أمر {prefix} ({namesText}) . جعل الله أبناءك من أهل القرآن";
        

        return Json(new {
            success = true,
            parentName = parentName,
            halaqat = halaqatList
        });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SubmitRegistration(string StudentName, int? Age, string gender, string ParentName, string ParentPhone, string LastLevelOfMemorization, string? Address, int? PreferredHalaqaId, IFormFile? ProfileImage)
    {
        if (string.IsNullOrWhiteSpace(StudentName) || string.IsNullOrWhiteSpace(ParentName) || string.IsNullOrWhiteSpace(ParentPhone))
        {
            return Json(new { success = false, message = "يرجى ملء جميع الحقول المطلوبة" });
        }

        // Egyptian phone validation: 11 digits starting with 01
        var cleanPhone = new string(ParentPhone.Where(char.IsDigit).ToArray());
        if (cleanPhone.Length != 11 || !cleanPhone.StartsWith("01"))
        {
            return Json(new { success = false, message = "رقم الهاتف غير صحيح. يجب أن يكون رقم مصري مكون من 11 رقمًا يبدأ بـ 01" });
        }

        ElMaherQuranSchool.Models.Gender parsedGender = gender == "Female" ? ElMaherQuranSchool.Models.Gender.Female : ElMaherQuranSchool.Models.Gender.Male;

        string? imageUrl = null;
        if (ProfileImage != null && ProfileImage.Length > 0)
        {
            string uploadsDir = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "students");
            if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfileImage.FileName);
            string filePath = Path.Combine(uploadsDir, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ProfileImage.CopyToAsync(stream);
            }
            imageUrl = "/uploads/students/" + fileName;
        }

        var request = new RegistrationRequest
        {
            StudentName = StudentName,
            Age = Age,
            Gender = parsedGender,
            ParentName = ParentName,
            ParentPhone = ParentPhone,
            LastLevelOfMemorization = LastLevelOfMemorization ?? string.Empty,
            Address = Address,
            ProfileImageUrl = imageUrl,
            PreferredHalaqaId = PreferredHalaqaId,
            Status = RegistrationStatus.Pending
        };

        _context.RegistrationRequests.Add(request);
        await _context.SaveChangesAsync();

        return Json(new { success = true, message = "تم إرسال طلب التسجيل بنجاح! سنتواصل معك قريبًا." });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
