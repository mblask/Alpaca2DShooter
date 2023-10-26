using AlpacaMyGames;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestingObject : MonoBehaviour
{
    private Pathfinding _pathfinding;

    private void Start()
    {
        _pathfinding = new Pathfinding(10, 10, 1.0f, Vector2.zero);
        _pathfinding.Show();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mousePosition = Utilities.GetMouseWorldLocation();
            _pathfinding.Find(Vector2.zero, mousePosition);
            List<Vector2> worldPoints = _pathfinding.GetWorldPoints();

            for (int i = 0; i < worldPoints.Count - 1; i++)
            {
                Vector2 current = worldPoints[i];
                Vector2 neighbor = worldPoints[i + 1];
                
                Debug.DrawLine(current, neighbor, Color.white, 10);
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            Vector3 mousePosition = Utilities.GetMouseWorldLocation();
            _pathfinding.GetPath(mousePosition).ToggleObstacle();
            _pathfinding.Show();
        }
    }

    public static void DrawPath(List<Vector2> points)
    {
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector2 current = points[i];
            Vector2 neighbor = points[i + 1];

            Debug.DrawLine(current, neighbor, Color.white, 10);
        }
    }
}
