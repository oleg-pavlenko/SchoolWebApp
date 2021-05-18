using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolWebApp.Data;
using SchoolWebApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolWebApp.Controllers
{
  [Authorize]
  public class HomeController : Controller
  {
    private readonly ApplicationDbContext _db;
    private readonly UserManager<Student> _userManager;

    public HomeController(ApplicationDbContext db, UserManager<Student> userManager)
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
      List<Discipline> newDisciplines = new List<Discipline>();

      foreach (var discipline in studentDisciplines.Disciplines)
      {
        if (discipline.IsSelected)
        {
          Discipline disc = student.Disciplines.Where(d => d.Id == discipline.Discipline.Id).SingleOrDefault<Discipline>();
          if (disc == null)
          {
            newDisciplines.Add(disc);
          }
        }
      }
      
      int numOfRecSelDiscs = studentDisciplines.Disciplines.Where(d => d.IsSelected).ToList().Count;
      int numOfStudDisc = student.Disciplines.Count;

      if (numOfRecSelDiscs != numOfStudDisc || newDisciplines.Count > 0)
      {
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

        return View("UpdateSuccess");
      }
      return RedirectToAction(nameof(Index));
    }

    [AllowAnonymous]
    public IActionResult Privacy()
    {
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
