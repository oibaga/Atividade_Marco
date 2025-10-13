using UnityEngine;

[System.Serializable]
public class Dialog
{
    public string name;
    public Sprite image; 
    [TextArea(3, 10)] public string sentence;
}
