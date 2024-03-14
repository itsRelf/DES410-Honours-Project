using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class potionScript : MonoBehaviour, IPickup
{
    [SerializeField] private int value;
    [SerializeField] private int cost;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Player") return;
        if (ShopItem)
        {
            var currentCash = other.GetComponent<PlayerScript>().Currency;
            if (currentCash < cost) return;
            HandlePickup(other.gameObject);

        }
        else
            HandlePickup(other.gameObject);
    }
}
