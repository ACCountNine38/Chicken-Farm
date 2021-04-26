using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Audio[] audios;

    public void Awake()
    {
        foreach (Audio a in audios)
        {
            a.source = gameObject.AddComponent<AudioSource>();
            a.source.clip = a.clip;
            a.source.volume = a.volume;
            a.source.pitch = a.pitch;
            a.source.spatialBlend = a.blend;
            a.source.rolloffMode = AudioRolloffMode.Linear;

        }
    }

    public void Play(string name)
    {
        foreach (Audio audio in audios)
        {
            if (audio.name == name)
            {
                audio.source.Play();
                break;
            }
        }
        // FindObjectOfType<AudioManager>().Play("name");
    }

    public void UIButtonHover()
    {
        Play("hover");
    }

    public void UIButtonPress()
    {
        Play("press");
    }
}
