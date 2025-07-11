using UnityEngine;

public class Weapon : ScriptableObject
{
    [Header("Base Weapon Stats (Immutable)")]
    public string weaponName;
    public float baseMinDamage = 5f;
    public float baseMaxDamage = 10f;
    public float baseAttackSpeed = 1f;     // attacks per second
    public float baseCritChance = 0.1f;   // 10%
    public float baseCritDamage = 2f;     // 2× damage
    public float baseKnockback = 5f;
    public float baseRange = 1f;

    [Header("Runtime Weapon Stats (Read-Only)")]
    public float minDamage { get; private set; }
    public float maxDamage { get; private set; }
    public float attackSpeed { get; private set; }
    public float critChance { get; private set; }
    public float critDamage { get; private set; }
    public float knockbackForce { get; private set; }
    public float range { get; private set; }

    /// <summary>
    /// Apply a full set of modifiers from your C++ DDA system.
    /// </summary>
    public void ApplyModifiers(WeaponModifiers mods)
    {
        minDamage = baseMinDamage * mods.minDamage.multiplier + mods.minDamage.additive;
        maxDamage = baseMaxDamage * mods.maxDamage.multiplier + mods.maxDamage.additive;
        attackSpeed = baseAttackSpeed * mods.attackSpeed.multiplier + mods.attackSpeed.additive;
        critChance = baseCritChance * mods.criticalChance.multiplier + mods.criticalChance.additive;
        critDamage = baseCritDamage * mods.criticalDamage.multiplier + mods.criticalDamage.additive;
        knockbackForce = baseKnockback * mods.knockbackForce.multiplier + mods.knockbackForce.additive;
        range = baseRange * mods.range.multiplier + mods.range.additive;
    }

    /// <summary>
    /// Rolls a damage value between minDamage and maxDamage,
    /// and applies critical strike if it procs.
    /// </summary>
    public float GetDamage()
    {
        // 1) Roll base damage in the configured range
        float minD = minDamage == 0.0f ? baseMinDamage : minDamage;
        float maxD = maxDamage == 0.0f ? baseMaxDamage : maxDamage;

        float dmg = Random.Range(minD, maxD);
        dmg = Mathf.Round(dmg);

        // 2) Roll for critical hit
        if (Random.value <= critChance)
        {
            dmg *= critDamage;
        }

        return dmg;
    }
}

/// <summary>
/// A simple two-term modifier: scale then shift.
/// </summary>
[System.Serializable]
public struct StatModifier
{
    public float multiplier; // 1 = no change; 1.2 = +20%; 0.8 = –20%
    public float additive;   // 0 = no change; e.g. +2 adds flat bonus
}

/// <summary>
/// Bundle all per-stat modifiers together for atomic updates.
/// </summary>
[System.Serializable]
public struct WeaponModifiers
{
    public StatModifier minDamage;
    public StatModifier maxDamage;
    public StatModifier attackSpeed;
    public StatModifier criticalChance;
    public StatModifier criticalDamage;
    public StatModifier knockbackForce;
    public StatModifier range;
}
