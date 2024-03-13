using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour, IDamageable
{
    [Header("Components")]
    [SerializeField] private BaseCharacterStats _characterStats;
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private PlayerControls _playerControls;
    [SerializeField] private Animator _weaponAnimator;

    [Header("Children Components")]
    [SerializeField] private GameObject _elbow;
    [SerializeField] private AttackScript _atkScript;

    [Header("UI Components")]
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private TextMeshProUGUI _currencyUI;

    [Header("Data")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private int _health;
    [field: SerializeField] public int Currency { get; set; }
    [SerializeField] private float _delay = 0.15f;
    [SerializeField] private int _knockBackStrength = 8;
    [SerializeField] private bool _hit;
    
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _healthBar = FindObjectOfType<HealthBar>();
        _currencyUI = GameObject.Find("CurrencyText").GetComponent<TextMeshProUGUI>();
        _playerControls = new PlayerControls();

        _playerControls.Default.Attack.performed += Attack_performed; ;
    }

    private void Attack_performed(InputAction.CallbackContext obj)
    {
        _weaponAnimator.SetTrigger("Attacking");
    }

    private void Start()
    {
        _health = _characterStats.Health;
        _moveSpeed = _characterStats.moveSpeed;

        _healthBar.SetMaxHealth(_health);
        _healthBar.SetHealth(_health);

        Currency = 0;
        UpdateCurrency();
        _atkScript.SetDamageValue(10);
    }

    private void FixedUpdate()
    {
        if(!_hit)
            HandleMovement();
        UpdateCurrency();
    }

    void Update()
    {
        RotateElbow();
    }

    private void UpdateCurrency()
    {
        _currencyUI.text = Currency.ToString();
    }

    public void HandleDamage(int damageValue, Vector2 position)
    {
        if (_hit) return;
        StopAllCoroutines();
        _hit = true;
        _health -= damageValue;
        _healthBar.SetHealth(_health);
        Vector2 dir = (transform.position - (Vector3)position).normalized;
        _rigidBody.AddForce(dir * _knockBackStrength, ForceMode2D.Impulse);
        StartCoroutine(Reset());
    }

    private void HandleMovement()
    {
        var velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _rigidBody.MovePosition(_rigidBody.position + velocity * _moveSpeed * Time.fixedDeltaTime);
    }

    private void RotateElbow()
    {
        Vector2 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        _elbow.transform.right = dir;
        var scale = _elbow.transform.localScale;
        scale.y = dir.x < 0 ? -1 : 1;
        _elbow.transform.localScale = scale;
    }

    public void Heal(int value)
    {
        _health += value;
        _healthBar.SetHealth(_health);
    }

    public int HealthCheckIn()
    {
        return _health;
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(_delay);
        _rigidBody.velocity = Vector2.zero;
        _hit = false;
    }

    private void OnEnable()
    {
        _playerControls.Default.Enable();
    }

    private void OnDisable()
    {
        _playerControls.Default.Disable();
    }
}
