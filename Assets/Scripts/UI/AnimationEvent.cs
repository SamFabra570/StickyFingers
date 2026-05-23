using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    public void AnimStateDone()
    {
        HUB_UIManager.Instance.isAnimDone = true;
    }
}
