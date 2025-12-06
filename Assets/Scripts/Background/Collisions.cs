using UnityEngine;

public class Collisions : MonoBehaviour
{
    private BoxCollider2D boxCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CharacterBehavior.cameraMovedCallback.AddListener(onCameraMove);
        Camera cam = Camera.main;
        boxCollider = GetComponent<BoxCollider2D>();
        float left = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)).x;
        float right = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.nearClipPlane)).x;
        boxCollider.offset = new Vector2(((right + left) / 2), boxCollider.offset.y);
        boxCollider.size = new Vector2(right - left, boxCollider.size.y);
    }

    private void onCameraMove(float dx)
    {
        boxCollider.offset += new Vector2(dx, 0);
    }
}
