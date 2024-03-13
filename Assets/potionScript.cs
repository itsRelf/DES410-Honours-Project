using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class potionScript : MonoBehaviour, IPickup
{
    [SerializeField] private int value;
    public void HandlePickup(GameObject other)
    {
        other.GetComponent<PlayerScript>().Heal(value);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Player") return;
        HandlePickup(other.gameObject);
    }
}
