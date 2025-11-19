
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalLevelDoor : MonoBehaviour
{
    public string SceneNameToLoad;
    public Transform DirectionObject;
    public Animator FlashUIAnimator;
    [SerializeField] private bool bShowMouseCusor = false;
    private GameObject SurrealRoom;
    private void OnEnable()
    {
        //If Null error is here, create game object with name matching parameter
        SurrealRoom = transform.parent.transform.Find("SurrealRoom").gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (DirectionObject == null) return;
            var playerVelocity = other.GetComponent<CharacterController>().velocity;
            playerVelocity.y = 0;
            var selfFoward = DirectionObject.forward;
            selfFoward.y = 0;
            var Angle = Vector3.Angle(selfFoward, playerVelocity);
            Debug.Log(Angle);
            if (Angle > 90)
            {
                SurrealRoom.gameObject.GetComponent<MeshRenderer>().enabled = true;
                SurrealRoom.gameObject.GetComponent<MeshCollider>().enabled = true;
                StartCoroutine(FlashAndLoad());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag != "Player") return;
        if (DirectionObject == null) return;
        var playerVelocity = other.GetComponent<CharacterController>().velocity;
        playerVelocity.y = 0;
        var selfFoward = DirectionObject.forward;
        selfFoward.y = 0;
        var Angle = Vector3.Angle(selfFoward, playerVelocity);
        Debug.Log(Angle);
        if (Angle < 90)
        {
            SurrealRoom.gameObject.GetComponent<MeshRenderer>().enabled = false;
            SurrealRoom.gameObject.GetComponent<MeshCollider>().enabled = false;
        }
    }
    void OnDrawGizmos()
    {
        if (DirectionObject == null) return;
        Vector3 selfForward = DirectionObject.forward;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(DirectionObject.position, DirectionObject.position + selfForward * 3);

        Gizmos.DrawSphere(transform.position + selfForward * 3, 0.05f);
    }

    IEnumerator FlashAndLoad()
    {
        if (string.IsNullOrEmpty(SceneNameToLoad)) yield break;
        yield return new WaitForSeconds(1);
        if (FlashUIAnimator != null) FlashUIAnimator.SetTrigger("FlashIn");
        yield return new WaitForSeconds(1);
        if(bShowMouseCusor) 
        {        
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        SceneManager.LoadScene(SceneNameToLoad);
    }
}
