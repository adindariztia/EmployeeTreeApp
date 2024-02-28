using EmployeeTree.Model;

namespace EmployeeTree.DataTransferObject;

public class EmployeeDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public List<Dictionary<int, string>> Managers { get; set; }
    public bool IsManagerAvailable { get; set; }
    public string? Message { get; set; }
    public List<string>? DirectReports { get; set; }
    public List<string>? IndirectReports { get; set; }
    public int EmployeeLevel { get; set; }
}