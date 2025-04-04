using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DraggableClueCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] TextMeshProUGUI clueNameText;

    public string ClueName => clueData.clueName;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Transform originalParent;
    private ClueData clueData;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Initialize(ClueData data)
    {
        clueData = data;
        clueNameText.text = clueData.clueName;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        originalParent = transform.parent;
        transform.SetParent(originalParent.parent);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        transform.SetParent(originalParent);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var board = FindFirstObjectByType<ClueBoardManager>();

        if(eventData.button == PointerEventData.InputButton.Right)
        {
            board.HandleClueRightClick(this);
        }
        else if(eventData.button == PointerEventData.InputButton.Left)
        {
            board.HandleClueClicked(this);
        }
    }
}
