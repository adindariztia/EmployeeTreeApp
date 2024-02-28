using System.Net.Security;
using EmployeeTree.Model;
using EmployeeTree.Services;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit.Abstractions;

namespace EmployeeTreeTest;

public class EmployeeServiceTest
{
    private readonly ITestOutputHelper _output;
    private IEmployeeService _employeeService;
    private static readonly string _faultyData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Model", "Fixtures", "Faulty-employees.json");
    private static readonly string _correctData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Model", "Fixtures", "Employees.json");
    
    [Theory]
    [InlineData("maurice", true, false)] //employee with no hierarchy
    [InlineData("udinda", true, false)] //no employeeId
    [InlineData("udin", false, true)] //employee with > one manager
    public void GetEmployeeTree_WhenDataIsInvalid_ReturnsFalse(string employeeName, bool isEmpty, bool isManagerFound)
    {
        _employeeService = new EmployeeService(_faultyData);
        
        var result = _employeeService.GetEmployeeTree(employeeName);

        if (isEmpty)
            Assert.True(result.Managers == null || result.Managers.Count == 0);
        Assert.Equal(isManagerFound, result.IsManagerAvailable);
    }

    [Theory]
    [InlineData("raelynn", 0, 2, 7, false)]
    [InlineData("jordana", 2, 0, 0, true)]
    [InlineData("evelina", 3, 0, 0, true)]
    [InlineData("kacie", 1, 3, 1, true)]
    public void GetEmployeeTree_WhenDataValid_ReturnsTrue(string employeeName, int managersCount, int directReportsCount,
        int indirectReportsCount, bool isManagerAvailable)
    {
        _employeeService = new EmployeeService(_correctData);

        var result = _employeeService.GetEmployeeTree(employeeName);
        
        Assert.Equal(isManagerAvailable, result.IsManagerAvailable);
        Assert.Equal(managersCount, result.Managers.Count);
        Assert.Equal(directReportsCount, result.DirectReports?.Count);
        Assert.Equal(indirectReportsCount, result.IndirectReports?.Count);
    }
}