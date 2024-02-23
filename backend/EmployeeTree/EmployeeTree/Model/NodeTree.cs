namespace EmployeeTree.Model;

public class NodeTree
{
    public Employee Employee { get; set; }
    public List<NodeTree> Children { get; set; } = new List<NodeTree>();
}