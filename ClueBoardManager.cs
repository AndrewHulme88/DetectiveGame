using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using System.Linq;
using TMPro;
using System.Collections;

public class ClueBoardManager : MonoBehaviour
{
    [SerializeField] GameObject boardCanvas;
    [SerializeField] GameObject clueCardPrefab;
    [SerializeField] Transform clueCardContainer;
    [SerializeField] float clueCardSpacingHorizontal = 200f;
    [SerializeField] GameObject linePrefab;
    [SerializeField] GameObject theoryPopup;
    [SerializeField] TextMeshProUGUI popupText;
    [SerializeField] float popupDuration = 2.5f;
    [SerializeField] List<TheorySO> allTheories = new();

    public List<TheorySO> AllTheories => allTheories;

    private NoteBookManager notebookManager;
    private bool boardOpen = false;
    private DraggableClueCard firstSelectedCard;
    private List<GameObject> activeLines = new();
    private HashSet<ClueLink> existingLinks = new();
    private Coroutine popupRoutine;

    public bool IsBoardOpen => boardOpen;

    private void Awake()
    {
        notebookManager = FindFirstObjectByType<NoteBookManager>();

        foreach (var theory in allTheories)
        {
            theory.isSolved = false;
            theory.isUnlocked = theory.startsUnlocked;
        }
    }

    private void Update()
    {
        if (FindFirstObjectByType<DialogueManager>()?.IsDialogueOpen == true) return;

        if (Keyboard.current.mKey.wasPressedThisFrame)
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
            var clueCard = card.GetComponent<DraggableClueCard>();
            clueCard.Initialize(clues[i]);

            RectTransform rect = card.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(startX + i * clueCardSpacingHorizontal, y);
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)clueCardContainer);
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
        var newLink = new ClueLink(a, b, null);

        if(existingLinks.Contains(newLink))
        {
            Debug.Log("Link already exists between these clues");
            return;
        }

        var lineObject = Instantiate(linePrefab, clueCardContainer);
        lineObject.transform.SetAsFirstSibling();

        var line = lineObject.GetComponent<UILineRenderer>();
        line.raycastTarget = false;

        Vector2 start = GetAnchoredPosition(a.transform as RectTransform);
        Vector2 end = GetAnchoredPosition(b.transform as RectTransform);

        line.Points = new Vector2[] { start, end };

        newLink.line = line;
        existingLinks.Add(newLink);
        activeLines.Add(lineObject);
    }

    private Vector2 GetAnchoredPosition(RectTransform rect)
    {
        return rect.anchoredPosition;
    }

    public void SubmitTheory()
    {
        List<List<DraggableClueCard>> groups = GetAllConnectedGroups();

        foreach(var group in groups)
        {
            List<string> clueNames = new();

            foreach(var clue in group)
            {
                clueNames.Add(clue.ClueName);
            }

            foreach (var theory in allTheories.Where(t => t.isUnlocked))
            {
                if(theory.isSolved)
                {
                    continue;
                }

                if(MatchTheory(theory, clueNames))
                {
                    theory.isSolved = true;
                    theory.onSolved?.Invoke();
                    ShowPopup($"Theory '{theory.theoryName}' is correct!", Color.green);
                    notebookManager.AddSolvedTheory(theory);

                    foreach(var clueCard in group)
                    {
                        Destroy(clueCard.gameObject);
                    }

                    foreach(var clueCard in group)
                    {
                        var clueName = clueCard.ClueName;
                        var clueToRemove = notebookManager.CollectedClues.Find(c => c.clueName == clueName);
                        
                        if(clueToRemove != null)
                        {
                            notebookManager.RemoveClue(clueToRemove);
                        }
                    }

                    existingLinks.RemoveWhere(link => group.Contains(link.a) || group.Contains(link.b));
                    List<GameObject> toRemove = new();

                    foreach(var lineObject in activeLines)
                    {
                        Destroy(lineObject);
                        toRemove.Add(lineObject);
                    }
                    foreach(var line in toRemove)
                    {
                        activeLines.Remove(line);
                    }

                    Debug.Log($"Checking unlocks for theory: {theory.theoryName}, has {theory.unlocksWhenSolved.Count} to unlock.");

                    foreach (var t in theory.unlocksWhenSolved)
                    {
                        if(!t.isUnlocked)
                        {
                            t.isUnlocked = true;
                            Debug.Log($"Unlocked new theory: {t.theoryName}");
                        }
                    }

                    foreach (var t in allTheories)
                    {
                        Debug.Log($"ALL THEORIES LIST: {t.theoryName}: isUnlocked = {t.isUnlocked}, isSolved = {t.isSolved}");
                    }

                    notebookManager.RefreshTheoryLists(allTheories);

                    return;
                }
            }
        }

        ShowPopup("No valid theory found.", Color.red);
    }

    private bool MatchTheory(TheorySO theory, List<string> linkedClues)
    {
        var required = theory.requiredClues;

        if(linkedClues.Count != required.Count)
        {
            return false;
        }

        return required.All(req =>
        linkedClues.Any(linked =>
        linked.Equals(req, System.StringComparison.OrdinalIgnoreCase)));
    }

    private List<List<DraggableClueCard>> GetAllConnectedGroups()
    {
        List<List<DraggableClueCard>> groups = new();
        HashSet<DraggableClueCard> visited = new();

        foreach(var link in existingLinks)
        {
            if(!visited.Contains(link.a))
            {
                List<DraggableClueCard> group = new();
                Traverse(link.a, group, visited);
                groups.Add(group);
            }

            if(!visited.Contains(link.b))
            {
                List<DraggableClueCard> group = new();
                Traverse(link.b, group, visited);
                groups.Add(group);
            }
        }

        return groups;
    }

    private void Traverse(DraggableClueCard node, List<DraggableClueCard> group, HashSet<DraggableClueCard> visited)
    {
        if (visited.Contains(node)) return;

        visited.Add(node);
        group.Add(node);

        foreach(var link in existingLinks)
        {
            if(link.a == node && !visited.Contains(link.b))
            {
                Traverse(link.b, group, visited);
            }
            else if(link.b == node && !visited.Contains(link.a))
            {
                Traverse(link.a, group, visited);
            }
        }
    }

    private void ShowPopup(string message, Color color)
    {
        if(popupRoutine != null)
        {
            StopCoroutine(popupRoutine);
        }

        popupText.text = message;
        popupText.color = color;
        theoryPopup.SetActive(true);

        popupRoutine = StartCoroutine(HidePopupAfterDelay());
    }

    private IEnumerator HidePopupAfterDelay()
    {
        yield return new WaitForSeconds(popupDuration);
        theoryPopup.SetActive(false);
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
