using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationSystem : MonoBehaviour
{
    [SerializeField] private List<Conversation> _conversationList;
    private int _speechIndex = 0;

    private bool _isConversing = false;

    private void OnEnable()
    {
        NPCStats.OnEnemyDeath += DeactivateSpeechBoxOnNPCDeath;
    }

    private void OnDisable()
    {
        NPCStats.OnEnemyDeath -= DeactivateSpeechBoxOnNPCDeath;
    }

    private void DeactivateSpeechBoxOnNPCDeath(NPCStats NPCStats)
    {
        if (NPCStats.gameObject.Equals(this.gameObject) && _isConversing)
            SpeechBox.DeactivateSpeechBoxSingleStatic();
    }

    public void ActivateConversation()
    {
        _isConversing = true;

        for (int i = 0; i < _conversationList.Count; i++)
        {
            if (!_conversationList[i].IsComplete())
            {
                if (!SpeechBox.IsWriting())
                {
                    //start writing current speech

                    SpeechBox.ActivateSpeechBoxSingleStatic(_conversationList[i].Speech.SpeechList[_speechIndex]);
                    _speechIndex++;

                    if (_speechIndex >= _conversationList[i].Speech.SpeechList.Count)
                    {
                        _speechIndex = 0;

                        _conversationList[i].SetComplete(true);
                    }
                }
                else
                {
                    //interupt current speech and write it completely

                    SpeechBox.WriteCurrentMessageCompletelyStatic();
                }

                return;
            }
        }

        //if speech box is not writing and all conversations are complete, deactivate the speech box
        if (!SpeechBox.IsWriting())
        {
            Debug.Log("All conversations complete");
            SpeechBox.DeactivateSpeechBoxSingleStatic();
            _isConversing = false;

            _conversationList.ForEach(x => x.SetComplete(!x.IsMission));
            _speechIndex = 0;
        }
        //if speech box is writing and all conversations are complete, interupt current speech and write it completely
        else
            SpeechBox.WriteCurrentMessageCompletelyStatic();
    }

    public List<Conversation> GetConversationList()
    {
        return _conversationList;
    }
}
