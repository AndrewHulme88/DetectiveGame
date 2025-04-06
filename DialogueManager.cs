using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine.InputSystem;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TextMeshProUGUI speakerNameText;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] Transform optionsContainer;
    [SerializeField] GameObject optionButtonPrefab;
    [SerializeField] GameObject continueButton;

    public bool IsDialogueOpen { get; private set; } = false;

    private DialogueSO currentDialogue;
    private DialogueOption currentSelectedOption;

    private void Start()
    {
        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if(IsDialogueOpen && continueButton.activeSelf && Keyboard.current.eKey.wasPressedThisFrame)
        {
            OnContinuePressed();
        }
    }

    public void StartDialogue(DialogueSO dialogue)
    {
        IsDialogueOpen = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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
        currentSelectedOption = option;

        foreach(Transform child in optionsContainer)
        {
            Destroy(child.gameObject);
        }

        if(option.followUpDialogue != null)
        {
            continueButton.SetActive(true);
        }
        else
        {
            continueButton.SetActive(true);
        }
    }

    public void OnContinuePressed()
    {
        continueButton.SetActive(false);

        if(currentSelectedOption.followUpDialogue != null)
        {
            StartDialogue(currentSelectedOption.followUpDialogue);
        }
        else
        {
            CloseDialogue();
        }
    }

    private void CloseDialogue()
    {
        StartCoroutine(CloseDialogueDelay());
    }

    private IEnumerator CloseDialogueDelay()
    {
        yield return new WaitForSeconds(0.1f);

        IsDialogueOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        dialoguePanel.SetActive(false);
    }
}
