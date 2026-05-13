using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class RunExitController : MonoBehaviour
{
    [Header("Game references")]
    [SerializeField] private WheelController wheel;
    [SerializeField] private ExitConfirmPanel exitConfirmPanel;
    [SerializeField] private DeathGameOverPanel deathPanel;
    [SerializeField] private DeathConfirmPanel deathConfirmPanel;
    [SerializeField] private RewardCollectAnimator collectAnimator;

    [Header("UI references (controller pushes state to these)")]
    [SerializeField] private ZoneHUD zoneHUD;
    [SerializeField] private BlurBackgroundOverlay blurOverlay;

    [Header("Exit button")]
    [SerializeField] private Button exitButton;

    private ExitFlowState state = ExitFlowState.None;

    private bool _reviveInFlight;

    public ExitFlowState State => state;

    public event Action<ExitFlowState> OnStateChanged;

    void OnEnable()
    {
        if (wheel != null) wheel.OnDeathHit += HandleDeathHit;
        if (exitButton != null) exitButton.onClick.AddListener(PressExit);
    }

    void OnDisable()
    {
        if (wheel != null) wheel.OnDeathHit -= HandleDeathHit;
        if (exitButton != null) exitButton.onClick.RemoveListener(PressExit);
    }

    public void PressExit()
    {
        if (wheel == null) return;

        ExitKind kind = ExitContext.Classify(wheel);

        if (kind == ExitKind.FreshStart)
        {
            SetState(ExitFlowState.FreshStartConfirm);
        }
        else if (kind == ExitKind.SafeExit)
        {
            SetState(ExitFlowState.CollectConfirm);
        }
        else
        {
            NotifyExitUnavailable();
        }
    }

    public event Action OnExitUnavailable;

    private void NotifyExitUnavailable()
    {
        OnExitUnavailable?.Invoke();
        if (exitButton != null)
        {
            Tween.PunchScale(exitButton.transform, new Vector3(0.08f, 0.08f, 0f), 0.25f, useUnscaledTime: true);
        }
    }

    public void ConfirmFreshStart()
    {
        SetState(ExitFlowState.None);
        if (wheel != null) wheel.Restart();
    }

    public void ConfirmCollect()
    {
        SetState(ExitFlowState.None);
        if (collectAnimator != null) collectAnimator.PlayCollectAndLeave();
    }

    public void CancelExit()
    {
        SetState(ExitFlowState.None);
    }

    public void PressGiveUp()
    {
        SetState(ExitFlowState.GiveUpConfirm);
    }

    public void ConfirmLoseRewards()
    {
        SetState(ExitFlowState.None);
        if (wheel != null) wheel.Restart();
    }

    public void CancelGiveUp()
    {
        SetState(ExitFlowState.DeathSkull);
    }

    public bool PressRevive()
    {
        if (wheel == null) return false;
        if (_reviveInFlight) return false;
        _reviveInFlight = true;
        if (!wheel.TryRevive()) { _reviveInFlight = false; return false; }
        SetState(ExitFlowState.None);
        return true;
    }

    private void HandleDeathHit()
    {
        _reviveInFlight = false;
        SetState(ExitFlowState.DeathSkull);
    }

    private void SetState(ExitFlowState newState)
    {
        if (state == newState) return;
        state = newState;

        bool isConfirmingExit = state == ExitFlowState.CollectConfirm
                             || state == ExitFlowState.FreshStartConfirm;
        bool isDeathFlow      = state == ExitFlowState.DeathSkull
                             || state == ExitFlowState.GiveUpConfirm;
        bool isUIOverlay      = isConfirmingExit || isDeathFlow;

        if (zoneHUD != null)           zoneHUD.SetDimmed(isUIOverlay);
        if (blurOverlay != null)       blurOverlay.SetVisible(isConfirmingExit);
        if (exitConfirmPanel != null)  exitConfirmPanel.Apply(state);
        if (deathPanel != null)        deathPanel.Apply(state);
        if (deathConfirmPanel != null) deathConfirmPanel.Apply(state);

        OnStateChanged?.Invoke(state);
    }

}
