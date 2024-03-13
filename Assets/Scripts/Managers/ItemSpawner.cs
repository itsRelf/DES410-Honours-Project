using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> _dropItems;
    [SerializeField] public List<GameObject> _shopItems;

    private void Awake()
    {
    }

    public void SpawnRandomItem(Vector2 position)
    {
        var index = Random.Range(0, _dropItems.Count);
        Instantiate(_dropItems[index], position,Quaternion.identity, null);
    }

    public void SpawnShopItems(Vector2 podium1, Vector2 podium2, Vector2 podium3)
    {
        var index = Random.Range(0, _dropItems.Count);
        Instantiate(_shopItems[index], podium1, Quaternion.identity, null);
        index = Random.Range(0, _dropItems.Count);
        Instantiate(_shopItems[index], podium2, Quaternion.identity, null);
        index = Random.Range(0, _dropItems.Count);
        Instantiate(_shopItems[index], podium3, Quaternion.identity, null);
    }
}
