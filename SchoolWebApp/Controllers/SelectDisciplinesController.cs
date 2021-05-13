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
  public class SelectDisciplinesController : Controller
  {
    private readonly ApplicationDbContext _db;
    private readonly UserManager<Student> _userManager;

    public SelectDisciplinesController(ApplicationDbContext db, UserManager<Student> userManager)
    {
      _db = db;
      _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
      Student student = await _db.Users
        .Include(s => s.Disciplines)
        .SingleOrDefaultAsync(s => s.UserName == User.Identity.Name);
      List<Discipline> disciplines = await _db.Disciplines.ToListAsync();

      List<DisciplineViewModel> selectedDisciplines = new List<DisciplineViewModel>();
      foreach (Discipline d in disciplines)
      {
        DisciplineViewModel selectedDiscipline = new DisciplineViewModel
        {
          Discipline = d,
          IsSelected = student.Disciplines.Contains(d)
        };
        selectedDisciplines.Add(selectedDiscipline);
      }
      StudentToDisciplinesViewModel studentToDisciplines = new StudentToDisciplinesViewModel
      {
        Student = student,
        Disciplines = selectedDisciplines
      };
      return View(studentToDisciplines);
    }

    [HttpPost]
    public IActionResult SelectDisciplines(StudentToDisciplinesViewModel studentDisciplines)
    {
      Student student = _db.Users
          .Include(s => s.Disciplines)
          .SingleOrDefault(s => s.Id == studentDisciplines.Student.Id);
      student.Disciplines.Clear();
      foreach (DisciplineViewModel discipline in studentDisciplines.Disciplines)
      {
        if (discipline.IsSelected)
        {
          Discipline newDiscipline = _db.Disciplines.SingleOrDefault(d => d.Id == discipline.Discipline.Id);
          student.Disciplines.Add(newDiscipline);
        }
      }
      _db.SaveChanges();

      return RedirectToAction(nameof(Index));
    }
  }
}
