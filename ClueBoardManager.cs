using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class ClueBoardManager : MonoBehaviour
{
    [SerializeField] GameObject boardCanvas;
    [SerializeField] GameObject clueCardPrefab;
    [SerializeField] Transform clueCardContainer;

    private NoteBookManager notebookManager;

    private bool boardOpen = false;

    public bool IsBoardOpen => boardOpen;

    private void Awake()
    {
        notebookManager = FindFirstObjectByType<NoteBookManager>();
    }

    private void Update()
    {
        if(Keyboard.current.mKey.wasPressedThisFrame)
        {
            ToggleBoard();
        }
    }

    public void ToggleBoard()
    {
        if(notebookManager != null && notebookManager.IsNotebookOpen)
        {
            notebookManager.ToggleNoteBook();
        }

        boardOpen = !boardOpen;
        boardCanvas.SetActive(boardOpen);

        Cursor.lockState = boardOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = boardOpen;

        if (boardOpen && notebookManager != null)
        {
            LoadClues(notebookManager.CollectedClues);
        }
    }

    public void LoadClues(List<ClueData> clues)
    {
        foreach (Transform child in clueCardContainer)
            Destroy(child.gameObject);

        foreach(var clue in clues)
        {
            GameObject card = Instantiate(clueCardPrefab, clueCardContainer);
            card.GetComponent<DraggableClueCard>().Initialize(clue.clueName);
        }
    }
}
