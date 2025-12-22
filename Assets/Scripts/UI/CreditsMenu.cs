using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/*
 * credits menu functionality including button to return to main menu and auto-scrolling credits
 */
public class CreditsMenu : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 50;
    private VisualElement frame;
    private Button button;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        frame = GetComponent<UIDocument>().rootVisualElement;
        button = frame.Q<Button>("MenuButton");
        frame = frame.Q<VisualElement>("AutoScrollContainer");
        button.clicked += () => SceneManager.LoadScene("Scenes/MainMenu");
    }

    // Update is called once per frame
    void Update()
    {
        frame.style.translate = frame.resolvedStyle.translate + (Vector3.down * Time.deltaTime * scrollSpeed);
        if (-frame.resolvedStyle.translate.y > frame.resolvedStyle.height + button.resolvedStyle.translate.y + (button.resolvedStyle.height * 2))
            frame.style.translate = Vector3.zero;
    }
}
