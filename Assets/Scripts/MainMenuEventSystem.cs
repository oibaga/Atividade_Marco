using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuEventSystem : MonoBehaviour
{
    public Texture2D defaultCursor;
    public Texture2D clickableCursor;
    public GameObject credits;
    public GameObject popup;
    public Button creditsButton;
    public Button playButton;
    void Awake()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        credits.SetActive(false);
        popup.SetActive(true);
        creditsButton.enabled = true;
        playButton.enabled = true;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void OpenCredits()
    {
        credits.SetActive(true);
        popup.SetActive(false);
        creditsButton.enabled = false;
        playButton.enabled = false;
    }
    public void CloseCredits()
    {
        credits.SetActive(false);
        popup.SetActive(true);
        creditsButton.enabled = true;
        playButton.enabled = true;
    }

    public void Clickable()
    {
        Cursor.SetCursor(clickableCursor, Vector2.zero, CursorMode.Auto);
    }

    public void Default()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }
}
