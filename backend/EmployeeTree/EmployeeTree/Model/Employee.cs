namespace EmployeeTree.Model;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? ManagerId { get; set; }
    public List<int>? DirectReportsTo { get; set; }
    public List<int>? IndirectReportsTo { get; set; }
    public int EmployeeLevel { get; set; }
}