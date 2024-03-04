using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    private Rigidbody2D _rigidbody;
    [SerializeField] private int _health;
    [SerializeField] private GameObject _elbow;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _health = 100;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    void Update()
    {
        RotateElbow();
    }

    private void HandleMovement()
    {
        var velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _rigidbody.MovePosition(_rigidbody.position + velocity * _moveSpeed * Time.fixedDeltaTime);
    }

    private void RotateElbow()
    {
        Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 10, Vector3.forward);
        transform.rotation = rotation;
    }
}
