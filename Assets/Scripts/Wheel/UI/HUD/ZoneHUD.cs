using UnityEngine;
using UnityEngine.UI;

public class ZoneHUD : MonoBehaviour
{
    [SerializeField] private WheelController controller;
    [SerializeField] private CanvasGroup zoneGroup;
    [SerializeField] private Canvas zoneCanvas;

    [SerializeField] private Button spinButton;
    [SerializeField] private SpinButtonAnimator spinAnimator;
    [SerializeField] private SpinRewardFlyAnimator spinFlyAnimator;

    private SpinButtonAnimator.State _lastTarget;
    private bool _buttonStateInitialized;

    void Awake()
    {
        if (controller == null) { Debug.LogError("[ZoneHUD] controller not assigned", this); enabled = false; return; }
        if (spinButton == null) { Debug.LogError("[ZoneHUD] spinButton not assigned", this); enabled = false; return; }

        spinButton.onClick.AddListener(OnSpin);
    }

    void OnDestroy()
    {
        if (spinButton != null) spinButton.onClick.RemoveListener(OnSpin);
    }

    public void SetDimmed(bool dimmed)
    {
        if (zoneGroup != null)
        {
            zoneGroup.alpha = dimmed
                ? DeathOverlayStyle.ZoneBarOverlayAlpha
                : DeathOverlayStyle.ZoneBarPromotedAlpha;

            zoneGroup.blocksRaycasts = !dimmed;
            zoneGroup.interactable   = !dimmed;
        }

        if (zoneCanvas != null)
        {
            zoneCanvas.sortingOrder = dimmed
                ? UICanvasOrders.RewardListBelowOverlay
                : UICanvasOrders.HUDPromoted;
        }
    }

    void Update()
    {
        if (controller == null) return;

        bool flyBusy = spinFlyAnimator != null && spinFlyAnimator.IsBusy;
        SpinButtonAnimator.State target = (controller.CanSpin && !flyBusy)
            ? SpinButtonAnimator.State.Ready
            : SpinButtonAnimator.State.Spinning;

        if (!_buttonStateInitialized || target != _lastTarget)
        {
            if (spinAnimator != null) spinAnimator.SetState(target);
            else if (spinButton != null)
                spinButton.interactable = target == SpinButtonAnimator.State.Ready;
            _lastTarget = target;
        }
        _buttonStateInitialized = true;
    }

    void OnSpin()
    {
        if (controller == null) return;
        controller.RequestSpin();
    }
}
