using System.Collections;
using TMPro;
using UnityEngine;

public class EnemyScript : MonoBehaviour, IDamageable
{
    [SerializeField] private BaseCharacterStats _stats;

    [SerializeField] private int _health;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private Vector2 _movementVector;
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] public bool IsDead;
    [SerializeField] private bool _hit;

    [SerializeField] private float _delay = 0.15f;
    [SerializeField] private int _kickBackStrength = 16;
    [SerializeField] private EnemyAttackScript _attack;
    [SerializeField] private HealthBar _healthbar;

    [SerializeField] private GameObject _player;

    [SerializeField] private float _cooldown;
    // Start is called before the first frame update
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _player = GameObject.Find("Player");
    }
    void Start()
    {
        _health = _stats.Health;
        _moveSpeed = _stats.moveSpeed;
        _healthbar.SetMaxHealth(_health);
    }

    // Update is called once per frame
    void Update()
    {
        TrackHealth();
    }

    private void FixedUpdate()
    {
        HandleCoolDown();
        if (!_hit)
            MoveToPlayer();
    }
    private void TrackHealth()
    {
        IsDead = _health <= 0;
    }

    private void HandleCoolDown()
    {
        _cooldown = _cooldown > 0 ? _cooldown -= Time.deltaTime : 0;
    }

    private void MoveToPlayer()
    {
        if (_player == null)
            return;

        float distance = Vector2.Distance(_player.transform.position, transform.position);
        if (distance < 3f)
        {
            if (distance <= 1f && _cooldown <= 0)
            {
                _attack.Attack();
                _cooldown = 1f;
            }
            else
            {
                Vector2 direction = (_player.transform.position - transform.position).normalized;
                _rigidBody.MovePosition(_rigidBody.position + direction * _moveSpeed * Time.fixedDeltaTime);
            }
        }
    }

    public void HandleDamage(int damageValue, Vector2 position)
    {
        if (_hit) return;
        StopAllCoroutines();
        _hit = true;
        _health -= damageValue;
        Vector2 dir = (transform.position - (Vector3)position).normalized;
        Debug.Log(dir);
        _rigidBody.AddForce(dir * _kickBackStrength, ForceMode2D.Impulse);
        _healthbar.SetHealth(_health);
        StartCoroutine(Reset());
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(_delay);
        _rigidBody.velocity = Vector2.zero;
        _hit = false;
    }

    private void OnEnable()
    {
        _player = GameObject.Find("Player");
    }
}
