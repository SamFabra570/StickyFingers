using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InputIconImage : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite keyboardSprite;
    [SerializeField] private Sprite xboxSprite;
    [SerializeField] private Sprite playstationSprite;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        //Debug.Log($"{name} enabled");

        InputIconManager.OnInputTypeChanged += UpdateIcon;

        if (InputIconManager.Instance != null)
            UpdateIcon(InputIconManager.Instance.CurrentInputType);
    }

    private void OnDisable()
    {
        InputIconManager.OnInputTypeChanged -= UpdateIcon;
    }

    public void UpdateIcon(InputType inputType)
    {
        //Debug.Log($"{name} updating to {inputType}");
        
        switch (inputType)
        {
            case InputType.Keyboard:
                image.sprite = keyboardSprite;
                break;

            case InputType.Xbox:
                image.sprite = xboxSprite;
                break;

            case InputType.PlayStation:
                image.sprite = playstationSprite;
                break;
        }
        
        //Debug.Log(image.sprite.name);
    }
}