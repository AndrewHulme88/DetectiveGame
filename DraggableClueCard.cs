using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DraggableClueCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] TextMeshProUGUI clueNameText;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Transform originalParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Initialize(string clueName)
    {
        clueNameText.text = clueName;
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
        FindFirstObjectByType<ClueBoardManager>().HandleClueClicked(this);
    }
}
