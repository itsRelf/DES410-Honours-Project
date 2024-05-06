using UnityEngine;

public class EnemyAttackScript : MonoBehaviour
{
    private GameObject _target;
    private Animator _animator;
    private BoxCollider2D _damageTrigger;
    private int _damage = 10;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _target = GameObject.Find("Player");
        _damageTrigger = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var dir = ((Vector2) _target.transform.position - (Vector2) transform.parent.position).normalized;
        transform.parent.transform.right = dir;
        var scale = transform.parent.localScale;
        scale.y = dir.x < 0 ? -1 : 1;
        transform.parent.localScale = scale;
    }

    public void Attack()
    {
        _animator.SetTrigger("Attacking");
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
        if (other.tag != "Player") return; //guard against hitting player
        var damageable = other.GetComponent<IDamageable>();
        damageable.HandleDamage(_damage, transform.parent.parent.position);
    }
}
