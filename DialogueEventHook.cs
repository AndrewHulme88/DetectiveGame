using UnityEngine;

public class DialogueEventHook : MonoBehaviour
{
    [SerializeField] CharacterProfileSO profileToUnlock;

    public void UnlockProfile()
    {
        var notebook = FindFirstObjectByType<NoteBookManager>();
        if(notebook != null)
        {
            notebook.UnlockProfile(profileToUnlock);
        }
    }
}
