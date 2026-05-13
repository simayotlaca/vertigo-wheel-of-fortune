using UnityEngine;

public static class WheelSlotContentAreas
{
    public struct Areas
    {
        public Vector2 IconCenter;
        public Vector2 IconSize;
        public Vector2 AmountCenter;
        public Vector2 AmountSize;
    }

    public static Areas Compute(float slotSize, WheelAnimationConfig cfg)
    {
        Areas a;
        a.IconCenter   = new Vector2(slotSize * cfg.iconCenterXRatio,   slotSize * cfg.iconCenterYRatio);
        a.IconSize     = new Vector2(slotSize * cfg.iconAreaSizeRatio,  slotSize * cfg.iconAreaSizeRatio);
        a.AmountCenter = new Vector2(slotSize * cfg.amountCenterXRatio, slotSize * cfg.amountCenterYRatio);
        a.AmountSize   = new Vector2(slotSize * cfg.amountAreaWidthRatio, slotSize * cfg.amountAreaHeightRatio);
        return a;
    }
}
