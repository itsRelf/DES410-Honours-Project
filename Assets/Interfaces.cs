using UnityEngine;

public interface IDamageable
{
    public void HandleDamage(int damageValue);
}

public interface IPickup
{
    public void HandlePickup(GameObject other);
}