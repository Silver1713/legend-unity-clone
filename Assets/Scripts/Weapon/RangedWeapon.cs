using UnityEngine;

[CreateAssetMenu(fileName = "RangedWeapon", menuName = "Scriptable Objects/Weapon/Ranged")]
public class RangedWeapon : Weapon
{
    [Header("Ranged Weapon Projectile")]
    public GameObject projectilePrefab; // Prefab for the projectile
}
