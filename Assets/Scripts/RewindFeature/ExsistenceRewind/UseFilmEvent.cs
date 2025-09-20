using UnityEngine;

public class UseFilmEvent : RewindableEvent
{
    public UseFilmEvent (GameObject inObjectHoldCamera) => GameObjectTarget = inObjectHoldCamera;
    public override void Rewind()
    {
        var photoCamera = GameObjectTarget.GetComponent<PhotoCamera>();
        if (photoCamera == null) return;
        photoCamera.AddToRemainingFilm(1);
        Debug.Log(photoCamera + "Has Rewind");
    }
}
