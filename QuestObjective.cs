using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestObjective
{
    [Header("General")]
    public int index;
    public string context;

    [Header("Condition")]
    public QuestObjectiveCondition condition;
    public bool isOptional;
    public bool isCriticalFail;
}

public enum QuestObjectiveCondition { Incompleted, Completed, Failed}