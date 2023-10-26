using System.Collections.Generic;

public class PathNode
{
    public int Column;
    public int Row;

    public bool IsObstacle;

    public int Gcost;
    public int HCost;
    public int FCost;

    public PathNode PreviousNode;
    private List<PathNode> _neighbors;

    public PathNode(int column, int row)
    {
        Column = column;
        Row = row;

        IsObstacle = false;

        PreviousNode = null;
        _neighbors = new List<PathNode>();
    }

    public void ToggleObstacle()
    {
        IsObstacle = !IsObstacle;
    }

    public void AddNeighbor(PathNode node)
    {
        _neighbors.Add(node);
    }

    public List<PathNode> GetNeighbors()
    {
        return _neighbors;
    }

    public void CalculateFCost()
    {
        FCost = Gcost + HCost;
    }

    public override string ToString()
    {
        if (IsObstacle)
            return "=";

        return Column + "," + Row;
    }
}