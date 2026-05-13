using UnityEngine;
using UnityEngine.UI;


//spinin durumlarını buglammadan basic yapılarla bu partta bölüp tamamladım

public class SpinButtonAnimator : MonoBehaviour
{
    public enum State
    {
        Ready,
        Spinning,
        Cooldown,
        Skippable,
    }

    [SerializeField] private Button button;

    [Header("Button Tints")]
    [Tooltip("Hiçbir etkileşim yokken butonun rengi.")]
    [SerializeField] private Color normalTint      = Color.white;
    [Tooltip("Fareyle üzerine gelince / seçiliyken butonun rengi.")]
    [SerializeField] private Color highlightedTint = new Color(0.96f, 0.96f, 1.00f, 1f);
    [Tooltip("Tıklanırken butonun rengi.")]
    [SerializeField] private Color pressedTint     = new Color(0.78f, 0.80f, 0.92f, 1f);
    [Tooltip("Pasifken (interactable=false) butonun rengi.")]
    [SerializeField] private Color disabledTint    = new Color(0.55f, 0.58f, 0.70f, 1f);

    [Header("Color Transition")]
    [Tooltip("Bir state'ten diğerine geçerken rengin yumuşatma süresi (saniye).")]
    [Min(0f)]//bunu kaymadığım zaman, çok problem oldu, unutma!
    [SerializeField] private float colorFadeDuration = 0.1f;

    private State _state = State.Ready;

    void Awake()
    {
        if (button == null) Debug.LogError("SpinButtonAnimator: button not wired.", this);
        ConfigureColorBlock();
        ApplyState(_state);
    }

    public void SetState(State newState)
    {
        if (_state == newState) return;
        _state = newState;
        ApplyState(newState);
    }

    public State CurrentState => _state;

    private void ApplyState(State s)
    {
        if (button is null)
        {
            return;
        }
        
        
        bool interactable = (s == State.Ready || s == State.Skippable);

        if (button.interactable != interactable)
            button.interactable = interactable;
    }

    private void ConfigureColorBlock()
    {
        if (button == null) return;
        ColorBlock cb = button.colors;
        cb.normalColor      = normalTint;
        cb.highlightedColor = highlightedTint;
        cb.pressedColor     = pressedTint;
        cb.selectedColor    = highlightedTint;
        cb.disabledColor    = disabledTint;
        cb.colorMultiplier  = 1f;
        cb.fadeDuration     = colorFadeDuration;
        button.colors = cb;
    }

    void Reset()
    {
        button = GetComponent<Button>();
    }
}
