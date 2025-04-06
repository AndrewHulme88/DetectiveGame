using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TextMeshProUGUI speakerNameText;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] Transform optionsContainer;
    [SerializeField] GameObject optionButtonPrefab;

    private DialogueSO currentDialogue;

    private void Start()
    {
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(DialogueSO dialogue)
    {
        currentDialogue = dialogue;

        dialoguePanel.SetActive(true);
        speakerNameText.text = dialogue.speakerName;
        dialogueText.text = dialogue.introLine;

        foreach(Transform child in optionsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach(var option in dialogue.options)
        {
            GameObject buttonObject = Instantiate(optionButtonPrefab, optionsContainer);
            TextMeshProUGUI buttonText = buttonObject.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = option.playerChoice;

            Button button = buttonObject.GetComponent<Button>();
            DialogueOption localOption = option;
            button.onClick.AddListener(() => SelectOption(localOption));
        }
    }

    private void SelectOption(DialogueOption option)
    {
        dialogueText.text = option.response;

        foreach(Transform child in optionsContainer)
        {
            Destroy(child.gameObject);
        }

        if(option.followUpDialogue != null)
        {
            Invoke(nameof(ContinueToFollowUp), 1.5f);
        }
        else
        {
            Invoke(nameof(CloseDialogue), 2f);
        }
    }

    private void ContinueToFollowUp()
    {
        StartDialogue(currentDialogue.options.Find(o => o.response == dialogueText.text).followUpDialogue);
    }

    private void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}
