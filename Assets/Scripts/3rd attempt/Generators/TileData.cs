using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileSO_", menuName = "TileData/Create Tile Object")]
public class TileData : ScriptableObject
{
    public enum Type
    {
        Wall,
        Floor,
        SpawnPoint,
    }

    public Type TileType;
    [Tooltip("Place the desired TileBase objects into this list. \n For walls place in the following order - 'TL' 'TC' 'TR' 'BL' 'BC' 'BR'")]public List<TileBase> Tile; 
    public Color AssignedColor;
}
