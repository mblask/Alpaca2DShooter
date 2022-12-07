using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            //CAUSING PROBLEMS WHEN RESTARTING - BUILDING OBJECTS WHEN NOT NEEDED
            /*
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject newGO = new GameObject();
                    newGO.AddComponent<T>();
                    Debug.Log("Singleton created: " + typeof(T).ToString());
                }
            }
            */

            return _instance;
        }
    }

    public virtual void Awake()
    {
        _instance = this as T;
    }
}
