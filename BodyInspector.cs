using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BodyInspector : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI causeOfDeathText;
    [SerializeField] TextMeshProUGUI backgroundText;
    //[SerializeField] Image portraitImage;

    private bool isInspectorOpen = false;

    public bool IsInspectorOpen => isInspectorOpen;

    public void OpenInspection(VictimProfileSO victim)
    {
        isInspectorOpen = true;

        nameText.text = victim.victimName;
        causeOfDeathText.text = "Cause of Death: " + victim.causeOfDeath;
        backgroundText.text = victim.background;
        //portraitImage.sprite = victim.portrait;

        panel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseInspection()
    {
        isInspectorOpen = false;

        panel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
