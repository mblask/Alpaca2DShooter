using AlpacaMyGames;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AStarPathfinding
{
    private Grid<Path> _grid;

    private List<Path> _openList = new List<Path>();
    private List<Path> _closedList = new List<Path>();

    [SerializeField] private List<Vector2> _finalPath = new List<Vector2>();

    public void GenerateGrid(Vector3 origin, float cellSize, int columns, int rows)
    {
        _grid = new Grid<Path>(origin, cellSize, columns, rows);

        for (int i = 0; i < _grid.Rows; i++)
        {
            for (int j = 0; j < _grid.Columns; j++)
            {
                Path current = _grid.GetValue(i, j);
                current.SetXY(i, j);
                calculateNeighbors(current);

                if (Utilities.ChanceFunc(30) && !((i == 0 && j == 0) || (i == _grid.Rows - 1 && j == _grid.Columns - 1)))
                    current.SetObstacle(true);
            }
        }
    }

    private void calculateNeighbors(Path current)
    {
        //Up
        Path up = _grid.GetValue(current.X, current.Y + 1);
        if (up != null) current.AddNeighbor(up);

        //UpRight
        Path upRight = _grid.GetValue(current.X + 1, current.Y + 1);
        if (upRight != null) current.AddNeighbor(upRight);

        //Right
        Path right = _grid.GetValue(current.X + 1, current.Y);
        if (right != null) current.AddNeighbor(right);

        //DownRight
        Path downRight = _grid.GetValue(current.X + 1, current.Y - 1);
        if (downRight != null) current.AddNeighbor(downRight);

        //Down
        Path down = _grid.GetValue(current.X, current.Y - 1);
        if (down != null) current.AddNeighbor(down);

        //DownLeft
        Path downLeft = _grid.GetValue(current.X - 1, current.Y - 1);
        if (downLeft != null) current.AddNeighbor(downLeft);

        //Left
        Path left = _grid.GetValue(current.X - 1, current.Y);
        if (left != null) current.AddNeighbor(left);

        //UpLeft
        Path upLeft = _grid.GetValue(current.X - 1, current.Y + 1);
        if (upLeft != null) current.AddNeighbor(upLeft);
    }

    public void Find(Vector2 worldStart, Vector2 worldEnd)
    {
        Vector2Int start = _grid.GetGridPositionFromWorldPosition(worldStart);
        Vector2Int end = _grid.GetGridPositionFromWorldPosition(worldEnd);

        Find(start, end);
    }

    public void Find(Vector2Int start, Vector2Int end)
    {
        Find(start.x, start.y, end.x, end.y);
    }

    public void Find(int starti, int startj, int targeti, int targetj)
    {
        Path start = _grid.GetValue(starti, startj);
        Path goal = _grid.GetValue(targeti, targetj);
        start.SetG(0);
        start.SetH(Vector2.Distance(start.GetPosition(), goal.GetPosition()));
        start.CalculateF();

        _openList.Add(_grid.GetValue(starti, startj));

        int count = 0;
        while (_openList.Count > 0 || count < 1000)
        {
            Path current = getMinFPath();
            if (current == null)
            {
                Debug.Log("Cannot find a way");
                return;
            }

            if (current == goal)
            {
                Debug.Log("Done!");
                reconstructPath(current);
                return;
            }

            if (current.GetNeighbors().Count == 0)
                calculateNeighbors(current);

            _openList.Remove(current);
            _closedList.Add(current);
            foreach (Path neighbor in current.GetNeighbors())
            {
                if (_closedList.Contains(neighbor) || neighbor.IsObstacle())
                    continue;

                float tempG = current.GetG() + Vector2.Distance(neighbor.GetPosition(), current.GetPosition());

                if (tempG < neighbor.GetG())
                {
                    neighbor.SetG(tempG);
                    neighbor.SetH(Vector2.Distance(neighbor.GetPosition(), goal.GetPosition()));
                    neighbor.CalculateF();
                    neighbor.SetPrevious(current);

                    if (!_openList.Contains(neighbor))
                        _openList.Add(neighbor);
                }
            }

            count++;
        }
    }

    private void reconstructPath(Path final)
    {
        final.TextToShow("1");
        _finalPath.Add(_grid.GetWorldPositionFromGridPosition(final.X, final.Y));

        while (final.GetPrevious() != null)
        {
            Path previous = final.GetPrevious();
            previous.TextToShow("1");
            _finalPath.Add(_grid.GetWorldPositionFromGridPosition(previous.X, previous.Y));
            final = final.GetPrevious();
        }

        _finalPath.Reverse();
    }

    private Path getMinFPath()
    {
        if (_openList.Count == 0)
            return null;

        Path min = _openList[0];

        foreach (Path path in _openList)
        {
            if (path.GetF() < min.GetF())
                min = path;
        }

        return min;
    }

    public void Show()
    {
        _grid.ClearGrid();
        _grid.DrawGrid(new DrawSettings { Color = Color.white, FontSize = 4.0f });
    }
}