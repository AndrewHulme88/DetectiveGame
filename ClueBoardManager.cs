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
    private HashSet<ClueLink> existingLinks = new();

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

        foreach(var link in existingLinks)
        {
            if (link.line == null) continue;

            Vector2 start = GetAnchoredPosition(link.a.transform as RectTransform);
            Vector2 end = GetAnchoredPosition(link.b.transform as RectTransform);

            link.line.Points = new Vector2[] { start, end };
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

    public void HandleClueRightClick(DraggableClueCard clicked)
    {
        ClueLink foundLink = null;

        foreach (var link in existingLinks)
        {
            if (link.a == clicked || link.b == clicked)
            {
                foundLink = link;
                break;
            }
        }

        if (foundLink != null)
        {
            existingLinks.Remove(foundLink);

            GameObject lineToRemove = null;

            foreach (var lineObject in activeLines)
            {
                var line = lineObject.GetComponent<UILineRenderer>();

                if (line.Points.Length < 2) continue;

                Vector2 posA = GetAnchoredPosition(foundLink.a.GetComponent<RectTransform>());
                Vector2 posB = GetAnchoredPosition(foundLink.b.GetComponent<RectTransform>());

                if ((line.Points[0] == posA && line.Points[1] == posB) ||
                    (line.Points[0] == posB && line.Points[1] == posA))
                {
                    lineToRemove = lineObject;
                    break;
                }
            }

            if(lineToRemove != null)
            {
                activeLines.Remove(lineToRemove);
                Destroy(lineToRemove);
            }

            Debug.Log("Line removed.");
        }
    }

    void CreateLink(DraggableClueCard a, DraggableClueCard b)
    {
        var lineObject = Instantiate(linePrefab, clueCardContainer);
        var line = lineObject.GetComponent<UILineRenderer>();

        Vector2 start = GetAnchoredPosition(a.transform as RectTransform);
        Vector2 end = GetAnchoredPosition(b.transform as RectTransform);

        line.Points = new Vector2[] { start, end };

        var newLink = new ClueLink(a, b, line);
        existingLinks.Add(newLink);
        activeLines.Add(lineObject);
    }

    private Vector2 GetAnchoredPosition(RectTransform rect)
    {
        return rect.anchoredPosition;
    }
}

[System.Serializable]
public class ClueLink
{
    public DraggableClueCard a;
    public DraggableClueCard b;
    public UILineRenderer line;

    public ClueLink(DraggableClueCard a, DraggableClueCard b, UILineRenderer line)
    {
        if (a.GetInstanceID() < b.GetInstanceID())
        {
            this.a = a;
            this.b = b;
        }
        else
        {
            this.a = b;
            this.b = a;
        }

        this.line = line;
    }

    public override bool Equals(object obj)
    {
        if (obj is not ClueLink other) return false;

        return a == other.a && b == other.b;
    }

    public override int GetHashCode()
    {
        return a.GetHashCode() ^ b.GetHashCode();
    }
}
