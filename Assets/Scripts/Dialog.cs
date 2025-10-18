using UnityEngine;

[System.Serializable]
public class Dialog
{
    public string name;
    public Sprite image;
    public AudioClip[] textAudios;
    public AudioClip customAudio;
    [TextArea(3, 10)] public string sentence;
}
