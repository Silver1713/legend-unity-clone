using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/Entity/Player")]
public class PlayerStats : EntityData
{
    [Header("Ranged")] public float Projectile_Cooldown = .5f;
}
