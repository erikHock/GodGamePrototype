using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Quest
{
    protected int _questID;
    protected string _questName;

    public bool isCompleted { get; private set; }
    public QuestState currentState { get; private set; }

    public Action<Quest> OnQuestActivate;
    public Action<Quest> OnQuestComplete;

    public Quest(int questID, string questName)
    {
        this._questID = questID;
        this._questName = questName;
        this.currentState = QuestState.Pending;
        this.isCompleted = false;
    }
    public void ActivateQuest()
    {
        if (!this.isCompleted && this.currentState == QuestState.Pending)
        {
            this.currentState = QuestState.Active;

            OnQuestActivate?.Invoke(this);
        }
    }

    public void CompleteQuest()
    {
        if (!this.isCompleted && this.currentState == QuestState.Active)
        {
            this.currentState = QuestState.Complete;
            isCompleted = true;

            OnQuestComplete?.Invoke(this);
        }
    }
}
public enum QuestState
{
    Pending,
    Active,
    Complete
}
