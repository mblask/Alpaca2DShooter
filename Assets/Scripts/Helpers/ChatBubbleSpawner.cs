using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatBubbleSpawner : Singleton<ChatBubbleSpawner>
{
    [SerializeField] private Vector2 _bubbleOffset;

    private List<ChatBubble> _bubbles = new List<ChatBubble>();

    private GameAssets _gameAssets;

    private void Start()
    {
        _gameAssets = GameAssets.Instance;
    }

    public void SpawnBubble(Transform parentPosition, string textToDisplay)
    {
        Transform bubble = Instantiate(_gameAssets.NPCPrefab, parentPosition.position, Quaternion.identity, transform);

        ChatBubble chatBubble = bubble.GetComponent<ChatBubble>();

        chatBubble.SetTargetToFollow(parentPosition);
        chatBubble.SetOffset(_bubbleOffset);
        chatBubble.SetupText(textToDisplay);

        AddToBubbleList(chatBubble);
    }

    private void AddToBubbleList(ChatBubble bubble)
    {
        if (bubble == null)
            return;

        ChatBubble bubbleFromSameParent = null;

        foreach (ChatBubble bubbleInList in _bubbles)
        {
            if (bubbleInList.GetTargetToFollow() == bubble.GetTargetToFollow())
                bubbleFromSameParent = bubbleInList;
        }

        if (bubbleFromSameParent != null)
        {
            _bubbles.Remove(bubbleFromSameParent);
            Destroy(bubbleFromSameParent.gameObject);
        }

        _bubbles.Add(bubble);
    }

    public void RemoveBubble(ChatBubble bubble)
    {
        for (int i = 0; i < _bubbles.Count; i++)
        {
            if (_bubbles[i] == bubble)
            {
                _bubbles.RemoveAt(i);
                Destroy(bubble.gameObject);
                i--;
            }
        }
    }
}
