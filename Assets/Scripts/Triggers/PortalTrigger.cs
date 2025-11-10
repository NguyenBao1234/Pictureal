using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalTrigger : MonoBehaviour
{
    [SerializeField]private string SceneNameToLoad;
    [SerializeField]private Animator FlashUIAnimator;
    [SerializeField] private AudioClip TrasititionSFX;
    bool bTriggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")||bTriggered) return;
        StartCoroutine(FlashAndLoad());
        bTriggered = true;
    }
    
    IEnumerator FlashAndLoad()
    {
        if (string.IsNullOrEmpty(SceneNameToLoad)) yield break;
        if(TrasititionSFX) AudioSource.PlayClipAtPoint(TrasititionSFX, Camera.main.transform.position);
        yield return new WaitForSeconds(1);
        if (FlashUIAnimator) FlashUIAnimator.SetTrigger("FlashIn");
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(SceneNameToLoad);
    }
}
