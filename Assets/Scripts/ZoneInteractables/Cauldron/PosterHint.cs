using UnityEngine;
using ZoneInteractables;

public class PosterHint : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private GameObject hintPanel;

    public void Interact(GameObject player)
    {
        if (hintPanel == null)
        {
            Debug.LogWarning("[PosterHint] Hint panel not assigned.");
            return;
        }

        bool show = !hintPanel.activeSelf;
        hintPanel.SetActive(show);

        GameManager.Instance.PauseGame(show ? 1 : 0);
        PlayerController.Instance.isPaused = show;
    }
}
