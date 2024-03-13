using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KnockbackFeedback : MonoBehaviour
{
    [SerializeField] public Rigidbody2D _rigidBody;

    [SerializeField] private float _strength = 16, _delay = 0.15f;

    public UnityEvent OnBegin, OnDone;

    public void PlayFeedback(GameObject sender)
    {
        StopAllCoroutines();
        OnBegin?.Invoke();
        Vector2 dir = (transform.position - sender.transform.position).normalized;
        _rigidBody.AddForce(dir * _strength, ForceMode2D.Impulse);
        StartCoroutine(Reset());
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(_delay);
        _rigidBody.velocity = Vector2.zero;
        OnDone?.Invoke();
    }
}


