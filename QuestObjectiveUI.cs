using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class QuestObjectiveUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Quest quest;

    [Header("Components")]
    [SerializeField] Animator animator;
    [SerializeField] GameObject imageObject;
    [SerializeField] TextMeshProUGUI text;

    [Header("Events")]
    [SerializeField] EventChannel<Quest> OnSelectionUpdate;

    public void SetUI(Quest quest, string context)
    {
        this.quest = quest;
        text.text = context;
    }

    public void IsComplete(QuestObjectiveCondition condition)
    {
        if(condition == QuestObjectiveCondition.Completed)
            imageObject.SetActive(true);

        else
            imageObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (animator == null)
            return;

        animator.Play("Hover");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (animator == null)
            return;

        StartCoroutine(OnExit(eventData)); 
    }

    private IEnumerator OnExit(PointerEventData eventData)
    {
        yield return new WaitForSeconds(0.1f);
        animator.Play("Normalize");
    }
}