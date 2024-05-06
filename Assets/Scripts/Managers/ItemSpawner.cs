using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> _dropItems;
    [SerializeField] public List<GameObject> _shopItems;

    private void Awake()
    {
    }

    //spawn a random item from the list of drop items. 
    public void SpawnRandomItem(Vector2 position)
    {
        var index = Random.Range(0, _dropItems.Count);
        var item = _dropItems[index];
        switch (item.tag)
        {
            case "Potion":
                item.GetComponent<potionScript>().ShopItem = false;
                break;
        }
        Instantiate(item, position,Quaternion.identity, null);
    }


    //spawn shop items as the appropriate positions.
    public void SpawnShopItems(Vector2 podium1, Vector2 podium2, Vector2 podium3)
    {
        var index = Random.Range(0, _dropItems.Count);
        Instantiate(_shopItems[index], podium1, Quaternion.identity, null);
        index = Random.Range(0, _dropItems.Count);
        Instantiate(_shopItems[index], podium2, Quaternion.identity, null);
        index = Random.Range(0, _dropItems.Count);
        Instantiate(_shopItems[index], podium3, Quaternion.identity, null);
    }

    //spawns treasure in the middle of the treasure room
    public void SpawnTreasure(Vector2 position)
    {
        var index = Random.Range(0, _dropItems.Count);
        var obj = _dropItems[index];
        if (obj.tag == "Potion")
            obj.GetComponent<potionScript>().ShopItem = false;
        Instantiate(obj, position, Quaternion.identity, null);
    }
}
