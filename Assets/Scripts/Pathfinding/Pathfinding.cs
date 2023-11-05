using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding
{
    private const int STRAIGHT_COST = 10;
    private const int DIAGONAL_COST = 14;

    private Grid<PathNode> _grid;
    private float _cellSize;

    private List<PathNode> _openList;
    private List<PathNode> _closedList;

    private List<Vector2> _worldPoints;

    public Pathfinding(Vector3 start, Vector3 end, float cellSize)
    {
        int columns = Mathf.Abs(Mathf.FloorToInt((start.x - end.x) / cellSize)) + 5;
        int rows = Mathf.Abs(Mathf.FloorToInt((start.y - end.y) / cellSize)) + 5;

        _grid = new Grid<PathNode>(columns, rows, cellSize, Vector3.zero, 
            (int column, int row) => new PathNode(column, row));
        _cellSize = cellSize;
        _worldPoints = new List<Vector2>();

        calculateAllNeighbors();

        CalculateOrigin(start, end);
    }

    public Pathfinding(int columns, int rows, float cellSize, Vector3 origin)
    {
        _grid = new Grid<PathNode>(columns, rows, cellSize, origin,
            (int column, int row) => new PathNode(column, row));
        _cellSize = cellSize;
        _worldPoints = new List<Vector2>();

        calculateAllNeighbors();
    }

    public void Show()
    {
        _grid.Show();
    }

    public void ObstaclesInPathNode(PathNode node)
    {
        node.SetObstacle(false);
        Vector3 position = _grid.GetWorldPosition(node.Column, node.Row);
        Collider2D[] hits = Physics2D.OverlapBoxAll(position, new Vector2(_cellSize, _cellSize), 0.0f);
        if (hits.Length == 0)
            return;

        foreach (Collider2D hit in hits)
        {
            if (hit.isTrigger)
                continue;

            if (hit.GetComponent<TilemapCollider2D>() != null)
                node.SetObstacle(true);

            if (hit.GetComponent<BaseCollider>() != null)
                node.SetObstacle(true);
        }
    }

    public void CalculateOrigin(Vector3 start, Vector3 end)
    {
        Vector2 origin;
        if (end.y > start.y)
            if (end.x > start.x)
                origin = new Vector2(start.x, start.y);
            else
                origin = new Vector2(end.x, start.y);
        else
            if (end.x > start.x)
                origin = new Vector2(start.x, end.y);
            else
                origin = new Vector2(end.x, end.y);

        _grid.SetOrigin(origin);
    }

    public PathNode GetPathNode(Vector3 worldPosition)
    {
        return _grid.GetValue(worldPosition);
    }

    public List<Vector2> GetWorldPoints()
    {
        return _worldPoints;
    }

    public List<PathNode> Find(Vector3 startPosition, Vector3 endPosition)
    {
        CalculateOrigin(startPosition, endPosition);

        Vector2Int start = _grid.GetGridPositionFromWorld(startPosition);
        Vector2Int end = _grid.GetGridPositionFromWorld(endPosition);

        return Find(start.x, start.y, end.x, end.y);
    }

    public List<PathNode> Find(int startColumn, int startRow, int endColumn, int endRow)
    {
        PathNode start = _grid.GetValue(startColumn, startRow);
        PathNode end = _grid.GetValue(endColumn, endRow);

        if (start == null || end == null)
            return new List<PathNode>();

        _openList = new List<PathNode> { start };
        _closedList = new List<PathNode>();

        for (int column = 0; column < _grid.GetColumns(); column++)
        {
            for (int row = 0; row < _grid.GetRows(); row++)
            {
                PathNode node = _grid.GetValue(column, row);
                node.Gcost = int.MaxValue;
                node.CalculateFCost();
                node.PreviousNode = null;
            }
        }

        start.Gcost = 0;
        start.HCost = calculateDistance(start, end);
        start.CalculateFCost();

        while (_openList.Count > 0)
        {
            PathNode current = getPathNodeWithMinFCost();

            if (current == end)
            {
                return reconstructPath(current);
            }

            _openList.Remove(current);
            _closedList.Add(current);
            foreach (PathNode neighbor in current.GetNeighbors())
            {
                if (_closedList.Contains(neighbor))
                    continue;

                ObstaclesInPathNode(neighbor);

                if (neighbor.IsObstacle())
                    continue;

                int tentativeG = current.Gcost + calculateDistance(current, neighbor);
                if (tentativeG < neighbor.Gcost)
                {
                    neighbor.PreviousNode = current;
                    neighbor.Gcost = tentativeG;
                    neighbor.HCost = calculateDistance(neighbor, end);
                    neighbor.CalculateFCost();

                    if (!_openList.Contains(neighbor))
                        _openList.Add(neighbor);
                }
            }
        }

        return new List<PathNode>();
    }

    private void constructWorldPoints(List<PathNode> path)
    {
        _worldPoints = new List<Vector2>();

        foreach (PathNode node in path)
        {
            Vector2 worldPosition =
                _grid.GetWorldPosition(node.Column, node.Row) + new Vector3(_cellSize, _cellSize) * 0.5f;
            _worldPoints.Add(worldPosition);
        }
    }

    private List<PathNode> reconstructPath(PathNode endNode)
    {
        List<PathNode> finalPath = new List<PathNode> { endNode };

        PathNode current = endNode.PreviousNode;
        while (current != null)
        {
            finalPath.Add(current);
            current = current.PreviousNode;
        }

        finalPath.Reverse();

        constructWorldPoints(finalPath);

        return finalPath;
    }

    private void calculateAllNeighbors()
    {
        for (int row = 0; row < _grid.GetRows(); row++)
        {
            for (int column = 0; column < _grid.GetColumns(); column++)
            {
                calculateNeighbors(_grid.GetValue(column, row));
            }
        }
    }

    private void calculateNeighbors(PathNode node)
    {
        
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                PathNode neighbor = _grid.GetValue(node.Column + i, node.Row + j);
                if (neighbor != null) node.AddNeighbor(neighbor);
            }
        }
    }

    private PathNode getPathNodeWithMinFCost()
    {
        PathNode minNode = _openList[0];
        foreach (PathNode node in _openList)
        {
            if (node.FCost < minNode.FCost)
                minNode = node;
        }

        return minNode;
    }

    private int calculateDistance(PathNode start, PathNode end)
    {
        int dColumn = Mathf.Abs(start.Column - end.Column);
        int dRow = Mathf.Abs(start.Row - end.Row);
        int remaining = Mathf.Abs(dRow - dColumn);
        return DIAGONAL_COST * Mathf.Min(dRow, dColumn) + STRAIGHT_COST * remaining;
    }
}