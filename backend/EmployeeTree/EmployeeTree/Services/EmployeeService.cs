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
    
    public EmployeeDto GetEmployeeTree(string employeeName)
    {
        var managers = new List<Dictionary<int, string>>();
        var message = string.Empty;
        var indirectReports = new List<string>();
        var directReports = new List<string>();

        var currentNode = SearchEmployee(employeeName);
        
        if (currentNode == null)
        {
            return new EmployeeDto { Message = "Employee not found"};
        }

        var employeeId = (currentNode?.Employee.Id) ?? default(int);
        
        // evaluate if employee has multiple managers
        var (isManagerValid, multipleManagerNames) = IsMultipleManagerDetected(employeeId);
        if (!isManagerValid)
        {
            message = $"Unable to process, employee has multiple managers, found: {string.Join(", ", multipleManagerNames)}";
            return new EmployeeDto
            {
                Id = employeeId,
                Name = employeeName,
                IsManagerAvailable = true,
                Message = message
            };
        }
        
        // traverse upwardly to search for employee's managers
        GetManagers(currentNode, managers, employeeId); 
        
        //traverse downwardly to search for employee's direct and indirect reports
        (directReports, indirectReports) = GetReports(currentNode, employeeId, directReports, indirectReports);
        
        var isManagerAvailable = managers.Count > 0;
        
        if (!isManagerAvailable && directReports.Count == 0)
        {
            message = "Employee does not have hierarchy";
        }
        
        var employeeDto = new EmployeeDto
        {
            Id = employeeId,
            Name = currentNode?.Employee?.Name,
            Managers = managers,
            IsManagerAvailable = isManagerAvailable,
            EmployeeLevel = currentNode?.Employee?.EmployeeLevel ?? -1,
            DirectReports = directReports,
            IndirectReports = indirectReports,
            Message = string.IsNullOrEmpty(message) ? $"Request sucessfull" : message
        };
        
        return employeeDto;
    }

    private NodeTree? SearchEmployee(string employeeName)
    {
        var employee = _nodeMap.Where(emp => emp.Value.Employee.Name.Equals(employeeName))
            .Select(emp => emp.Value)
            .FirstOrDefault();
        return employee;
    }

    private (bool, List<string>) IsMultipleManagerDetected(int employeeId)
    {
        var multipleManagersId = _employees.Where(emp => emp.Id == employeeId).Select(emp => emp.ManagerId).ToList();
        var multipleManagerNames = _employees.Where(emp => multipleManagersId.Contains(emp.Id)).Select(emp => emp.Name)
            .ToList();
        
        // employee with more than 1 manager is not permitted
        return (multipleManagerNames.Count is 0 or 1, multipleManagerNames);
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

    private void GetManagers(NodeTree node, List<Dictionary<int, string>> result, int employeeId)
    {
        if (node.Employee.ManagerId != null)
        {
            var managerNode = _nodeMap[node.Employee.ManagerId.Value];
            GetManagers(managerNode, result, employeeId);
        }

        if (node.Employee.Id == employeeId) return;
        
        var employeeNode = new Dictionary<int, string>
        {
            { node.Employee.Id, node.Employee.Name }
        };
        result.Add(employeeNode);
    }

    private (List<string>,List<string>) GetReports(NodeTree node, int employeeId, List<string> directReports, List<string> indirectReports)
    {
        if (node.Employee.Id == employeeId)
        {
            directReports.AddRange(node.Children.Select(c => c.Employee.Name).ToList());
        }

        foreach (var child in node.Children)
        {
            var childNode = _nodeMap[child.Employee.Id];
            indirectReports.AddRange(childNode.Children.Select(c => c.Employee.Name).ToList());
            GetReports(childNode, employeeId, directReports, indirectReports);
        }
        
        return (directReports, indirectReports);
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
}