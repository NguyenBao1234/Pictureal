using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalInteractor : MonoBehaviour, IInteractable
{
    [SerializeField]private string SceneNameToLoad;
    [SerializeField]private bool bShowMouseCusor = false;
    [SerializeField]private Animator FlashUIAnimator;
    [SerializeField] private AudioClip TrasititionSFX;
    [SerializeField] private float Volume = 1f;

    
    IEnumerator FlashAndLoad()
    {
        if (string.IsNullOrEmpty(SceneNameToLoad)) yield break;
        if(TrasititionSFX) AudioSource.PlayClipAtPoint(TrasititionSFX, Camera.main.transform.position, Volume);
        yield return new WaitForSeconds(1);
        if (FlashUIAnimator) FlashUIAnimator.SetTrigger("FlashIn");
        yield return new WaitForSeconds(1);
        if(bShowMouseCusor) 
        {        
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        SceneManager.LoadSceneAsync(SceneNameToLoad);
    }
    public void Interact(GameObject interactor)
    {
        StartCoroutine(FlashAndLoad());
    }
}
