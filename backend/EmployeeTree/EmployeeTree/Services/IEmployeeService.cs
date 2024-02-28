using EmployeeTree.DataTransferObject;
using EmployeeTree.Model;

namespace EmployeeTree.Services;

public interface IEmployeeService
{
    /// <summary>
    /// Get employee tree up to root manager
    /// </summary>
    /// <param name="employeeName">Name of the employee</param>
    /// <returns>EmployeeDto object</returns>
    EmployeeDto GetEmployeeTree(string employeeName);
    /// <summary>
    /// Get all employees
    /// </summary>
    /// <returns>All employees in app memory</returns>
    List<EmployeeDto> GetEmployees();
}