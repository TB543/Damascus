using System;
using System.Collections.Generic;
using UnityEngine;

public class BGSpriteSpawner : MonoBehaviour
{
    [SerializeField] private Sprite image;
    [SerializeField] private int zLayer;
    [SerializeField] bool isBaseLayer;

    public bool IsBaseLayer => isBaseLayer;
    public int ZLayer => zLayer;
    int baseLayer;
    private LinkedList<GameObject> sprites = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        PlayerMovementController.cameraMovedCallback.AddListener(onCameraMove);
        checkSprites();

        // gets base layer
        foreach (Transform sibling in transform.parent)
        {
            BGSpriteSpawner layer = sibling.gameObject.GetComponent<BGSpriteSpawner>();
            if (isBaseLayer && layer is not null && layer.IsBaseLayer && zLayer != layer.ZLayer)
                throw new InvalidOperationException("Only 1 z-level can be set as the base layer");
            else if (layer is not null && layer.IsBaseLayer)
                baseLayer = layer.ZLayer;
        }
    }

    void checkSprites()
    {
        // get the screen bbox in world coordinates
        Camera cam = Camera.main;
        Vector3 bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 topRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.nearClipPlane));

        // place sprites as needed
        float scale = (topRight - bottomLeft).y / image.bounds.size.y;
        if (sprites.Count == 0)
            spawnSprite(scale);
        while (sprites.First.Value.GetComponent<SpriteRenderer>().bounds.min.x > bottomLeft.x)
            spawnSprite(scale);
        while (sprites.Last.Value.GetComponent<SpriteRenderer>().bounds.max.x < topRight.x)
            spawnSprite(scale, true);

        // removes sprites as needed
        while (sprites.First.Value.GetComponent<SpriteRenderer>().bounds.max.x < bottomLeft.x)
        {
            Destroy(sprites.First.Value);
            sprites.RemoveFirst();
        }
        while (sprites.Last.Value.GetComponent<SpriteRenderer>().bounds.min.x > topRight.x)
        {
            Destroy(sprites.Last.Value);
            sprites.RemoveLast();
        }
    }
    private void spawnSprite(float scale, bool right = false)
    {
        // gets sprite position
        Vector3 position = Vector3.zero;
        float offset = (image.bounds.size.x / 2) * scale;
        if (right)
            position = new Vector3(sprites.Last.Value.GetComponent<SpriteRenderer>().bounds.max.x + offset, 0, 0);
        else if (sprites.Count != 0)
            position = new Vector3(sprites.First.Value.GetComponent<SpriteRenderer>().bounds.min.x - offset, 0, 0);
        position.z = zLayer;

        // crates sprite
        GameObject sprite = new GameObject($"Sprite {position}");
        sprite.transform.parent = transform;
        sprite.transform.position = position;
        sprite.transform.localScale = new Vector3(scale, scale, 1);
        sprite.AddComponent<SpriteRenderer>().sprite = image;
        if (right)
            sprites.AddLast(sprite);
        else
            sprites.AddFirst(sprite);
    }

    private void onCameraMove(float dx)
    {
        transform.position += new Vector3(dx - (dx * Mathf.Pow(.75f, zLayer - baseLayer)), 0, 0);
        checkSprites();
    }
}
