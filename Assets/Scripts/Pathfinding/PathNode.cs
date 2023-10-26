using System.Collections.Generic;

public class PathNode
{
    public int Column;
    public int Row;

    private bool _isObstacle;

    public int Gcost;
    public int HCost;
    public int FCost;

    public PathNode PreviousNode;
    private List<PathNode> _neighbors;

    public PathNode(int column, int row)
    {
        Column = column;
        Row = row;

        _isObstacle = false;

        PreviousNode = null;
        _neighbors = new List<PathNode>();
    }

    public void ToggleObstacle()
    {
        _isObstacle = !_isObstacle;
    }

    public void SetObstacle(bool value)
    {
        _isObstacle = value;
    }

    public bool IsObstacle()
    {
        return _isObstacle;
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
        if (_isObstacle)
            return "=";

        return Column + "," + Row;
    }
}