using UnityEngine;

public class VictimBodyTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] VictimProfileSO victimData;

    private bool hasBeenInspected = false;

    public void Interact()
    {
        if (!hasBeenInspected)
        {
            hasBeenInspected = true;
            victimData.isDiscovered = true;

            var notebook = FindFirstObjectByType<NoteBookManager>();
            foreach(var clue in victimData.initialClues)
            {
                notebook.AddClue(clue);
            }
        }

        FindFirstObjectByType<BodyInspector>()?.OpenInspection(victimData);
    }
}
