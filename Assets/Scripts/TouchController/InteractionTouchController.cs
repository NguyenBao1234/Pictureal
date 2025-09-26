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
    [Tooltip("Object contain buttons hidden when open polaroid")]
    public GameObject ButtunBeyondPolaroid;
    
    public GameObject RewindBtn;
    public Image PauseBtnImg;
    public Sprite PauseImg;
    public Sprite ContinueImg;
    
    
    
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
        if(ButtunBeyondPolaroid) ButtunBeyondPolaroid.SetActive(false);
    }

    public void OnSubActionTouch()
    {
        Player.TouchSubAction();
        if (!Player.GetCameraPolaroid().IsTakenPhoto())
        {
            //Only back to visible if player has not taken photo
            TakeShootBtn.SetActive(false);
            ClosePolaroidBtn.SetActive(false);
            OpenPolaroidBtn.SetActive(true);
            if(ButtunBeyondPolaroid) ButtunBeyondPolaroid.SetActive(true);
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
            if(ButtunBeyondPolaroid) ButtunBeyondPolaroid.SetActive(true);
        }
    }

    public void OnPauseTouch()
    {
        Player.TouchPauseGame();
        if(Player.GetPausing()) PauseBtnImg.sprite = ContinueImg;
        else  PauseBtnImg.sprite = PauseImg;
    }

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

    public void OnJumpTouch()
    {
        Player.TouchJump();
    }

    public void OnSprintTouch()
    {
        Player.TouchSprint();
    }
}
