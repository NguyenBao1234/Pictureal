using UnityEngine;

public class PlaySoundEvent : MonoBehaviour
{
    public AudioClip clip;

    public void PlaySound()
    {
        AudioSource.PlayClipAtPoint(clip, transform.position);
        Debug.Log("Sound Played: "+clip + transform.position);
    }
}
