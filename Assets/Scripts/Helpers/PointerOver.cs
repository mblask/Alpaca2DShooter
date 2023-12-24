using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerOver : IPointerOver
{
    private static PointerOver _instance;
    private PointerEventData _eventData;

    private PointerOver()
    {
        _eventData = new PointerEventData(EventSystem.current);
    }

    public static PointerOver GetInstance()
    {
        if (_instance == null)
            _instance = new PointerOver();

        return _instance;
    }

    public bool OverUI()
    {
        int uiLayer = LayerMask.NameToLayer("UI");
        _eventData.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(_eventData, raycastResults);

        foreach (RaycastResult raycastResult in raycastResults)
            if (raycastResult.gameObject.layer.Equals(uiLayer))
                return true;

        return false;
    }
}
