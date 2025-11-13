using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class DialogueInteractor : MonoBehaviour, IInteractable
{
    [Header("Callback")]
    [SerializeField] private UnityEvent onDialogueComplete;
    [SerializeField] private bool bCallbackOnce = true;
    private bool bCalledback = false;
    [Header("Trigger")]
    [SerializeField] private bool bTriggeredOnce = true;
    private bool bTriggered = false;
    
    [Header("Dialogue Behavior")]
    [SerializeField] private bool bAppendWord = true; 
    [SerializeField] private float typingSpeed = 0.05f; 
    [SerializeField] private TextMeshProUGUI uiText;
    public List<DialogueStruct> Dialogues = new List<DialogueStruct>();
    
    [Header("Audio")]
    [SerializeField] private AudioClip charSound;  
    [SerializeField] private float charVolume = 0.6f; 
    [SerializeField] private bool skipSpaces = true;  
    [Tooltip("Phát sound mỗi N ký tự")]
    [SerializeField] private int charsPerSound = 1;
    private int currentDialogueIndex = 0;
    private Coroutine currentRoutine;
    
    
    private AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
    void Start()
    {
        if (uiText != null)
        {
            uiText.gameObject.SetActive(false);
            uiText.text = "";
        }
    }

    public void Interact(GameObject interactor)
    {
        Debug.Log(gameObject.name + " was interacted by " + interactor.name);
        if (bTriggeredOnce && bTriggered) return;
        if(!bTyping && currentRoutine == null) currentRoutine = StartCoroutine(PlayDialogueCoroutine(currentDialogueIndex));
    }
    bool bTyping = false;
    private IEnumerator PlayDialogueCoroutine(int inIndex)
    {
        if (Dialogues == null || Dialogues.Count == 0 || !uiText) yield break;
        
        var dialogue = Dialogues[inIndex];

        bTyping = true;
        uiText.gameObject.SetActive(true);
        if (bAppendWord) yield return StartCoroutine(TypeText(dialogue.message));
        else uiText.text = dialogue.message;

        yield return new WaitForSeconds(dialogue.durationAfter);
        bool bLastDialogue = (currentDialogueIndex == Dialogues.Count - 1);
        if (bLastDialogue)
        {
            uiText.text = "";
            uiText.gameObject.SetActive(false);
            bTriggered = true;
        }
        currentRoutine = null;
        currentDialogueIndex ++;
        if(currentDialogueIndex >= Dialogues.Count) currentDialogueIndex = 0;

        if (!bLastDialogue) yield break;
        if(bCallbackOnce && bCalledback) yield break;
        onDialogueComplete?.Invoke();
        bCalledback = true;
    }
    private IEnumerator TypeText(string text)
    {
        uiText.text = "";
        int soundCounter = 0;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            uiText.text += c;

            if (charSound&& audioSource)
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
        if (audioSource && audioSource.isPlaying) audioSource.Stop();
        if (!uiText.IsActive()) uiText.gameObject.SetActive(true);
        bTyping = false;
    }
}
