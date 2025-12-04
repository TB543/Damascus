using UnityEngine;

public class Collider : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerController.cameraMovedCallback.AddListener(onCameraMove);
        Camera cam = Camera.main;
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        float left = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)).x;
        float right = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.nearClipPlane)).x;
        collider.offset = new Vector2(((right + left) / 2), collider.offset.y);
        collider.size = new Vector2(right - left, collider.size.y);
    }

    private void onCameraMove(float dx)
    {
        GetComponent<BoxCollider2D>().offset += new Vector2(dx, 0);
    }
}
