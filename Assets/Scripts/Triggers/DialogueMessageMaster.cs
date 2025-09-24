using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

[System.Serializable]
public class TextEntry
{
    [TextArea(2, 5)]
    public string message;
    public float durationAfter = 2f;
}

public class DialogueMessageMaster : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI uiText;
    [Header("Entries")]
    public List<TextEntry> entries = new List<TextEntry>();

    [Header("Typing & Timing (global)")]
    public bool bAppendWord = true; 
    public float typingSpeed = 0.05f; 

    [Header("Trigger")]
    public bool bTriggeredOnce = true;
    private bool bTriggered = false;
    
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
            uiText.gameObject.SetActive(false);
            uiText.text = "";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || uiText == null) return;

        if (bTriggeredOnce && bTriggered) return;
        bTriggered = true;

        uiText.gameObject.SetActive(true);

        if (currentRoutine != null) StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(PlayEntriesCoroutine());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!bTriggeredOnce)
        {
            uiText.text = "";
            uiText.gameObject.SetActive(false);
        }
    }

    private IEnumerator PlayEntriesCoroutine()
    {
        if (entries == null || entries.Count == 0) yield break;

        for (int i = 0; i < entries.Count; i++)
        {
            TextEntry entry = entries[i];

            if (bAppendWord) yield return StartCoroutine(TypeText(entry.message));
            else uiText.text = entry.message;
            
            yield return new WaitForSeconds(entry.durationAfter);
        }
        if (bTriggeredOnce)
        {
            uiText.text = "";
            uiText.gameObject.SetActive(false);
        }
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
    }
}
