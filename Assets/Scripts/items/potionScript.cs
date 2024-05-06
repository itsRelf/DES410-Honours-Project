using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class potionScript : MonoBehaviour, IPickup
{
    [SerializeField] private int value;
    [SerializeField] private int cost;
    [field: SerializeField] public int dropChance { get; private set; }
    [SerializeField] public bool ShopItem;

    [SerializeField]
    private TextMeshProUGUI Text;

    private void Awake()
    {
        Text = GetComponentInChildren<TextMeshProUGUI>();
    }
    private void Start()
    {
        Text.gameObject.SetActive(ShopItem);
        Text.text = cost.ToString();
    }
    public void HandlePickup(GameObject other)
    {
        other.GetComponent<PlayerScript>().Heal(value);
        Destroy(this.gameObject);
    }

    //only active when the player steps on the collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Player") return;
        if (ShopItem)
        {
            var currentCash = other.GetComponent<PlayerScript>().Currency;
            if (currentCash < cost) return;
            currentCash -= cost;
            other.GetComponent<PlayerScript>().Currency = currentCash;
            HandlePickup(other.gameObject);

        }
        else
            HandlePickup(other.gameObject);
    }
}
