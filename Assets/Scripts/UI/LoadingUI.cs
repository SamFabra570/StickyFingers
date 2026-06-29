using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    public static LoadingUI Instance;

    public GameObject loadingScreen;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (loadingScreen.activeSelf) 
            loadingScreen.SetActive(false);
    }

    public void Show()
    {
        loadingScreen.SetActive(true);
    }

    public void Hide()
    {
        loadingScreen.SetActive(false);
    }

}
