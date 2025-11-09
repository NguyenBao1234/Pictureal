using UnityEngine;
[System.Serializable]
public struct DialogueStruct
{
    [TextArea(2, 5)]
    public string message;
    public float durationAfter;
}
