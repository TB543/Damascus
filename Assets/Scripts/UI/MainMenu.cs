using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/*
 * backend for the main menu handles button functionality
 */
public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // gets all the components
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        Button playButton = root.Q<Button>("PlayButton");
        Button settingsButton = root.Q<Button>("SettingsButton");
        Button creditsButton = root.Q<Button>("CreditsButton");
        Button exitButton = root.Q<Button>("ExitButton");

        // adds button functionality
        playButton.clicked += () => SceneManager.LoadScene("Scenes/Game");
        settingsButton.clicked += Application.Quit;
        creditsButton.clicked += () => SceneManager.LoadScene("Scenes/Credits");
        exitButton.clicked += Application.Quit;
    }
}
