using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolWebApp.Data;
using SchoolWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolWebApp.Controllers
{
  [Authorize(Roles = "admin")]
  public class StudentsController : Controller
  {
    private readonly ApplicationDbContext _db;
    private readonly UserManager<Student> _userManager;

    public StudentsController(ApplicationDbContext db, UserManager<Student> userManager)
    {
      _db = db;
      _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
      List<Student> students = await _db.Users
        .Include(s => s.Disciplines)
        .ToListAsync<Student>();
      return View(students);
    }

    public ActionResult Create()
    {
      return View();
    }

    // POST: StudentsController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(Student student)
    {
      if (ModelState.IsValid)
      {
        var res = await _userManager.CreateAsync(student, "123456");
        if (res.Succeeded)
        {
          await _userManager.AddToRoleAsync(student, "student");
          return RedirectToAction(nameof(Index));
        }
        ModelState.AddModelError("UserName", "Student was not created");
      }
      return View();
    }

    [HttpGet]
    public async Task<ActionResult> Delete(string id)
    {
      var student = await _userManager.FindByIdAsync(id);
      return View(student);
    }

    [HttpPost("Students/Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirm(string id)
    {
      var student = await _userManager.FindByIdAsync(id);
      await _userManager.DeleteAsync(student);
      return RedirectToAction(nameof(Index));
    }
  }
}
