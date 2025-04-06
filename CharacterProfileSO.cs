using UnityEngine;

[CreateAssetMenu(menuName = "DetectiveGame/Character Profile")]
public class CharacterProfileSO : ScriptableObject
{
    public string characterName;
    [TextArea] public string description;
    public Sprite portrait;

    public enum Role { Suspect, Witness, Victim }
    public Role characterRole;

    public bool startsUnlocked = false;

    [HideInInspector] public bool isUnlocked = false;
}
