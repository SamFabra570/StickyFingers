using UnityEngine;
using UnityEngine.SceneManagement;

public class PostGameManager : MonoBehaviour
{
    public void BackToHUB()
    {
        SceneManager.LoadScene("HUB");
    }
}
