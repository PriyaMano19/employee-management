
using EmployeeManagement.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Data;

public class EmployeeDbContext : IdentityDbContext<ApplicationUser>
{
    public EmployeeDbContext(
        DbContextOptions<EmployeeDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
}
