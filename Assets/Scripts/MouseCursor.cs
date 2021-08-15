using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    private static MouseCursor _instance;
    public static MouseCursor Instance
    {
        get
        {
            return _instance;
        }
    }

    public float StartingCrosshairScale = 1.75f;
    public float CursorModifier = 1.0f;

    #region Singleton
    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }
    #endregion

    private void Start()
    {
        transform.localScale = Vector3.one * StartingCrosshairScale;
    }

    private void Update()
    {
        CursorPosition();

        if (Input.GetKeyDown(KeyCode.Space))
            ChangeCursorSize();
    }

    public void CursorPosition()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition;
    }

    public void ChangeCursorSize()
    {
        Vector3 scale = gameObject.transform.localScale;

        gameObject.transform.localScale = scale * CursorModifier;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, StartingCrosshairScale / 1.5f);
    }
}