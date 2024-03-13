using UnityEngine;

public interface IDamageable
{
    public void HandleDamage(int damageValue, Vector2 position);
}

public interface IPickup
{
    public void HandlePickup(GameObject other);
}