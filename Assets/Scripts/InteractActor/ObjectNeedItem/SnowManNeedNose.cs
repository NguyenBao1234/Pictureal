using System.Collections;
using TMPro;
using UnityEngine;

public class SnowManNeedNose : ObjectNeedItem
{
    [SerializeField] private Transform NewRotation;
    [SerializeField] private GameObject KeyObjectRepresentation;
    [SerializeField] private TextMeshProUGUI InteractionText;
    [SerializeField] private GameObject Hint;
    protected override void InteractionByItem(GameObject ItemFromInteract)
    {
        base.InteractionByItem(ItemFromInteract);
        KeyObjectRepresentation.SetActive(true);
        Destroy(ItemFromInteract);
        StartCoroutine(InteractionSequence());
    }
    private IEnumerator InteractionSequence()
    {
        InteractionText.gameObject.SetActive(true);
        InteractionText.text = "Cảm ơn bạn";
        yield return new WaitForSeconds(0.5f);
        InteractionText.text = "Bạn đang tìm cánh cửa đúng không? Hãy đi theo mũi của tôi";
        gameObject.transform.rotation = NewRotation.rotation;
        Destroy(Hint);
        yield return new WaitForSeconds(5);
        InteractionText.text = "";
        InteractionText.gameObject.SetActive(false);
    }
}
