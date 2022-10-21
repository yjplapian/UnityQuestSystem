using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestStage
{
   [Multiline(3)]  public string stageLog;

    [Header("Indexes")]
    public int index;
    public int ObjectiveIndex;

    [Header("Objectives")]
    public List<QuestObjective> objectives;

    public void ResetObjectives()
    {
        int count = 0;
        foreach(var objective in objectives)
        {
            objective.condition = QuestObjectiveCondition.Incompleted;
            objective.index = count;
            count++;
        }
    }
}