using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] DialogueSO dialogue;

    public void Interact()
    {
        FindFirstObjectByType<DialogueManager>().StartDialogue(dialogue);
    }
}
