using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class QuestException : System.Exception
{
    public QuestException(string message) : base(message)
    {  }
}

[CreateAssetMenu(menuName = "Scriptable Components/Quests/Quest", fileName = "Quest")]
public class Quest : ScriptableObject
{
    [Header("General"), Tooltip("Name of the quest.")]
    public string _name;

    [Header("Conditions"), Tooltip("State of the quest.")]
    public QuestCondition questCondition;

    [Header("Message Flags"), Tooltip("Message Flag played.")]
    public QuestMessageFlag questMessageFlag;

    [Header("Chain Quest")]
    [Tooltip("Allow a follow up quest?")]public bool chainsQuest;
    [Tooltip("Next quest in line.")]public Quest nextQuest;

    [Header("Stage")]
    [Tooltip("Current stage of the quest.")] public int stageIndex;
    [Tooltip("Available stages.")] public List<QuestStage> stages = new();

    public QuestStage CurrentStage =>stages[stageIndex];

    [Header("Objective"), HideInInspector]
    public QuestObjective lastUpdatedObjective;

    [Header("Events")]
    [Tooltip("Gets called on the start of the quest.")] public EventChannel<Quest> OnStartQuest;
    [Tooltip("Gets called on the end of the quest.")] public EventChannel<Quest> OnEndQuest;
    [Tooltip("Gets called on a condition change of an objective.")] public EventChannel<Quest> OnObjectiveEnd;
    public EventChannel<Quest> OnObjectiveUpdate;
    [Tooltip("Gets called when each objective in a stage is completed.")] public EventChannel<Quest> OnCompleteStage;

    private void OnValidate() =>
        ResetQuest();

    private void ResetQuest()
    {
        questCondition = QuestCondition.Inactive;
        questMessageFlag = QuestMessageFlag.None;

        stageIndex = 0;
        lastUpdatedObjective = null;

        foreach(var stage in stages)
            stage.ResetObjectives();
    }

    public void StartQuest()
    {
        if (stages.Count == 0)
            throw new QuestException($"No stages found in {name}.");

        questCondition = QuestCondition.Started;
        questMessageFlag = QuestMessageFlag.Started;
        OnStartQuest.RaiseEvent(this);
    }
    public void EndQuestWithCondition(QuestCondition condition, QuestMessageFlag messageCondition)
    {
        questCondition = condition;
        questMessageFlag = messageCondition;
        OnEndQuest.RaiseEvent(this);
    }

    public void SetStage(int index) =>
        stageIndex = index;
    public void SetObjectiveCondition(QuestObjectiveCondition objectiveCondition, int targetObjective, int newStage)
    {
        CurrentStage.objectives[targetObjective].condition = objectiveCondition;
        lastUpdatedObjective = CurrentStage.objectives[targetObjective];

        if (CurrentStage.objectives.Any(objective => objective.condition == QuestObjectiveCondition.Failed && objective.isCriticalFail))
        {
            EndQuestWithCondition(QuestCondition.Failed, QuestMessageFlag.Failed);
            return;
        }

        if (CurrentStage.objectives.All(objective => objective.condition == QuestObjectiveCondition.Completed || objective.condition == QuestObjectiveCondition.Failed ||
         objective.condition == QuestObjectiveCondition.Incompleted && objective.isOptional))
        {
            if (stageIndex == stages.Count - 1)
            {
                EndQuestWithCondition(QuestCondition.Completed, QuestMessageFlag.Completed);
                return;
            }

            else
            {
                //TODO: reconfigure how stages are set.
                SetStage(newStage);

                if (CurrentStage.objectives.Count == 0)
                    throw new QuestException($"No objectives found in {name} at stage {newStage}.");

                OnCompleteStage.RaiseEvent(this);
            }
        }

        else
            OnObjectiveEnd.RaiseEvent(this);
    }
}

public enum QuestCondition {Inactive, Started, Completed, Failed};
public enum QuestMessageFlag {None, Started, Completed, Failed};