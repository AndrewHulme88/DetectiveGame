using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "DetectiveGame/Dialog")]
public class DialogueSO : ScriptableObject
{
    public string speakerName;
    [TextArea] public string introLine;

    public List<DialogueOption> options;
}

[System.Serializable]
public class DialogueOption
{
    [TextArea] public string playerChoice;
    [TextArea] public string response;
    public DialogueSO followUpDialogue;
    public TheorySO requiredTheory;

    public UnityEvent onSelected;
}
