using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class NoteBookManager : MonoBehaviour
{
    [SerializeField] GameObject noteBookUI;
    [SerializeField] Transform clueListParent;
    [SerializeField] GameObject clueEntryPrefab;

    private List<ClueData> collectedClues = new();

    private bool noteBookOpen = false;

    private void Update()
    {
        if(Keyboard.current.tabKey.wasPressedThisFrame)
        {
            //ToggleNoteBook();
        }
    }

    private void ToggleNoteBook()
    {
        noteBookOpen = !noteBookOpen;
        noteBookUI.SetActive(noteBookOpen);

        Cursor.lockState = noteBookOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = noteBookOpen;
    }

    public void AddClue(ClueData clue)
    {
        collectedClues.Add(clue);
        GameObject entry = Instantiate(clueEntryPrefab, clueListParent);
        entry.GetComponentInChildren<Text>().text = clue.clueName;
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
