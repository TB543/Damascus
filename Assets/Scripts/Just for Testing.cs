using System.Collections;
using UnityEngine;

public class JustforTesting : MonoBehaviour
{
    [SerializeField] private GameObject[] heros;
    [SerializeField] private GameObject playerController;

    private int index;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnNextAfterFrame());
    }

    public void left()
    {
        Destroy(heros[index].transform.GetChild(0).gameObject);
        index = index == 0 ? heros.Length - 1 : --index;
        StartCoroutine(SpawnNextAfterFrame());
    }

    public void right()
    {
        Destroy(heros[index].transform.GetChild(0).gameObject);
        index = index == heros.Length - 1 ? 0 : ++index;
        StartCoroutine(SpawnNextAfterFrame());
    }

    private IEnumerator SpawnNextAfterFrame()
    {
        yield return null; // wait until the end of this frame
        Instantiate(playerController, heros[index].transform);
    }
}
