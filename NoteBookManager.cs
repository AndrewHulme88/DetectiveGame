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
    [SerializeField] Transform unsolvedTheoryListParent;
    [SerializeField] Transform solvedTheoryListParent;
    [SerializeField] GameObject theoryEntryPrefab;
    [SerializeField] TextMeshProUGUI theoryDescriptionText;

    private List<ClueData> collectedClues = new();
    private List<TheorySO> solvedTheories = new();

    public List<ClueData> CollectedClues => collectedClues;
    public List<TheorySO> SolvedTheories => solvedTheories;

    private bool notebookOpen = false;
    private ClueBoardManager clueBoardManager;

    public bool IsNotebookOpen => notebookOpen;

    private void Awake()
    {
        clueBoardManager = FindFirstObjectByType<ClueBoardManager>();
    }

    private void Start()
    {
        RefreshTheoryLists(clueBoardManager.AllTheories);
    }

    private void Update()
    {
        if(Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleNoteBook();
        }
    }

    public void ToggleNoteBook()
    {
        if(clueBoardManager != null && clueBoardManager.IsBoardOpen)
        {
            clueBoardManager.ToggleBoard();
        }

        notebookOpen = !notebookOpen;
        noteBookUI.SetActive(notebookOpen);

        if(notebookOpen)
        {
            RefreshTheoryLists(clueBoardManager.AllTheories);
        }

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

    public void AddSolvedTheory(TheorySO theory)
    {
        if(!solvedTheories.Contains(theory))
        {
            solvedTheories.Add(theory);

            GameObject entry = Instantiate(theoryEntryPrefab, solvedTheoryListParent);
            entry.GetComponentInChildren<TextMeshProUGUI>().text = theory.theoryName;
        }
    }

    public void RefreshTheoryLists(List<TheorySO> allTheories)
    {
        foreach(Transform t in unsolvedTheoryListParent)
        {
            Destroy(t.gameObject);
        }
        foreach(Transform t in solvedTheoryListParent)
        {
            Destroy(t.gameObject);
        }

        foreach(var theory in allTheories)
        {
            if (!theory.isUnlocked) continue;

            GameObject entry = Instantiate(theoryEntryPrefab, theory.isSolved ?
                solvedTheoryListParent : unsolvedTheoryListParent);

            string label = theory.theoryName;

            entry.GetComponentInChildren<TextMeshProUGUI>().text = label;

            if(!theory.isSolved)
            {
                Button button = entry.AddComponent<Button>();
                button.onClick.AddListener(() => ShowTheoryDescription(theory));
            }
        }
    }

    private void ShowTheoryDescription(TheorySO theory)
    {
        theoryDescriptionText.text = theory.description;
    }

    public void RemoveClue(ClueData clue)
    {
        collectedClues.Remove(clue);
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
