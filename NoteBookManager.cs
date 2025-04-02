using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.EventSystems;

public class NoteBookManager : MonoBehaviour
{
    [SerializeField] GameObject noteBookUI;
    [SerializeField] Transform clueListParent;
    [SerializeField] GameObject clueEntryPrefab;
    [SerializeField] TextMeshProUGUI clueDescriptionText;

    private List<ClueData> collectedClues = new();

    private bool notebookOpen = false;

    public bool IsNotebookOpen => notebookOpen;

    private void Update()
    {
        if(Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleNoteBook();
        }
    }

    private void ToggleNoteBook()
    {
        notebookOpen = !notebookOpen;
        noteBookUI.SetActive(notebookOpen);

        Cursor.lockState = notebookOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = notebookOpen;
    }

    public void AddClue(ClueData clue)
    {
        collectedClues.Add(clue);
        GameObject entry = Instantiate(clueEntryPrefab, clueListParent);
        entry.GetComponentInChildren<TextMeshProUGUI>().text = clue.clueName;

        Button button = entry.AddComponent<Button>();
        button.onClick.AddListener(() => ShowClueDescription(clue));
    }

    private void ShowClueDescription(ClueData clue)
    {
        clueDescriptionText.text = clue.description;
    }
}

[System.Serializable]
public class ClueData
{
    public string clueName;
    public string description;

    public ClueData(string name, string desc)
    {
        clueName = name;
        description = desc;
    }
}
