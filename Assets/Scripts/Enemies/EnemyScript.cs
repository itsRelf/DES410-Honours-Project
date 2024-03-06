using UnityEngine;

public class EnemyScript : MonoBehaviour, IDamageable
{
    [SerializeField] private BaseCharacterStats _stats;

    [SerializeField] private int _health;
    [SerializeField] private int _moveSpeed;
    [SerializeField] private Vector2 _movementVector;
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] public bool IsDead;
    // Start is called before the first frame update
    void Start()
    {
        _health = _stats.Health;
        _moveSpeed = _stats.moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        TrackHealth();
    }

    private void TrackHealth()
    {
        IsDead = _health <= 0;
    }

    public void HandleDamage(int damageValue)
    {
        _health -= damageValue;
    }
}
