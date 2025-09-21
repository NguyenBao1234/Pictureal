using UnityEngine;

public class PlaySoundEvent : MonoBehaviour
{
    public AudioClip clip;
    public Transform LocationToPlaySound;

    public void PlaySound()
    {
        AudioSource.PlayClipAtPoint(clip, LocationToPlaySound.position);
        Debug.Log("Sound Played: "+clip + LocationToPlaySound.position);
    }
}
