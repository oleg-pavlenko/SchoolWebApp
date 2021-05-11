using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace SchoolWebApp.Models
{
  public class Student : IdentityUser
  {
    public List<Discipline> Disciplines { get; set; }
  }
}
