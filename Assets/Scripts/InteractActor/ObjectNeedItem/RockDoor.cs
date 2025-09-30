using System.Collections;
using TMPro;
using UnityEngine;

public class RockDoor : ObjectNeedItem
{
    [SerializeField] private Transform RockDoorTransform;
    [SerializeField] private Transform TargetTransform;
    [SerializeField] private float speed = 2;
    [SerializeField] private string Message = "Vừng ơi mở ra";
    [SerializeField] private GameObject KeyObjectRepresentation;
    [SerializeField] private TextMeshProUGUI InteractionText;
    protected override void InteractionByItem(GameObject ItemFromInteract)
    {
        base.InteractionByItem(ItemFromInteract);
        KeyObjectRepresentation.SetActive(true);
        Destroy(ItemFromInteract);
       StartCoroutine(MoveDoorCoroutine());
    }
    private IEnumerator MoveDoorCoroutine()
    {
        InteractionText.text = Message;
        while (Vector3.Distance(RockDoorTransform.position, TargetTransform.position) > 0.01f)
        {
            RockDoorTransform.position = Vector3.MoveTowards(
                RockDoorTransform.position,
                TargetTransform.position,
                speed * Time.deltaTime
            );
            yield return null;
        }
        RockDoorTransform.position = TargetTransform.position;
        InteractionText.text = "";
    }
}
