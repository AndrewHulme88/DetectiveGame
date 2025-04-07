using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "DetectiveGame/Victim Profile")]
public class VictimProfileSO : ScriptableObject
{
    public string victimName;
    public int age;
    public string causeOfDeath;
    [TextArea] public string background;
    public Sprite portrait;

    [HideInInspector] public bool isDiscovered = false;

    public CharacterProfileSO linkedProfile;
    public List<ClueData> initialClues;
}
