using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class ClueBoardManager : MonoBehaviour
{
    [SerializeField] GameObject boardCanvas;
    [SerializeField] GameObject clueCardPrefab;
    [SerializeField] Transform clueCardContainer;
    [SerializeField] float clueCardSpacingHorizontal = 200f;
    [SerializeField] GameObject linePrefab;

    private NoteBookManager notebookManager;
    private bool boardOpen = false;
    private DraggableClueCard firstSelectedCard;
    private List<GameObject> activeLines = new();

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

        float startX = -((clues.Count - 1) * clueCardSpacingHorizontal) / 2f;
        float y = 0f;

        for(int i = 0; i < clues.Count; i++)
        {
            GameObject card = Instantiate(clueCardPrefab, clueCardContainer);
            card.GetComponent<DraggableClueCard>().Initialize(clues[i].clueName);

            RectTransform rect = card.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(startX + i * clueCardSpacingHorizontal, y);
        }
    }

    public void HandleClueClicked(DraggableClueCard clicked)
    {
        if(firstSelectedCard == null)
        {
            firstSelectedCard = clicked;
        }
        else if(firstSelectedCard != clicked)
        {
            CreateLink(firstSelectedCard, clicked);
            firstSelectedCard = null;
        }
        else
        {
            firstSelectedCard = null;
        }
    }

    void CreateLink(DraggableClueCard a, DraggableClueCard b)
    {
        GameObject lineObject = Instantiate(linePrefab, clueCardContainer);
        UILineRenderer line = lineObject.GetComponent<UILineRenderer>();

        Vector2 start = GetAnchoredPosition(a.transform as RectTransform);
        Vector2 end = GetAnchoredPosition(b.transform as RectTransform);

        line.Points = new Vector2[] { start, end };

        activeLines.Add(lineObject);
    }

    private Vector2 GetAnchoredPosition(RectTransform rect)
    {
        return rect.anchoredPosition;
    }
}
