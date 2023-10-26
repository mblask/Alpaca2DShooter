using System;
using UnityEngine;

public class Grid<T>
{
    private int _columns;
    private int _rows;
    private float _cellSize;
    private Vector3 _origin;

    private T[,] _grid;
    private FloatingTextSingle[,] _gridText;

    public Grid(int columns, int rows, float cellSize, Vector3 origin, Func<int, int, T> generateGridObject = null)
    {
        _columns = columns;
        _rows = rows;
        _cellSize = cellSize;

        _grid = new T[columns, rows];

        _origin = origin;

        if (generateGridObject == null)
            return;

        for (int column = 0; column < _grid.GetLength(0); column++)
        {
            for (int row = 0; row < _grid.GetLength(1); row++)
            {
                _grid[column, row] = generateGridObject(column, row);
            }
        }
    }

    public void SetOrigin(Vector3 origin)
    {
        _origin = origin;
    }

    public void Show()
    {
        clearGridText();
        float duration = 3.0f;
        float fontSize = 3.0f;
        for (int column = 0; column < _grid.GetLength(0); column++)
        {
            for (int row = 0; row < _grid.GetLength(1); row++)
            {
                Vector3 position = GetWorldPosition(column, row) + new Vector3(_cellSize, _cellSize) * 0.5f;
                _gridText[column, row] = FloatingTextSpawnerStatic
                    .Create(position, _grid[column, row].ToString(), Color.white, duration, fontSize);
                Debug.DrawLine(GetWorldPosition(column, row), GetWorldPosition(column, row + 1), Color.white, duration);
                Debug.DrawLine(GetWorldPosition(column, row), GetWorldPosition(column + 1, row), Color.white, duration);
            }
        }
        
        Debug.DrawLine(GetWorldPosition(0, _rows), GetWorldPosition(_columns, _rows), Color.white, duration);
        Debug.DrawLine(GetWorldPosition(_columns, 0), GetWorldPosition(_columns, _rows), Color.white, duration);
    }

    private void clearGridText()
    {
        if (_gridText != null)
            foreach (FloatingTextSingle floatingText in _gridText)
                UnityEngine.Object.Destroy(floatingText.gameObject);

        _gridText = new FloatingTextSingle[_columns, _rows];
    }

    private bool validateGridPosition(int column, int row)
    {
        return column >= 0 && column < _columns && row >= 0 && row < _rows;
    }

    private bool validateGridPosition(Vector2Int xy)
    {
        return validateGridPosition(xy.x, xy.y);
    }

    public Vector3 GetWorldPosition(int column, int row)
    {
        return new Vector3(column, row) * _cellSize + _origin;
    }

    public Vector2Int GetGridPositionFromWorld(Vector3 position)
    {
        return new Vector2Int
        {
            x = Mathf.FloorToInt((position.x - _origin.x) / _cellSize),
            y = Mathf.FloorToInt((position.y - _origin.y) / _cellSize),
        };
    }

    public T GetValue(int column, int row)
    {
        if (!validateGridPosition(column, row))
            return default;

        return _grid[column, row];
    }

    public T GetValue(Vector3 worldPosition)
    {
        Vector2Int xy = GetGridPositionFromWorld(worldPosition);
        Debug.Log(xy);
        if (!validateGridPosition(xy))
            return default;

        return _grid[xy.x, xy.y];
    }

    public void SetValue(int column, int row, T value)
    {
        if (!validateGridPosition(column, row))
            return;

        _grid[column, row] = value;
    }

    public void SetValue(Vector3 worldPosition, T value)
    {
        Vector2Int xy = GetGridPositionFromWorld(worldPosition);
        if (!validateGridPosition(xy))
            return;

        _grid[xy.x, xy.y] = value;
        _gridText[xy.x, xy.y].SetText(value.ToString());
    }

    public int GetRows()
    {
        return _rows;
    }

    public int GetColumns()
    {
        return _columns;
    }
}
