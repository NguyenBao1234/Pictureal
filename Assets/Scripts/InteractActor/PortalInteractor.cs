using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalInteractor : MonoBehaviour, IInteractable
{
    [SerializeField]private string SceneNameToLoad;
    [SerializeField]private Animator FlashUIAnimator;
    [SerializeField] private AudioClip TrasititionSFX;
    
    IEnumerator FlashAndLoad()
    {
        if (string.IsNullOrEmpty(SceneNameToLoad)) yield break;
        if(TrasititionSFX) AudioSource.PlayClipAtPoint(TrasititionSFX, Camera.main.transform.position);
        yield return new WaitForSeconds(1);
        if (FlashUIAnimator) FlashUIAnimator.SetTrigger("FlashIn");
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(SceneNameToLoad);
    }
    public void Interact(GameObject interactor)
    {
        StartCoroutine(FlashAndLoad());
    }
}
