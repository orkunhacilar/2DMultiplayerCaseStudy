using UnityEngine;

public class ScreenBoundsUpdater : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        UpdateScreenBounds();
    }

    void UpdateScreenBounds()
    {
        Vector2 screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        foreach (PlayerBounds playerBounds in FindObjectsOfType<PlayerBounds>())
        {
            playerBounds.UpdateBounds(screenBounds);
        }
    }
}
