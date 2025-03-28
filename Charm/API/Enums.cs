﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using Arithmic;


// TODO: Find where these indexes actually go?
// Would be nice if bungie stopped changing these every season :)
public static class DestinyDamageType
{
    public static DestinyDamageTypeEnum GetDamageType(int index)
    {
        switch (index)
        {
            case -1:
                return DestinyDamageTypeEnum.None;
            case 1319:
            case 1373:
            case 1405:
            case 1473:
                return DestinyDamageTypeEnum.Kinetic;
            case 1320:
            case 1374:
            case 1406:
            case 1474:
                return DestinyDamageTypeEnum.Arc;
            case 1321:
            case 1375:
            case 1407:
            case 1475:
                return DestinyDamageTypeEnum.Solar;
            case 1322:
            case 1376:
            case 1408:
            case 1476:
                return DestinyDamageTypeEnum.Void;
            case 1323:
            case 1377:
            case 1409:
            case 1477:
                return DestinyDamageTypeEnum.Stasis;
            case 1324:
            case 1378:
            case 1410:
            case 1478:
                return DestinyDamageTypeEnum.Strand;
            default:
                Log.Warning($"Unknown DestinyDamageTypeEnum {index}");
                return DestinyDamageTypeEnum.None;
        }
    }
}

public enum DestinyDamageTypeEnum : int
{
    None = -1,
    [Description("Kinetic")]
    Kinetic,
    [Description(" Arc")]
    Arc,
    [Description(" Solar")]
    Solar,
    [Description(" Void")]
    Void,
    [Description(" Stasis")]
    Stasis,
    [Description(" Strand")]
    Strand
}

public enum DestinyTierType
{
    Unknown = -1,
    Currency = 0,
    Common = 1, // Basic
    Uncommon = 2, // Common
    Rare = 3,
    Legendary = 4, // Superior
    Exotic = 5,
}

// https://bungie-net.github.io/multi/schema_Destiny-DestinyUnlockValueUIStyle.html#schema_Destiny-DestinyUnlockValueUIStyle
// Pls update your api docs bungo, most dont match up
public enum DestinyUnlockValueUIStyle
{
    Automatic = 0,
    Checkbox = 1,
    DateTime = 2,
    Fraction = 3,
    Integer = 5,
    Percentage = 6,
    TimeDuration = 7,
    GreenPips = 9,
    RedPips = 10,
    Hidden = 11,
    RawFloat = 13,
}

public static class DestinyTierTypeColor
{
    private static readonly Dictionary<DestinyTierType, Color> Colors = new Dictionary<DestinyTierType, Color>
    {
        { DestinyTierType.Unknown, Color.FromArgb(255, 56, 56, 56) },
        { DestinyTierType.Currency, Color.FromArgb(255, 56, 56, 56) },
        { DestinyTierType.Common, Color.FromArgb(255, 194, 187, 179) },
        { DestinyTierType.Uncommon, Color.FromArgb(255, 51, 107, 62) },
        { DestinyTierType.Rare, Color.FromArgb(255, 85, 125, 155) },
        { DestinyTierType.Legendary, Color.FromArgb(255, 79, 55, 99) },
        { DestinyTierType.Exotic, Color.FromArgb(255, 203, 171, 54) }
    };

    public static Color GetColor(this DestinyTierType tierType)
    {
        if (Colors.ContainsKey(tierType))
            return Colors[tierType];
        else
            return Colors[DestinyTierType.Unknown];
    }
}
