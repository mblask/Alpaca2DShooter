using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Grid<T> where T : new()
{
    public Vector3 Origin { get; }
    public float CellSize { get; }
    public int Columns { get; }
    public int Rows { get; }

    [SerializeField] private List<T> _elements = new List<T>();

    private List<FloatingTextSingle> _gridText = new List<FloatingTextSingle>();

    public Grid(Vector3 origin, float cellSize, int columns, int rows)
    {
        Origin = origin;
        CellSize = cellSize;
        Columns = columns;
        Rows = rows;

        _elements = new List<T>();
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                _elements.Add(new T());
            }
        }
    }

    public Grid(Vector3 origin, Vector3 target, float cellSize)
    {
        Origin = origin;
        CellSize = cellSize;
        Columns = Mathf.FloorToInt(Mathf.Abs(target.x - origin.x) / cellSize) * 3;
        Rows = Mathf.FloorToInt(Mathf.Abs(target.y - origin.y) / cellSize) * 3;

        _elements = new List<T>();
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                _elements.Add(new T());
            }
        }
    }

    public void FillGrid(T value)
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                _elements[getGridIndex(i, j)] = value;
            }
        }
    }

    public void DrawGrid(DrawSettings settings)
    {
        int duration = 99;
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                Vector3 horizontal1 = Origin + new Vector3(j, i) * CellSize - new Vector3(Columns / 2.0f, Rows / 2.0f);
                Vector3 horizontal2 = Origin + new Vector3(j, i + 1) * CellSize - new Vector3(Columns / 2.0f, Rows / 2.0f);
                Debug.DrawLine(horizontal1, horizontal2, settings.Color, duration);

                Vector3 vertical1 = Origin + new Vector3(j, i) * CellSize - new Vector3(Columns / 2.0f, Rows / 2.0f);
                Vector3 vertical2 = Origin + new Vector3(j + 1, i) * CellSize - new Vector3(Columns / 2.0f, Rows / 2.0f);
                Debug.DrawLine(vertical1, vertical2, settings.Color, duration);
            }
        }

        Debug.DrawLine
            (Origin + new Vector3(0, Rows) * CellSize - new Vector3(Columns / 2.0f, Rows / 2.0f), Origin + new Vector3(Columns, Rows) * CellSize - new Vector3(Columns / 2.0f, Rows / 2.0f), settings.Color, duration);
        Debug.DrawLine
            (Origin + new Vector3(Columns, 0) * CellSize - new Vector3(Columns / 2.0f, Rows / 2.0f), Origin + new Vector3(Columns, Rows) * CellSize - new Vector3(Columns / 2.0f, Rows / 2.0f), settings.Color, duration);

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                Vector2 position = Origin + new Vector3(j, i) * CellSize - new Vector3(Columns / 2.0f, Rows / 2.0f) + Vector3.one * CellSize * 0.5f;
                _gridText.Add(FloatingTextSpawner.CreateFloatingTextStatic
                    (position, _elements[getGridIndex(j, i)].ToString(), settings.Color, duration, settings.FontSize, 0.0f, false));
            }
        }
    }

    public void ClearGrid()
    {
        foreach (FloatingTextSingle floatingText in _gridText)
            UnityEngine.Object.Destroy(floatingText.gameObject);

        _gridText.Clear();
    }

    public void DrawGrid()
    {
        DrawGrid(new DrawSettings { Color = Color.white, FontSize = 4.0f });
    }

    private int getGridIndex(int i, int j)
    {
        return i + j * Columns;
    }

    public Vector2Int GetGridPositionFromWorldPosition(Vector2 worldPosition)
    {
        int i = Mathf.FloorToInt((worldPosition.x - Origin.x) / CellSize + Columns * 0.5f);
        int j = Mathf.FloorToInt((worldPosition.y - Origin.y) / CellSize + Rows * 0.5f);

        return new Vector2Int(i, j);
    }

    public Vector2 GetWorldPositionFromGridPosition(int i, int j)
    {
        float x = Origin.x + (i - (Columns - 1) * 0.5f) * CellSize;
        float y = Origin.y + (j - (Rows - 1) * 0.5f) * CellSize;

        return new Vector2(x, y);
    }

    private bool validatePosition(int i, int j)
    {
        return i >= 0 && i < Columns && j >= 0 && j < Rows;
    }

    public T GetValue(int i, int j)
    {
        if (!validatePosition(i, j))
            return default;

        return _elements[getGridIndex(i, j)];
    }

    public T GetValue(Vector2Int gridPosition)
    {
        if (!validatePosition(gridPosition.x, gridPosition.y))
            return default;

        return _elements[getGridIndex(gridPosition.x, gridPosition.y)];
    }

    public T GetValueFromWorld(Vector2 worldPosition)
    {
        Vector2Int indices = GetGridPositionFromWorldPosition(worldPosition);
        return GetValue(indices.x, indices.y);
    }

    public void SetValue(int i, int j, T value)
    {
        if (i < 0 || i >= Columns || j < 0 || j >= Rows)
            return;

        _elements[getGridIndex(i, j)] = value;
    }

    public void SetValueFromWorld(Vector2 worldPosition, T value)
    {
        Vector2Int indices = GetGridPositionFromWorldPosition(worldPosition);
        SetValue(indices.x, indices.y, value);
    }
}