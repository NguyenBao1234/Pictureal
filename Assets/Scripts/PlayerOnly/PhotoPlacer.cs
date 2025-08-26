using UnityEngine;

public class PhotoPlacer : MonoBehaviour
{
    public Camera playerCam;
    public GameObject worldPrefab; // Prefab sẽ spawn khi thả ảnh

    private GameObject heldPhoto;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (heldPhoto)
        {
            heldPhoto.transform.position = playerCam.transform.position + playerCam.transform.forward * 2f;
            heldPhoto.transform.rotation = playerCam.transform.rotation;
        }
    }
    
    public void HoldPhoto(GameObject photo)
    {
        heldPhoto = photo;
    }

    void PlacePhoto()
    {
        // Lấy vị trí đặt ảnh
        Vector3 placePos = playerCam.transform.position + playerCam.transform.forward * 5f;

        // Spawn thế giới từ prefab
        Instantiate(worldPrefab, placePos, Quaternion.identity);

        // Gắn lại ảnh ở đó (hoặc hủy nếu không cần)
        heldPhoto.transform.position = placePos;
        heldPhoto = null;
    }
}
