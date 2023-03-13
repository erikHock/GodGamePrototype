using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScritableObjects/QuestSO")]
public class QuestSO : ScriptableObject
{
    public Quest quest;
    public string questName;
    public int questID;

    // Quest must be initialized
    public void InitializeQuest()
    {
        quest = new Quest(questID, questName);
    }

    public void TryCompleteQuest()
    {

    }
}
