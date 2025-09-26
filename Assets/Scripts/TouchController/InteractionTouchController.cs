using UnityEngine;
using UnityEngine.UI;

public class InteractionTouchController : MonoBehaviour
{
    PlayerController Player;

    public GameObject TakeShootBtn;
    private Image TakeShootBtnImg;
    public Sprite TakenPhotoStateImg;
    public Sprite UnTakenPhotoStateImg;

    public GameObject OpenPolaroidBtn;
    public GameObject ClosePolaroidBtn;
    
    public GameObject RewindBtn;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        TakeShootBtnImg = TakeShootBtn.GetComponent<Image>();
        if (!Player.bPickedPolaroid)
        {
            OpenPolaroidBtn.SetActive(false);
            RewindBtn.SetActive(false);
        }
    }

    public void OnActionTouch()
    {
        TakeShootBtn.SetActive(true);
        Player.TouchAction();
        OpenPolaroidBtn.SetActive(false);
        ClosePolaroidBtn.SetActive(true);
    }

    public void OnSubActionTouch()
    {
        Player.TouchSubAction();
        if (!Player.GetCameraPolaroid().IsTakenPhoto())
        {
            TakeShootBtn.SetActive(false);
            ClosePolaroidBtn.SetActive(false);
            OpenPolaroidBtn.SetActive(true);
        }
    }

    public void OnTakeShootTouch()
    {
        Player.TouchAction();
        if (Player.GetCameraPolaroid().IsTakenPhoto()) TakeShootBtnImg.sprite = TakenPhotoStateImg;
        else
        {
            TakeShootBtnImg.sprite = UnTakenPhotoStateImg;
            TakeShootBtn.SetActive(false);
            ClosePolaroidBtn.SetActive(false);
            OpenPolaroidBtn.SetActive(true);
        }
    }

    public void OnPauseTouch() => Player.TouchPauseGame();

    public void OnRewindPressedTouch()
    {
        Player.TouchStartRewind();
        if(RewindBtn != null) RewindBtn.GetComponent<Image>().enabled = false;
    }

    public void OnRewindReleasedTouch()
    {
        Player.TouchStopRewind();
        if(RewindBtn != null) RewindBtn.GetComponent<Image>().enabled = true;
    }

    public void OnPickUpPolaroid()
    {
        OpenPolaroidBtn.SetActive(true);
        RewindBtn.SetActive(true);
    }
}
