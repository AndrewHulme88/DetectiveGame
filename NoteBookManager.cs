using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

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
    [SerializeField] Transform profileListParent;
    [SerializeField] GameObject profileEntryPrefab;
    [SerializeField] TextMeshProUGUI profileDescriptionText;
    [SerializeField] Image profilePortraitImage;
    [SerializeField] GameObject cluesTab;
    [SerializeField] GameObject theoriesTab;
    [SerializeField] GameObject profilesTab;
    [SerializeField] List<CharacterProfileSO> allProfiles = new();
    [SerializeField] GameObject profilePopupUI;
    [SerializeField] TextMeshProUGUI profilePopupText;
    [SerializeField] float popuDuration = 2.5f;

    private List<ClueData> collectedClues = new();
    private List<TheorySO> solvedTheories = new();

    public List<ClueData> CollectedClues => collectedClues;
    public List<TheorySO> SolvedTheories => solvedTheories;
    public List<CharacterProfileSO> AllProfiles => allProfiles;

    private bool notebookOpen = false;
    private ClueBoardManager clueBoardManager;
    private Coroutine popupRoutine;

    public bool IsNotebookOpen => notebookOpen;

    private void Awake()
    {
        clueBoardManager = FindFirstObjectByType<ClueBoardManager>();
    }

    private void Start()
    {
        foreach (var profile in allProfiles)
        {
            profile.isUnlocked = profile.startsUnlocked;
        }

        RefreshTheoryLists(clueBoardManager.AllTheories);
    }

    private void Update()
    {
        if (FindFirstObjectByType<DialogueManager>()?.IsDialogueOpen == true) return;
        if (FindFirstObjectByType<BodyInspector>()?.IsInspectorOpen == true) return;

        if (Keyboard.current.tabKey.wasPressedThisFrame)
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
            ShowNotebookTab("Clues");

            RefreshClueList();
            RefreshTheoryLists(clueBoardManager.AllTheories);
            RefreshProfiles();
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

    public void RefreshClueList()
    {
        foreach(Transform t in clueListParent)
        {
            Destroy(t.gameObject);
        }

        foreach(var clue in collectedClues)
        {
            GameObject entry = Instantiate(clueEntryPrefab, clueListParent);
            entry.GetComponentInChildren<TextMeshProUGUI>().text = clue.clueName;

            Button button = entry.AddComponent<Button>();
            button.onClick.AddListener(() => ShowClueDescription(clue));
        }
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

    public void UnlockProfile(CharacterProfileSO profile)
    {
        if (!profile.isUnlocked)
        {
            profile.isUnlocked = true;
            RefreshProfiles();
            ShowProfilePopup(profile.characterName);
        }
    }

    public void RefreshProfiles()
    {
        foreach(Transform t in profileListParent)
        {
            Destroy(t.gameObject);
        }

        foreach(var profile in this.allProfiles)
        {
            if (!profile.isUnlocked) continue;

            GameObject entry = Instantiate(profileEntryPrefab, profileListParent);
            entry.GetComponentInChildren<TextMeshProUGUI>().text = profile.characterName;

            Button button = entry.AddComponent<Button>();
            button.onClick.AddListener(() => ShowProfileDescription(profile));
        }
    }

    private void ShowProfileDescription(CharacterProfileSO profile)
    {
        profileDescriptionText.text = profile.description;
        profilePortraitImage.sprite = profile.portrait;
    }

    public void ShowProfilePopup(string profileName)
    {
        if(popupRoutine != null)
        {
            StopCoroutine(popupRoutine);
        }

        profilePopupText.text = $"New Profile Unlocked: {profileName}";
        profilePopupUI.SetActive(true);
        popupRoutine = StartCoroutine(HideProfileAfterDelay());
    }

    private IEnumerator HideProfileAfterDelay()
    {
        yield return new WaitForSeconds(popuDuration);
        profilePopupUI.SetActive(false);
    }

    public void ShowNotebookTab(string tab)
    {
        cluesTab.SetActive(tab == "Clues");
        theoriesTab.SetActive(tab == "Theories");
        profilesTab.SetActive(tab == "Profiles");
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
