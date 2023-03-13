#define DEBUGGING
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestManager : MonoBehaviour
{
    [SerializeField] List<QuestSO> _questsSO = new List<QuestSO>();

    private int _currentQuestID;

    void Start()
    {
        // Initialize all Quest from QuestSO
        InitAllQuests();

        // Activate first quest 
        ActivateQuestByID(0);


        _currentQuestID = GetCurrentActiveQuestByID();

#if DEBUGGING
        GameDebugLog.LogMessage("Current QuestID: " + _currentQuestID);
#endif
    }

    void Update()
    {
        
    }
   

    private void NextQuest()
    {
        // If is completed
        // Take ID of completed quest 
        // ID ++
        // Actiavte quest with new ID
    }

    // Only active quest
    private int GetCurrentActiveQuestByID()
    {
        if (_questsSO.Count.Equals(0))
        {
            GameDebugLog.LogWarning("GetCurrentActiveQuestByID : questsSO is null");
            return 0;
        }

        var result = _questsSO.Where(x => x.quest != null && x.quest.currentState == QuestState.Active).FirstOrDefault();
        return result.questID;
    }

    private void InitAllQuests()
    {
        if (_questsSO.Count.Equals(0)) 
        {
            GameDebugLog.LogWarning("InitAllQuests : questsSO is null");
            return; 
        }

        var result = _questsSO.Where(x => x.quest == null).ToList();
        result.ForEach(x => x.InitializeQuest());
    }

    private void InitQuestByID (int questID)
    {
        if (_questsSO.Count.Equals(0))
        {
            GameDebugLog.LogWarning("InitQuestByID : questsSO is null");
            return;
        }

        var result = _questsSO.Where(x => x.quest == null && x.questID == questID).FirstOrDefault();
        result.InitializeQuest();
    }

    private void ActivateQuestByID(int questID)
    {
        if (_questsSO.Count.Equals(0))
        {
            GameDebugLog.LogWarning("ActivateQuestByID : questsSO is null");
            return;
        }

        var result = _questsSO.Where(x => x.quest != null && x.questID == questID).FirstOrDefault();
        result.quest.ActivateQuest();
    }
}
