using AlpacaMyGames;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblesManager : MonoBehaviour
{
    private static CollectiblesManager _instance;
    public static CollectiblesManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField] private List<Item> _unlockedCollectibles = new List<Item>();

    private void Awake()
    {
        if (_instance != null)
            Destroy(_instance.gameObject);

        _instance = this;
        DontDestroyOnLoad(this);
    }

    public void UnlockCollectible(Item item)
    {
        _unlockedCollectibles.AddIfNone(item);
    }

    public List<Item> GetUnlockedCollectibles()
    {
        return _unlockedCollectibles;
    }
}
