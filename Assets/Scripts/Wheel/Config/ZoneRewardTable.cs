using UnityEngine;

[System.Serializable]
public struct ZoneRewardEntry
{
    public RewardDefinition reward;

    [Min(1)]
    public int amount;

    [Min(0f)]
    public float weight;
}

[CreateAssetMenu(fileName = "ZoneRewardTable", menuName = "Wheel/ZoneRewardTable")]
public class ZoneRewardTable : ScriptableObject
{

    public ZoneType zoneType = ZoneType.Normal;

    public ZoneConfig targetZone;

    public ZonePoolRules poolRules;

    public ZoneRewardEntry[] entries;
}
