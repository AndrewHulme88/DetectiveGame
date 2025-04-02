using UnityEngine;

public class Clue : MonoBehaviour, IInteractable
{
    [SerializeField] string clueName = "Unnamed Clue";
    [SerializeField] string clueDescription = "No description";

    public void Interact()
    {
        NoteBookManager notebook = FindFirstObjectByType<NoteBookManager>();
        notebook.AddClue(new ClueData(clueName, clueDescription));

        Debug.Log($"Clue found: {clueName}");
        gameObject.SetActive(false);
    }
}
