
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Api.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
