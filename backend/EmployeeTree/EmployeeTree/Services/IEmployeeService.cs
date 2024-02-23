using EmployeeTree.DataTransferObject;
using EmployeeTree.Model;

namespace EmployeeTree.Services;

public interface IEmployeeService
{
    /// <summary>
    /// Get employee tree up to root manager
    /// </summary>
    /// <param name="employeeId">Id of the employee</param>
    /// <returns>EmployeeDto object</returns>
    EmployeeDto GetEmployeeTree(int employeeId);
    /// <summary>
    /// Get all employees
    /// </summary>
    /// <returns>All employees in app memory</returns>
    List<EmployeeDto> GetEmployees();
    /// <summary>
    /// Validate if the employee has valid manager/direct reports
    /// An employee needs to either have manager or direct reports (except for level 1 employees)
    /// An employee cannot have both manager and direct reports
    /// </summary>
    /// <param name="employee"></param>
    /// <returns>Returns whether employee reports to is valid or not and its reason if not valid</returns>
    EmployeeReports ValidateEmployeeReportsTo(Employee employee);
    /// <summary>
    /// Search employee based on name
    /// </summary>
    /// <param name="employeeName">name of the employee</param>
    /// <returns>All employees' matching names</returns>
    List<EmployeeDto> SearchEmployee(string employeeName);
}