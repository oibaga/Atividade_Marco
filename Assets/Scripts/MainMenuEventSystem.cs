using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuEventSystem : MonoBehaviour
{
    public Texture2D defaultCursor;
    void Awake()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
