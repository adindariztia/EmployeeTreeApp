using EmployeeTree.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeTree.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeeController : Controller
{
    private readonly IEmployeeService _service;

    public EmployeeController(IEmployeeService service)
    {
        _service = service;
    }

    [HttpGet, Route("employees")]
    public IActionResult GetEmployees()
    {
        var result = _service.GetEmployees();
        return Ok(result);
    }
    
    [HttpGet, Route("{employeeName}")]
    public IActionResult GetManagers(string employeeName)
    {
        var result = _service.GetEmployeeTree(employeeName);
        return Ok(result);
    }
}