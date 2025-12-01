using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class AutoScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 50;
    private VisualElement root;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        Button menuButton = root.Q<Button>("MenuButton");
        root = root.Q<VisualElement>("AutoScrollContainer");
        menuButton.RegisterCallback<ClickEvent>(OnMenuButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
        root.transform.position += Vector3.down * Time.deltaTime * scrollSpeed;
        if (-root.transform.position.y > root.resolvedStyle.height)
            root.transform.position = Vector3.zero;
    }

    private void OnMenuButtonClicked(ClickEvent evt)
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }
}
