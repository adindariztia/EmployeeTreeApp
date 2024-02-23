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
    [InlineData(1, true, false)] //employee with no manager
    [InlineData(2, true, false)] //no employeeId
    [InlineData(6, false, true)] //employee with > one manager
    public void GetEmployeeTree_WhenDataIsInvalid_ReturnsFalse(int employeeId, bool isEmpty, bool isManagerFound)
    {
        _employeeService = new EmployeeService(_faultyData);
        
        var result = _employeeService.GetEmployeeTree(employeeId);

        if (isEmpty)
            Assert.Empty(result.Managers);
        Assert.Equal(isManagerFound, result.IsManagerAvailable);
    }

    [Theory]
    [InlineData(3, 1)]
    [InlineData(4, 2)]
    [InlineData(10, 3)]
    public void GetEmployeeTree_WhenDataValid_ReturnsTrue(int employeeId, int managersCount)
    {
        _employeeService = new EmployeeService(_correctData);

        var result = _employeeService.GetEmployeeTree(employeeId);
        
        Assert.True(result.IsManagerAvailable);
        Assert.Equal(managersCount, result.Managers.Count);
    }

    [Theory]
    [InlineData(2, null, null)]
    [InlineData(3, 1, new object[] { 2, 3 })]
    public void ValidateEmployeeReportsTo_WhenDataIsInvalid(int level, int? managerId, object[]? directReportsArray)
    {
        _employeeService = new EmployeeService(_faultyData);
        var employee = new Employee
        {
            Id = 1,
            EmployeeLevel = level,
            ManagerId = managerId,
            DirectReportsTo = directReportsArray?.Select(x => (int)x).ToList(),
            Name = "Ramboo"
        };
        
        var result =_employeeService.ValidateEmployeeReportsTo(employee);
        
        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(1, null, null)]
    [InlineData(2, 4, null)]
    [InlineData(2, null, new object[] { 3, 4 })]
    public void ValidateEmployeeReportsTo_WhenDataIsValid(int level, int? managerId, object[]? directReportsArray)
    {
        _employeeService = new EmployeeService(_correctData);

        var employee = new Employee
        {
            Id = 1,
            Name = "Cican",
            EmployeeLevel = level,
            ManagerId = managerId,
            DirectReportsTo = directReportsArray?.Select(x => (int)x).ToList()
        };
        
        var result =_employeeService.ValidateEmployeeReportsTo(employee);

        Assert.True(result.IsValid);
    }
}