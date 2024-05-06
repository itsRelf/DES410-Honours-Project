using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour, IDamageable
{
    [Header("Components")]
    [SerializeField] private BaseCharacterStats _characterStats;
    [SerializeField] private Rigidbody2D _rigidBody;
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
    [SerializeField] private int _maxHealth;
    [field: SerializeField] public int Currency { get; set; }
    [SerializeField] private float _delay = 0.15f;
    [SerializeField] private int _knockBackStrength = 8;
    [SerializeField] private bool _hit;
    [SerializeField] public bool Dead;

    [Header("Controls")]
    [SerializeField] private PlayerControls _playerControls;
    [SerializeField] private bool _dodging;
    [SerializeField] private bool _newDodgePressNeeded;
    [SerializeField] private bool _isImmune;

    
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _healthBar = GameObject.Find("PlayerHealth").GetComponent<HealthBar>();
        _currencyUI = GameObject.Find("CurrencyText").GetComponent<TextMeshProUGUI>();
        _playerControls = new PlayerControls();

        _playerControls.Default.Attack.performed += ctx =>
        {
            _weaponAnimator.SetTrigger("Attacking");
        };
        _playerControls.Default.Dash.started += ctx => _dodging = ctx.ReadValueAsButton();
        _playerControls.Default.Dash.performed += ctx => _dodging = ctx.ReadValueAsButton();
        _playerControls.Default.Dash.canceled += ctx =>
        {
            //_dodging = ctx.ReadValueAsButton();
            if (_newDodgePressNeeded)
                _newDodgePressNeeded = false;
        };

    }

    private void Start()
    {
        _maxHealth = _characterStats.Health;
        _health = _maxHealth;
        _moveSpeed = _characterStats.moveSpeed;

        _healthBar.SetMaxHealth(_maxHealth);

        Currency = 0;
        UpdateCurrency();
        _atkScript.SetDamageValue(10);
    }

    private void FixedUpdate()
    {
        if (!_hit && !_dodging)
        {
            //Debug.Log($"Dodging: {_dodging}\n Hit: {_hit}");
            HandleMovement();
        }
        if(_dodging && !_newDodgePressNeeded)
            HandleDash();
        UpdateCurrency();
        Dead = _health <= 0;
    }

    void Update()
    {
        RotateElbow(); //rotates the weapon around the player
    }

    private void UpdateCurrency()
    {
        _currencyUI.text = Currency.ToString();
    }

    public void HandleDamage(int damageValue, Vector2 position) //apply damage to the player along with a knockback
    {
        if (_hit) return;
        StopAllCoroutines();
        _hit = true;
        _health -= damageValue;
        _healthBar.SetHealth(_health);
        Vector2 dir = (transform.position - (Vector3)position).normalized;
        _rigidBody.AddForce(dir * _knockBackStrength, ForceMode2D.Impulse);
        _isImmune = true;
        StartCoroutine(Reset());
    }

    private void HandleMovement()
    {
        //Debug.Log($"Dodging: {_dodging}\n Hit: {_hit}\n Moving");
        var velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _rigidBody.MovePosition(_rigidBody.position + velocity * _moveSpeed * Time.fixedDeltaTime);
    }

    private void HandleDash() //apply forward momentum to the player in the direction of the mouse.
    {
       //Debug.Log("Dodging");
        Vector2 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        //Debug.Log(dir);
        _rigidBody.AddForce(dir * _moveSpeed, ForceMode2D.Impulse);
        StartCoroutine(Reset());
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
        if (_health >= _maxHealth)
            _health = _maxHealth;
        _healthBar.SetHealth(_health);
    }

    public int HealthCheckIn()
    {
        return _health;
    }

    private IEnumerator Reset() //reset any applied force to the player rigid body
    {
        yield return new WaitForSeconds(_delay);
        _rigidBody.velocity = Vector2.zero;
        if(_hit)
            _hit = false;
        if (_isImmune)
            _isImmune = false;
        if (_dodging)
        {
            _newDodgePressNeeded = true;
        }

        _dodging = false;
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
