
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.DTOs;

public class CreateEmployeeDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Department { get; set; } = string.Empty;

    [Range(typeof(decimal), "0", "1000000000")]
    public decimal Salary { get; set; }
}
