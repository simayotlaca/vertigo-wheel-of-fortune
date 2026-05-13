using TMPro;
using UnityEngine;
using static WheelSlotContentAreas;

public static class WheelAmountTextStyle
{
    public enum Profile
    {
        Hidden,
        Short,
        Medium,
        Compact,
        Long
    }

    public struct Style
    {
        public Profile Profile;
        public Vector2 SizeDelta;
        public float FontSizeMin;
        public float FontSizeMax;
        public bool EnableAutoSizing;
        public TextAlignmentOptions Alignment;
    }

    public static Profile Classify(string formatted)
    {
        if (string.IsNullOrEmpty(formatted)) return Profile.Hidden;

        for (int i = 0; i < formatted.Length; i++)
        {
            char c = formatted[i];
            if (c == 'M' || c == 'm') return Profile.Long;
            if (c == 'K' || c == 'k') return Profile.Compact;
        }

        int bodyLen = formatted.Length;
        if (bodyLen > 0 && (formatted[0] == 'x' || formatted[0] == '+')) bodyLen--;

        if (bodyLen <= 2) return Profile.Short;
        return Profile.Medium;
    }

    public static Style Resolve(string formatted, float slotSize, Areas areas, WheelAnimationConfig cfg)
    {
        Profile p = Classify(formatted);
        Style s;
        s.Profile = p;
        s.Alignment = TextAlignmentOptions.Center;

        float boxH = areas.AmountSize.y;

        switch (p)
        {
            case Profile.Hidden:
                s.SizeDelta = new Vector2(areas.AmountSize.x * cfg.amountShortWidthRatio, boxH);
                s.FontSizeMin = slotSize * 0.10f;
                s.FontSizeMax = slotSize * 0.18f;
                s.EnableAutoSizing = false;
                break;

            case Profile.Short:

                s.SizeDelta = new Vector2(areas.AmountSize.x * cfg.amountShortWidthRatio, boxH);
                s.FontSizeMin = slotSize * 0.20f;
                s.FontSizeMax = slotSize * 0.20f;
                s.EnableAutoSizing = false;
                break;

            case Profile.Medium:
                s.SizeDelta = new Vector2(areas.AmountSize.x * cfg.amountMediumWidthRatio, boxH);
                s.FontSizeMin = slotSize * 0.16f;
                s.FontSizeMax = slotSize * 0.18f;
                s.EnableAutoSizing = true;
                break;

            case Profile.Compact:
                s.SizeDelta = new Vector2(areas.AmountSize.x * cfg.amountCompactWidthRatio, boxH);
                s.FontSizeMin = slotSize * 0.13f;
                s.FontSizeMax = slotSize * 0.16f;
                s.EnableAutoSizing = true;
                break;

            case Profile.Long:
            default:
                s.SizeDelta = new Vector2(areas.AmountSize.x * cfg.amountLongWidthRatio, boxH);
                s.FontSizeMin = slotSize * 0.12f;
                s.FontSizeMax = slotSize * 0.15f;
                s.EnableAutoSizing = true;
                break;
        }
        return s;
    }
}
