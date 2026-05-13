using UnityEngine;

public class BlurBackgroundOverlay : MonoBehaviour
{
    [SerializeField] private CanvasGroup blurGroup;
    [SerializeField] private float fadeDuration = 0.2f;

    private float target_alpha;

    void OnEnable()
    {
        SetVisible(false);
    }

    void Update()
    {
        if (blurGroup == null) return;
        float current = blurGroup.alpha;
        if (Mathf.Approximately(current, target_alpha))
        {
            enabled = false;
            return;
        }

        float step = fadeDuration > 0f ? Time.unscaledDeltaTime / fadeDuration : 1f;
        blurGroup.alpha = Mathf.MoveTowards(current, target_alpha, step);

        bool visible = blurGroup.alpha > 0.01f;
        blurGroup.blocksRaycasts = visible;
        blurGroup.interactable = visible;
    }

    public void SetVisible(bool visible)
    {
        target_alpha = visible ? 1f : 0f;
        enabled = true;
    }
}
