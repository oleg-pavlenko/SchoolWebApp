using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolWebApp.Models
{
  public class StudentToDisciplinesViewModel
  {
    public Student Student { get; set; }
    public List<DisciplineViewModel> Disciplines { get; set; }
  }
}
