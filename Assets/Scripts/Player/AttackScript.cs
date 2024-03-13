using UnityEngine;

public class AttackScript : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private BoxCollider2D _damageTrigger;

    private void Awake()
    {
        _damageTrigger = GetComponent<BoxCollider2D>();
    }
    private void Start()
    {
        _damageTrigger.enabled = false;
    }

    ///Called in the player class to set the damage of the weapon. Simple implementation, will build complexity with more weapon varieties later.
    public void SetDamageValue(int damage)
    {
        _damage = damage;
    }

    public void StartAttack()
    {
        _damageTrigger.enabled = true;
    }

    public void EndAttack()
    {
        _damageTrigger.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.name);
        if (other.tag != "Enemy") return; //guard against hitting player
        var damageable = other.GetComponent<IDamageable>();
        damageable.HandleDamage(_damage, transform.parent.parent.position);
    }

    private void OnDrawGizmos()
    {
        if (_damageTrigger.enabled)
        {
            var cubePosition = new Vector3(transform.position.x + _damageTrigger.offset.x,
                transform.position.y + _damageTrigger.offset.y);
            Gizmos.DrawWireCube(cubePosition, _damageTrigger.size);
        }
    }
}
