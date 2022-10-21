using System.Collections;
using UnityEngine;
using TMPro;

//TODO: Update Message!
public class NotificationHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Animator titleMessageAnimator;
    [SerializeField] Animator criticalMessageAnimator;
    [SerializeField] Animator genericMessageAnimator;

    [SerializeField] TextMeshProUGUI titleMessageText;
    [SerializeField] TextMeshProUGUI criticalMessageText;
    [SerializeField] TextMeshProUGUI genericMessageMessageText;

    [Header("Events")]
    [SerializeField] EventChannel<Quest> OnStartQuest;
    [SerializeField] EventChannel<Quest> OnEndQuest;
    [SerializeField] EventChannel<Quest> OnObjectiveConditionChange;
    [SerializeField] EventChannel<Quest> OnGetStage;

    private void OnEnable()
    {
        OnStartQuest.OnEventRaised += StartQuestMessage;
        OnEndQuest.OnEventRaised += QuestConditionMesssage;
        OnObjectiveConditionChange.OnEventRaised += QuestObjectiveConditionMessage;
        OnGetStage.OnEventRaised += GetNewStageObjectives;
    }

    private void OnDisable()
    {
        OnStartQuest.OnEventRaised -= StartQuestMessage;
        OnEndQuest.OnEventRaised -= QuestConditionMesssage;
        OnObjectiveConditionChange.OnEventRaised -= QuestObjectiveConditionMessage;
        OnGetStage.OnEventRaised -= GetNewStageObjectives;
    }

    public void GetNewStageObjectives(Quest quest)
    {
        int length = quest.CurrentStage.objectives.Count;
        StartCoroutine(CompleteStage(quest, length));
    }

    public void QuestObjectiveConditionMessage(Quest quest)
    {
        criticalMessageText.text = $"{quest.lastUpdatedObjective.condition}: {quest.lastUpdatedObjective.context}";
        criticalMessageAnimator.Play("Popup");
    }

    public void StartQuestMessage(Quest quest)
    {
        int length = quest.CurrentStage.objectives.Count;
        StartCoroutine(ProcessQuestStateInformation(quest, true, true, length));
    }

    public void QuestConditionMesssage(Quest quest)
    {
        titleMessageText.text = $"{quest.questCondition}: {quest._name}";
        titleMessageAnimator.Play("Popup");
    }

    public IEnumerator ProcessQuestStateInformation(Quest quest, bool includeTitle, bool includeContext, int length)
    {
        if (includeTitle)
        {
            if (quest.questCondition == QuestCondition.Started)
                titleMessageText.text = $"{quest.questCondition}: {quest._name}";

            titleMessageAnimator.Play("Popup");
        }

        if (includeContext)
        {
            yield return new WaitUntil(() => titleMessageAnimator.GetCurrentAnimatorStateInfo(0).IsName("Popup") &&
             titleMessageAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

            for (int i = 0; i < length; i++)
            {
                criticalMessageText.text = quest.CurrentStage.objectives[i].context;
                criticalMessageAnimator.Play("Popup");

                yield return new WaitUntil(() => criticalMessageAnimator.GetCurrentAnimatorStateInfo(0).IsName("Popup") &&
                 criticalMessageAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
                yield return new WaitForSeconds(0.3f);
            }
        }
    }

    public IEnumerator ProcessObjectiveState(Quest quest)
    {
        yield return new WaitWhile(() => criticalMessageAnimator.GetCurrentAnimatorStateInfo(0).IsName("Popup"));

        criticalMessageText.text = $"{quest.lastUpdatedObjective.condition}: {quest.lastUpdatedObjective.context}";
        criticalMessageAnimator.Play("Popup");
    }

    public IEnumerator CompleteStage(Quest quest, int length)
    {
        yield return ProcessObjectiveState(quest);

        for(int i = 0; i < length; i++)
        {
            yield return new WaitUntil(() => criticalMessageAnimator.GetCurrentAnimatorStateInfo(0).IsName("Popup") &&
             criticalMessageAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
            yield return new WaitForSeconds(0.3f);

            criticalMessageText.text = quest.CurrentStage.objectives[i].context;
            criticalMessageAnimator.Play("Popup");
        }
    }
}