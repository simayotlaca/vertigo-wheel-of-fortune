using System;
using System.Collections;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MetaProgressRowUI : MonoBehaviour
{
    private const byte glow_alpha = 70;

    [Header("Shake Animation")]
    [Tooltip("Unlock anında satırın sallanma hızı (yüksek = hızlı titreşim).")]
    [Min(0f)]
    [SerializeField] private float shakeSpeed = 60f;
    [Tooltip("Sallanma genliği (px).")]
    [Min(0f)]
    [SerializeField] private float shakeMagnitude = 4f;
    [Tooltip("Sallanma toplam süresi (saniye).")]
    [Min(0f)]
    [SerializeField] private float shakeDuration = 0.12f;

    [Header("Unlock — Flash Overlay")]
    [Tooltip("Flash overlay'in tepe alpha değeri (parlama yoğunluğu).")]
    [Range(0f, 1f)]
    [SerializeField] private float flashAlphaPeak = 0.85f;
    [Tooltip("Flash fade-in süresi (siyahtan parlamaya).")]
    [Min(0f)]
    [SerializeField] private float flashFadeInDuration = 0.06f;
    [Tooltip("Flash fade-out süresi (parlamadan siyaha).")]
    [Min(0f)]
    [SerializeField] private float flashFadeOutDuration = 0.14f;

    [Header("Unlock — Icon Punch")]
    [Tooltip("Icon zıplama büyüklüğü (0.18 = %18 büyür).")]
    [Range(0f, 1f)]
    [SerializeField] private float iconPunchScale = 0.18f;
    [Tooltip("Icon zıplama süresi.")]
    [Min(0f)]
    [SerializeField] private float iconPunchDuration = 0.22f;
    [Tooltip("Icon zıplamadan önce bekleme.")]
    [Min(0f)]
    [SerializeField] private float iconPunchDelay = 0.06f;
    [Tooltip("Border fade-in süresi (icon ile birlikte).")]
    [Min(0f)]
    [SerializeField] private float borderFadeInDuration = 0.15f;

    [Header("Unlock — Badge")]
    [Tooltip("Badge'in icon punch'tan sonra ne kadar bekleyip gelmesi.")]
    [Min(0f)]
    [SerializeField] private float badgeDelay = 0.14f;
    [Tooltip("Badge yazısı fade-in süresi.")]
    [Min(0f)]
    [SerializeField] private float badgeFadeInDuration = 0.12f;
    [Tooltip("Badge zıplama büyüklüğü.")]
    [Range(0f, 1f)]
    [SerializeField] private float badgePunchScale = 0.25f;
    [Tooltip("Badge zıplama süresi.")]
    [Min(0f)]
    [SerializeField] private float badgePunchDuration = 0.20f;

    [Header("Unlock — Pre-Exit Delay & Fly-out")]
    [Tooltip("Shake'ten önce bekleme.")]
    [Min(0f)]
    [SerializeField] private float preShakeDelay = 0.16f;
    [Tooltip("Shake bittikten sonra fly-out'tan önce bekleme.")]
    [Min(0f)]
    [SerializeField] private float preFlyOutDelay = 0.97f;
    [Tooltip("Fly-out shrink süresi (satır 0'a düşene kadar).")]
    [Min(0f)]
    [SerializeField] private float flyOutDuration = 0.35f;
    [Tooltip("Fly-out sonrası temizliğe bekleme.")]
    [Min(0f)]
    [SerializeField] private float postFlyOutDelay = 0.35f;

    [Header("Pulse (on AnimateTo points change)")]
    [Tooltip("Pulse büyüklüğü (1.05 = %5 büyür).")]
    [Min(1f)]
    [SerializeField] private float pulseScale = 1.05f;
    [Tooltip("Pulse süresi (geri 1.0'a OutBack ile döner).")]
    [Min(0f)]
    [SerializeField] private float pulseDuration = 0.30f;

    [SerializeField] private Image weaponIcon;
    [SerializeField] private TMP_Text nameLabel;
    [SerializeField] private Image barFill;
    [SerializeField] private TMP_Text amountLabel;
    [SerializeField] private GameObject unlockedBadge;
    [SerializeField] private TMP_Text unlockedBadgeLabel;
    [SerializeField] private RectTransform rowRoot;
    [SerializeField] private float animDuration = 0.6f;

    [SerializeField] private Image rarityEdge;
    [SerializeField] private Image iconGlow;
    [SerializeField] private Image iconFrame;
    [SerializeField] private Image border;
    [SerializeField] private Image flashOverlay;
    [SerializeField] private Image barHighlight;

    private WeaponProgressDefinition def;
    private int displayed_points;
    private int anim_from_points;
    private int anim_to_points;
    private float anim_elapsed;
    private bool animating;
    private Action anim_on_complete;
    private Tween pulse_tween;

    public WeaponProgressDefinition Definition => def;

    public void Bind(WeaponProgressDefinition d, RewardDefinition reward, int currentPoints)
    {
        def = d;
        if (d == null) return;

        Color32 rarity = RarityColor(reward);

        if (weaponIcon != null)
        {
            weaponIcon.sprite = reward == null ? null : (reward.wheelIcon != null ? reward.wheelIcon : reward.icon);
            weaponIcon.preserveAspect = true;

            weaponIcon.color = Color.white;
        }
        if (nameLabel != null) nameLabel.text = d.displayName;

        if (rarityEdge != null) rarityEdge.color = rarity;
        if (barFill   != null) barFill.color   = rarity;
        if (iconGlow  != null)
        {

            Color32 g = rarity; g.a = glow_alpha; iconGlow.color = g;
        }

        if (border != null)
        {
            Color32 b = rarity; b.a = 0; border.color = b;
        }
        if (flashOverlay != null)
        {
            Color32 f = flashOverlay.color; f.a = 0; flashOverlay.color = f;
        }
        if (unlockedBadgeLabel != null)
            unlockedBadgeLabel.color = rarity;

        displayed_points = currentPoints;
        animating = false;
        SetAmount(currentPoints);
        SetBarFill(currentPoints);
        if (unlockedBadge != null) unlockedBadge.SetActive(currentPoints >= d.requiredPoints);
    }

    private static Color32 RarityColor(RewardDefinition reward)
    {
        if (reward == null) return new Color32(0x8A, 0x90, 0x9A, 255);
        switch (reward.minZoneTier)
        {
            case RewardTier.Super: return new Color32(0xF4, 0x8A, 0x3A, 255);
            case RewardTier.Safe:  return new Color32(0x4C, 0x9C, 0xF0, 255);
            default:               return new Color32(0x8A, 0x90, 0x9A, 255);
        }
    }

    public void AnimateTo(int oldPoints, int newPoints, Action onComplete = null)
    {
        if (def == null) { onComplete?.Invoke(); return; }
        if (newPoints == oldPoints)
        {
            SetAmount(newPoints);
            SetBarFill(newPoints);
            onComplete?.Invoke();
            return;
        }

        anim_on_complete = onComplete;
        anim_from_points = oldPoints;
        anim_to_points = newPoints;
        anim_elapsed = 0f;
        animating = true;
        enabled = true;

        PlayPulse();
    }

    void Update()
    {
        if (!animating || def == null) { enabled = false; return; }
        anim_elapsed += Time.deltaTime;
        float t = animDuration <= 0f ? 1f : Mathf.Clamp01(anim_elapsed / animDuration);
        float eased = 1f - Mathf.Pow(1f - t, 3f);
        int v = Mathf.RoundToInt(Mathf.Lerp(anim_from_points, anim_to_points, eased));
        if (v != displayed_points)
        {
            displayed_points = v;
            SetAmount(v);
            SetBarFill(v);
        }
        if (t >= 1f)
        {
            animating = false;
            displayed_points = anim_to_points;
            SetAmount(anim_to_points);
            SetBarFill(anim_to_points);
            bool wasUnlocked = anim_from_points >= def.requiredPoints;
            bool nowUnlocked = anim_to_points >= def.requiredPoints;
            if (unlockedBadge != null) unlockedBadge.SetActive(nowUnlocked);
            if (!wasUnlocked && nowUnlocked) PlayPulse();
            var cb = anim_on_complete;
            anim_on_complete = null;
            cb?.Invoke();
        }
    }

    private void SetAmount(int current)
    {
        if (amountLabel == null || def == null) return;

        amountLabel.SetText("{0} / {1}", current, def.requiredPoints);
    }

    private void SetBarFill(int current)
    {
        if (barFill == null || def == null || def.requiredPoints <= 0) return;
        float t = Mathf.Clamp01((float)current / def.requiredPoints);
        Vector3 s = barFill.transform.localScale;
        s.x = t;
        barFill.transform.localScale = s;
    }

    public void PlayCompletionAndExit(string logId, Action onDone)
    {
        DebugLogger.Log($"[MetaProgressPanel] SHOW_UNLOCK rewardId={logId}");

        if (flashOverlay != null)
        {
            Sequence.Create()
                .Chain(Tween.Alpha(flashOverlay, 0f, flashAlphaPeak, flashFadeInDuration))
                .Chain(Tween.Alpha(flashOverlay, flashAlphaPeak, 0f, flashFadeOutDuration));
        }

        Sequence.Create()
            .ChainDelay(iconPunchDelay)
            .ChainCallback(() =>
            {
                if (weaponIcon != null)
                    Tween.PunchScale(weaponIcon.rectTransform, new Vector3(iconPunchScale, iconPunchScale, 0f), iconPunchDuration);
                if (border != null)
                    Tween.Alpha(border, 0f, 1f, borderFadeInDuration);
            })
            .ChainDelay(badgeDelay)
            .ChainCallback(() =>
            {
                if (unlockedBadge != null)
                {
                    unlockedBadge.SetActive(true);
                    if (unlockedBadgeLabel is Graphic g)
                        Tween.Alpha(g, 0f, 1f, badgeFadeInDuration);
                    var badgeRT = unlockedBadge.GetComponent<RectTransform>();
                    if (badgeRT != null)
                        Tween.PunchScale(badgeRT, new Vector3(badgePunchScale, badgePunchScale, 0f), badgePunchDuration);
                }
            })
            .ChainDelay(preShakeDelay)
            .ChainCallback(() =>
            {
                if (rowRoot != null) StartCoroutine(ShakeAnchored(rowRoot, shakeMagnitude, shakeDuration, shakeSpeed));
            })
            .ChainDelay(preFlyOutDelay)
            .ChainCallback(() =>
            {
                DebugLogger.Log($"[MetaProgressPanel] FLY_OUT rewardId={logId}");
                if (rowRoot != null)
                    Tween.Scale(rowRoot, Vector3.zero, flyOutDuration, Ease.InQuad);
            })
            .ChainDelay(postFlyOutDelay)
            .ChainCallback(() =>
            {
                if (rowRoot != null)
                {
                    rowRoot.localScale = Vector3.one;
                    rowRoot.anchoredPosition = Vector2.zero;
                }
                if (border != null)       { Color32 c = border.color; c.a = 0; border.color = c; }
                if (flashOverlay != null) { Color32 c = flashOverlay.color; c.a = 0; flashOverlay.color = c; }
                if (unlockedBadge != null) unlockedBadge.SetActive(false);
                onDone?.Invoke();
            });
    }

    private static IEnumerator ShakeAnchored(RectTransform rt, float magnitude, float dur, float speed)
    {
        if (rt == null || dur <= 0f) yield break;
        Vector2 origin = rt.anchoredPosition;
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            float k = 1f - Mathf.Clamp01(t / dur);
            float dx = Mathf.Sin(t * speed) * magnitude * k;
            rt.anchoredPosition = origin + new Vector2(dx, 0f);
            yield return null;
        }
        rt.anchoredPosition = origin;
    }

    private void PlayPulse()
    {
        if (rowRoot == null) return;
        if (pulse_tween.isAlive) pulse_tween.Stop();
        rowRoot.localScale = Vector3.one * pulseScale;
        pulse_tween = Tween.Scale(rowRoot, Vector3.one, pulseDuration, Ease.OutBack);
    }
}
