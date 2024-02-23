using EmployeeTree.DataTransferObject;
using EmployeeTree.Model;
using Newtonsoft.Json;

namespace EmployeeTree.Services;

public class EmployeeService : IEmployeeService
{
    private readonly Dictionary<int, NodeTree> _nodeMap = new Dictionary<int, NodeTree>();
    private List<Employee> _employees = new List<Employee>();

    public EmployeeService(string dataJsonPath)
    {
        OrganizationTree(LoadEmployees(dataJsonPath));
    }
    
    public EmployeeDto GetEmployeeTree(int employeeId)
    {
        var managers = new List<Dictionary<int, string>>();
        var message = string.Empty;
        var currentNode = new NodeTree();
        var multipleManagers = new List<string>();
        var employeeReportsValidity = new EmployeeReports();
        
        if (!_nodeMap.ContainsKey(employeeId))
        {
            return new EmployeeDto { Message = "Employee not found", Managers = managers};
        }
        
        var multipleManagersId = _employees.Where(emp => emp.Id == employeeId).Select(emp => emp.ManagerId).ToList();
        var multipleManagerNames = _employees.Where(emp => multipleManagersId.Contains(emp.Id)).Select(emp => emp.Name)
            .ToList();
        
        //check if employee has multiple managers, if dont then proceed to get valid managers
        if (multipleManagerNames.Count > 1)
        {
            message = $"Unable to process, employee has multiple managers, found: {string.Join(", ", multipleManagerNames)}";
        }
        
        // evaluate if employee has valid reports to data
        currentNode = _nodeMap[employeeId];
        employeeReportsValidity = ValidateEmployeeReportsTo(currentNode.Employee);
        if (!employeeReportsValidity.IsValid)
        {
            message += "\n" + employeeReportsValidity.Message;
        } else
        {
            GetManagers(currentNode, managers, employeeId, multipleManagers);
        }
        
        var isManagerAvailable = managers.Count > 0;
        
        var employeeDto = new EmployeeDto
        {
            Id = employeeId,
            Name = currentNode?.Employee?.Name,
            Managers = managers,
            IsManagerAvailable = isManagerAvailable,
            EmployeeLevel = currentNode?.Employee?.EmployeeLevel ?? -1,
            DirectReportsTo = currentNode?.Employee?.DirectReportsTo,
            IndirectReportsTo = currentNode?.Employee?.IndirectReportsTo,
            Message = string.IsNullOrEmpty(message) ? $"Request sucessfull" : message
        };
        
        return employeeDto;
    }
    
    private void OrganizationTree(List<Employee> employees)
    {
        foreach (var employee in employees)
        {
            var node = new NodeTree { Employee = employee };
            _nodeMap[employee.Id] = node;
        }

        //assign children to its manager node
        foreach (var employee in employees)
        {
            if (employee.ManagerId.HasValue && _nodeMap.TryGetValue(employee.ManagerId.Value, out var managerNode))
            {
                managerNode.Children.Add(new NodeTree() { Employee = employee });
            }
        }
    }

    private void GetManagers(NodeTree node, List<Dictionary<int, string>> result, int employeeId, List<string> multipleManagers)
    {
        if (node.Employee.ManagerId != null)
        {
            var managerNode = _nodeMap[node.Employee.ManagerId.Value];
            GetManagers(managerNode, result, employeeId, multipleManagers);
        }

        if (node.Employee.Id == employeeId) return;
        
        var employeeNode = new Dictionary<int, string>
        {
            { node.Employee.Id, node.Employee.Name }
        };
        result.Add(employeeNode);
    }
    
    public EmployeeReports ValidateEmployeeReportsTo(Employee employee)
    {
        var message = string.Empty;
        var isValid = true;
        var isEmptyDirectReports = (employee.DirectReportsTo == null || employee.DirectReportsTo?.Count == 0);
        
        // Only EmployeeLevel = 1 are allowed to have no manager and direct reports
        if (!employee.ManagerId.HasValue && isEmptyDirectReports && employee.EmployeeLevel != 1)
            (isValid, message) = (false, "Employee needs to either have manager/direct report");

        if (employee.ManagerId.HasValue && !isEmptyDirectReports)
            (isValid, message) = (false, "Employee having managers cannot have direct reports");
        
        return new EmployeeReports { IsValid = isValid, Message = message };
    }
    
    private List<Employee> LoadEmployees(string targetJson)
    {
        var employees = new List<Employee>();

        using (var r = new StreamReader(@targetJson))
        {
            var json = r.ReadToEnd();
            employees = JsonConvert.DeserializeObject<List<Employee>>(json);
            _employees = employees;
            return employees;
        }
    }

    public List<EmployeeDto> GetEmployees()
    {
        return _employees.Select(employee => new EmployeeDto { Id = employee.Id, Name = employee.Name }).ToList();
    }

    public List<EmployeeDto> SearchEmployee(string employeeName)
    {
        var res = _employees
            .Select(employee => new EmployeeDto { Id = employee.Id, Name = employee.Name })
            .Where(employee => employee.Name.Contains(employeeName))
            .ToList();
        return res;
    }
}