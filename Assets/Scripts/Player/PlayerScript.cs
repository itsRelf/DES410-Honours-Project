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
        _currencyUI.text = Currency.ToString();

        _atkScript.SetDamageValue(10);
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    void Update()
    {
        RotateElbow();
    }

    public void HandleDamage(int damageValue)
    {
        _health -= damageValue;
        _healthBar.SetHealth(_health);
    }

    private void HandleMovement()
    {
        var velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _rigidBody.MovePosition(_rigidBody.position + velocity * _moveSpeed * Time.fixedDeltaTime);
    }

    private void RotateElbow()
    {
        Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        _elbow.transform.rotation = rotation;
    }

    public int HealthCheckIn()
    {
        return _health;
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
