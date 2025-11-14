using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class AutoTextDisplay : MonoBehaviour
{
    [SerializeField] private UnityEvent OnDialogueFinished;
    [Header("UI")]
    public TextMeshProUGUI uiText;
    [Header("Entries")]
    public List<DialogueStruct> Messages = new List<DialogueStruct>();

    [Header("Typing & Timing (global)")]
    public bool bAppendWord = true; 
    public float typingSpeed = 0.05f; 
    
    
    [Header("Audio (optional)")]
    public AudioClip charSound;  
    public float charVolume = 0.6f; 
    public bool skipSpaces = true;  
    [Tooltip("Phát sound mỗi N ký tự")]
    public int charsPerSound = 1;  

    private Coroutine currentRoutine;
    private AudioSource audioSource;


    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 0;
            audioSource.loop = false;
            audioSource.playOnAwake = false;
        }
    }

    private void Start()
    {
        if (uiText != null)
        {
            uiText.gameObject.SetActive(true);
            uiText.text = "";
            StartCoroutine(PlayMessageCoroutine());
        }
    }

    private IEnumerator PlayMessageCoroutine()
    {
        if (Messages == null || Messages.Count == 0) yield break;
        uiText.gameObject.SetActive(true);
        for (int i = 0; i < Messages.Count; i++)
        {
            var message = Messages[i];

            if (bAppendWord) yield return StartCoroutine(TypeText(message.message));
            else uiText.text = message.message;
            
            yield return new WaitForSeconds(message.durationAfter);
        }
        OnDialogueFinished?.Invoke();
        uiText.text = ""; 
        uiText.gameObject.SetActive(false);
        currentRoutine = null;
    }
    private IEnumerator TypeText(string text)
    {
        uiText.text = "";

        int soundCounter = 0;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            uiText.text += c;

            if (charSound != null && audioSource != null)
            {
                if (!(skipSpaces && c == ' '))
                {
                    soundCounter++;
                    if (charsPerSound <= 1 || soundCounter % charsPerSound == 0)
                    {
                        float randomPitch = Random.Range(0.7f, 1.3f);
                        audioSource.pitch = randomPitch;
                        audioSource.PlayOneShot(charSound, charVolume);
                    }
                }
            }

            yield return new WaitForSeconds(typingSpeed);
        }
        if (audioSource != null && audioSource.isPlaying) audioSource.Stop();
        if (!uiText.IsActive()) uiText.gameObject.SetActive(true);
    }
}
