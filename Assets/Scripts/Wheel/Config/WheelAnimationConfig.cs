using UnityEngine;



//scriptable objects: inspector gözünden gözüken tüm sliderların şablonu
//her alan için tipine göre UI çiziyoruz
//float + [Range(0.1f, 1f)] → slider
//float + [Min(0f)] → sayı kutusu
//Color → renk seçici
//[Header(...)] → kalın başlık
//[Tooltip(...)]→fareyle üzerine gelince balon yazı
//inspector buradaki alanları görüp çizecek
//[Range(0.1f, 1f)→"Inspector'da bunu slider olarak çiz, 0.1 ile 1 arası"

// asıl ayarın kaydedildiği yer .assets kısmı. oraya taşıdım.

[CreateAssetMenu(fileName = "WheelAnimationConfig", menuName = "Wheel/WheelAnimationConfig")]
public class WheelAnimationConfig : ScriptableObject
{
    [Header("Spin Reveal (per-spin burst to row)")]
    [Tooltip("Dim hold before launching the particle burst from the wheel slot.")]
    [Min(0f)] public float spinFlyStartDelay = 0.6f;
    [Tooltip("Hold after the last particle lands so RewardListItemUI's count-up tween can finish before SPIN re-enables.")]
    [Min(0f)] public float spinPostLandHold = 0.45f;

    [Header("Panel show (DeathGameOverPanel etc.)")]
    [Tooltip("Scale-in duration for DeathGameOverPanel and similar generic panels.")]
    [Min(0f)] public float panelShowDuration = 0.3f;

    [Header("Fly icon land scale")]
    [Tooltip("Currency-card icon-to-HUD-pill fly duration.")]
    [Min(0f)] public float currencyFlyDuration = 0.55f;
    [Tooltip("Land scale for fly icons (slot→row and card→HUD). 1.0 = no shrink, 0.75 = settle smaller into target.")]
    [Range(0.1f, 1f)] public float flyLandScale = 0.75f;

    [Header("Count-up labels (reward row, HUD pill, ZoneHUD)")]
    [Tooltip("Shared duration for every count-up label tween (e.g. \"30 → 130 climbing\"). Keeping all three displays on one number guarantees they animate in lock-step.")]
    [Min(0f)] public float countUpDuration = 0.45f;

    [Header("Reward Row Receive — Particle Burst (Coin-Master style)")]

    [Tooltip("Maximum particles per burst. Capped at this even if reward.amount is larger; a 50-coin reward bursts 8 particles, each adding ~6 to the count. Lower for smaller wheel; higher for fireworks feel. Pool is sized for 3 simultaneous bursts.")]
    [Range(1, 16)] public int burstMaxParticles = 8;
    [Tooltip("Min/max initial outward speed of a particle (px/s).")]
    [Min(0f)] public float burstStartSpeedMin = 260f;
    [Min(0f)] public float burstStartSpeedMax = 460f;
    [Tooltip("Spread of initial direction in degrees (360 = full ring, 180 = top hemisphere). Direction biased outward from the wheel slot toward the panel.")]
    [Range(0f, 360f)] public float burstSpreadDegrees = 360f;
    [Tooltip("Per-frame velocity damping. velocity *= pow(burstDrag, dt*60). 0.92 ≈ medium drag — particles travel ~250px before stopping.")]
    [Range(0.5f, 1f)] public float burstDrag = 0.92f;
    [Tooltip("Time between consecutive particle launches in a burst. 0 = all at once; 0.04 = staggered stream.")]
    [Min(0f)] public float burstStaggerStep = 0.035f;
    [Tooltip("Particle lifetime cap (s). Particles that haven't reached the target by then are forced to arrive.")]
    [Min(0.1f)] public float burstParticleLifetime = 0.75f;
    [Tooltip("Attractor ramp curve power. 1 = linear (constant pull); 2 = quadratic (free flight first, slingshot in); 3 = cubic (stronger late pull).")]
    [Range(0.5f, 4f)] public float burstAttractorRamp = 2.0f;
    [Tooltip("Particle visual size at spawn (px square).")]
    [Min(4f)] public float burstParticleSize = 36f;
    [Tooltip("Particle scale on arrival, multiplied with size. <1 = shrinks into the slot.")]
    [Range(0.1f, 1.5f)] public float burstEndScale = 0.55f;
    [Tooltip("Distance to target (px) below which a particle is considered arrived.")]
    [Min(0f)] public float burstArrivalThreshold = 8f;

    [Header("Row Receive — Per-Arrival Punch")]
    [Tooltip("Peak scale of the row icon punch on EACH particle arrival. Smaller than a one-shot punch because it fires N times — cumulative read.")]
    [Range(1f, 1.20f)] public float rowArrivalPunchScale = 1.07f;
    [Tooltip("Per-arrival punch duration. Short so back-to-back arrivals overlap into a continuous wobble.")]
    [Min(0f)] public float rowArrivalPunchDuration = 0.10f;

    [Header("Count-up label — Text Tick Pulse")]
    [Tooltip("Peak scale of the per-integer text pulse on the row's count label.")]
    [Range(1f, 1.15f)] public float textTickPulseScale = 1.04f;
    [Tooltip("Text pulse duration (one in/out cycle on the sin curve).")]
    [Min(0f)] public float textTickPulseDuration = 0.09f;
    [Tooltip("Minimum interval between text pulses. Throttles rapid integer changes so the text reads as rhythmic ticks, not jitter.")]
    [Min(0f)] public float textTickPulseMinInterval = 0.10f;

