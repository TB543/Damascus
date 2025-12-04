using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
        playButton.RegisterCallback<ClickEvent>(OnPlayButtonClicked);
        settingsButton.RegisterCallback<ClickEvent>(OnSettingsButtonClicked);
        creditsButton.RegisterCallback<ClickEvent>(OnCreditsButtonClicked);
        exitButton.RegisterCallback<ClickEvent>(OnExitButtonClicked);
    }

    private void OnPlayButtonClicked(ClickEvent evt)
    {
        SceneManager.LoadScene("Scenes/Game");
    }

    private void OnSettingsButtonClicked(ClickEvent evt)
    {
    }

    private void OnCreditsButtonClicked(ClickEvent evt)
    {
        SceneManager.LoadScene("Scenes/Credits");
    }

    private void OnExitButtonClicked(ClickEvent evt)
    {
        Application.Quit();
    }
}
