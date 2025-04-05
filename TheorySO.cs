using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "DetectiveGame/Theory")]
public class TheorySO : ScriptableObject
{
    public string theoryName;
    [TextArea] public string description;
    public List<string> requiredClues;
    public bool startsUnlocked = false;
    public List<TheorySO> unlocksWhenSolved;
    public UnityEvent onSolved;

    [HideInInspector] public bool isSolved = false;
    [HideInInspector] public bool isUnlocked = false;
}
