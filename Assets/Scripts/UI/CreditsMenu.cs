using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class AutoScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 50;
    private VisualElement frame;
    private VisualElement button;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        frame = GetComponent<UIDocument>().rootVisualElement;
        button = frame.Q<Button>("MenuButton");
        frame = frame.Q<VisualElement>("AutoScrollContainer");
        button.RegisterCallback<ClickEvent>(OnMenuButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
        frame.style.translate = frame.resolvedStyle.translate + (Vector3.down * Time.deltaTime * scrollSpeed);
        if (-frame.resolvedStyle.translate.y > frame.resolvedStyle.height + button.resolvedStyle.translate.y + (button.resolvedStyle.height * 2))
            frame.style.translate = Vector3.zero;
    }

    private void OnMenuButtonClicked(ClickEvent evt)
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }
}