    [Header("Receive offset (count-up vs. burst)")]
    [Tooltip("Delay between burst start and count-up start. Lets the first particle leave the wheel before the number begins climbing.")]
    [Min(0f)] public float countUpStartOffset = 0.06f;

    [Header("Slice Dim (non-winner fade during reveal)")]
    [Tooltip("Duration (seconds) for the dim IN tween — the focus punch on landing.")]
    [Min(0f)] public float dimInDuration = 0.25f;
    [Tooltip("Duration (seconds) for the dim OUT tween — the soft release as the wheel breathes back.")]
    [Min(0f)] public float dimOutDuration = 0.55f;
    [Tooltip("Maximum overlay alpha at full dim.")]
    [Range(0f, 1f)] public float dimMaxAlpha = 0.55f;
    [Tooltip("Overlay diameter as a fraction of SlotSize (1.0 matches bronze hole).")]
    [Min(0f)] public float dimSizeScale = 1.0f;
    [Tooltip("Multiplicative tint applied to icon and amount color at full dim.")]
    public Color dimTint = new Color(0.45f, 0.45f, 0.45f, 1f);

    [Header("Slice Glow (winner halo)")]
    [Tooltip("Tween speed (1/sec) for the winner glow alpha lerp.")]
    [Min(0f)] public float glowSpeed = 8f;
    [Tooltip("Maximum glow alpha at full intensity.")]
    [Range(0f, 1f)] public float glowMaxAlpha = 0.85f;
    [Tooltip("Glow diameter as a fraction of SlotSize. >1 bleeds past the slot for halo feel.")]
    [Min(0f)] public float glowSizeScale = 1.15f;
    [Tooltip("How long the glow holds at full intensity before auto-fading.")]
    [Min(0f)] public float glowHoldSeconds = 0.45f;
    [Tooltip("Glow color (RGB used; alpha animated separately).")]
    public Color glowTint = new Color32(255, 200, 80, 255);

    [Header("Wheel Slot — Content Layout (icon & amount placement)")]
    [Tooltip("Icon merkez X — slot boyutunun oranı (0=ortada, -0.5=sol, 0.5=sağ).")]
    [Range(-1f, 1f)] public float iconCenterXRatio = 0f;
    [Tooltip("Icon merkez Y — slot boyutunun oranı (0=ortada, -0.5=aşağı, 0.5=yukarı).")]
    [Range(-1f, 1f)] public float iconCenterYRatio = 0f;
    [Tooltip("Icon alanı boyutu — slot boyutunun oranı (1.0 = slot kadar büyük).")]
    [Min(0f)] public float iconAreaSizeRatio = 1.0f;

    [Tooltip("Amount yazı merkez X — slot boyutunun oranı.")]
    [Range(-1f, 1f)] public float amountCenterXRatio = 0f;
    [Tooltip("Amount yazı merkez Y — slot boyutunun oranı (-0.28 = altta).")]
    [Range(-1f, 1f)] public float amountCenterYRatio = -0.28f;
    [Tooltip("Amount yazı alanı genişliği — slot boyutunun oranı.")]
    [Min(0f)] public float amountAreaWidthRatio = 1.0f;
    [Tooltip("Amount yazı alanı yüksekliği — slot boyutunun oranı.")]
    [Min(0f)] public float amountAreaHeightRatio = 0.22f;

    [Header("Wheel Slot — Global Icon Scale")]
    [Tooltip("Tüm dilim icon'larının global boyut çarpanı (kategori-bağımsız).")]
    [Range(0.1f, 2f)] public float globalIconScale = 0.94f;

    [Header("Wheel Slot — Amount Text Width by Length")]
    [Tooltip("Kısa yazı için genişlik (örn. x2, +5).")]
    [Range(0f, 2f)] public float amountShortWidthRatio = 0.55f;
    [Tooltip("Orta uzunluk yazı için genişlik.")]
    [Range(0f, 2f)] public float amountMediumWidthRatio = 0.78f;
    [Tooltip("Kompakt yazı için genişlik (örn. 5K, 30K).")]
    [Range(0f, 2f)] public float amountCompactWidthRatio = 0.95f;
    [Tooltip("Uzun yazı için genişlik (örn. 1.5M).")]
    [Range(0f, 2f)] public float amountLongWidthRatio = 1.00f;

    //inspectordaki spin,reval gibigibi başlukların kaynağı burası
    [Header("Slice — Amount Text Pop (on slice draw)")]
    [Tooltip("Başlangıç ölçeği — yazı bu boyuttan 1.0'a OutBack ile büyür.")]
    [Range(0.1f, 1f)] public float amountPopStartScale = 0.7f;
    [Tooltip("Pop tween süresi (saniye).")]
    [Min(0f)] public float amountPopDuration = 0.32f;

    [Header("Hint Pulse (idle SPIN button nudge)")]
    [Tooltip("Peak scale of the hint pulse relative to original scale.")]
    [Min(1f)] public float hintPulseScale = 1.12f;
    [Tooltip("Duration of one in/out leg of the hint pulse.")]
    [Min(0f)] public float hintPulseDuration = 0.4f;
    [Tooltip("Number of hint pulse loops before stopping.")]
    [Min(0)] public int hintMaxLoops = 3;
}
