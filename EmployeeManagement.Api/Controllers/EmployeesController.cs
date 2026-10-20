
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/employees")]
[Authorize]

public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EmployeeResponseDto>>> GetAll()
    {
        var employees = await _employeeService.GetAllAsync();
        return Ok(employees);
    }

 
[HttpGet("{id:int}")]
public async Task<ActionResult<EmployeeResponseDto>> GetById(int id)
{
    if (id <= 0)
    {
        return BadRequest(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Invalid employee ID",
            Detail = "Employee ID must be greater than zero."
        });
    }

    var employee = await _employeeService.GetByIdAsync(id);

    if (employee is null)
    {
        return NotFound(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Employee not found",
            Detail = $"No employee exists with ID {id}."
        });
    }

    return Ok(employee);
}

    [HttpPost]
    public async Task<ActionResult<EmployeeResponseDto>> Create(
        CreateEmployeeDto request)
    {
        var employee = await _employeeService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = employee.Id },
            employee);
    }
}
