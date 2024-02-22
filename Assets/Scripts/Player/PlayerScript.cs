using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    private Rigidbody2D _rigidbody;
    private Vector2 _velocity;
    private int _health;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    void Update()
    {
        
    }

    private void HandleMovement()
    {
        _velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _rigidbody.MovePosition(_rigidbody.position + _velocity * _moveSpeed * Time.fixedDeltaTime);
    }
}
