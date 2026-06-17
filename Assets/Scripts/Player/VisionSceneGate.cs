using UnityEngine;
using UnityEngine.SceneManagement;

// The player persists across scenes (DontDestroyOnLoad on PlayerController), so its vision/stealth
// stack — the cone visual, the detection scan, the fog-of-war revealer — would otherwise keep
// running in non-gameplay scenes like the HUB, where it has no targets and just looks wrong.
// This gate turns those objects/behaviours on ONLY in the gameplay scene, mirroring the existing
// SceneManager.GetActiveScene().name == "Game" checks already used throughout PlayerController.
public class VisionSceneGate : MonoBehaviour
{
    [Tooltip("Scene where the vision/stealth stack is active. Everywhere else it is turned off.")]
    [SerializeField] private string gameplaySceneName = "Game";

    [Tooltip("GameObjects active ONLY in the gameplay scene. Assign the 'Vision' cone-visual child here.")]
    [SerializeField] private GameObject[] gameplayObjects;

    [Tooltip("Behaviours enabled ONLY in the gameplay scene. Assign PlayerVisionCone (scan) and FogOfWarRevealer here.")]
    [SerializeField] private Behaviour[] gameplayBehaviours;

    private void Awake()
    {
        // Awake runs once — the player is DontDestroyOnLoad. Apply for the scene we boot into,
        // then react to every later scene change (the persistent player outlives scene loads).
        Apply(SceneManager.GetActiveScene().name);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => Apply(scene.name);

    private void Apply(string sceneName)
    {
        bool inGame = sceneName == gameplaySceneName;

        // Objects first: activating the cone visual runs its Awake (which builds the shared cone mesh)
        // BEFORE we enable the behaviours that read it (e.g. FogOfWarRevealer sharing ConeMesh).
        if (gameplayObjects != null)
            foreach (GameObject go in gameplayObjects)
                if (go != null) go.SetActive(inGame);

        if (gameplayBehaviours != null)
            foreach (Behaviour b in gameplayBehaviours)
                if (b != null) b.enabled = inGame;
    }
}
