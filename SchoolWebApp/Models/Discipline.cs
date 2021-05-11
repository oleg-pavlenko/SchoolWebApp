using System.Collections.Generic;

namespace SchoolWebApp.Models
{
  public class Discipline
  {
    public int Id { get; set; }
    public string Title { get; set; }

    public List<Student> Students { get; set; }
  }
}
