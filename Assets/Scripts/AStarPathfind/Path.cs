using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Path
{
    public int X;
    public int Y;

    private bool _isObstacle = false;

    private List<Path> _neighbors = new List<Path>();
    private Path _previous = null;

    private float _f;
    private float _g = float.MaxValue;
    private float _h;

    private string _textToWrite = "0";

    public bool IsObstacle()
    {
        return _isObstacle;
    }

    public void SetObstacle(bool value)
    {
        _isObstacle = value;
    }

    public void SetXY(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Vector2 GetPosition()
    {
        return new Vector2(X, Y);
    }

    public void SetPrevious(Path previous)
    {
        _previous = previous;
    }

    public Path GetPrevious()
    {
        return _previous;
    }

    public List<Path> GetNeighbors()
    {
        return _neighbors;
    }

    public void AddNeighbor(Path path)
    {
        _neighbors.Add(path);
    }

    public float GetF()
    {
        return _f;
    }

    public void CalculateF()
    {
        _f = _g + _h;
    }

    public float GetG()
    {
        return _g;
    }

    public void SetG(float g)
    {
        _g = g;
    }

    public void SetH(float h)
    {
        _h = h;
    }

    public void TextToShow(string text)
    {
        _textToWrite = text;
    }

    public override string ToString()
    {
        if (_isObstacle)
            return "=";

        return _textToWrite;
    }
}