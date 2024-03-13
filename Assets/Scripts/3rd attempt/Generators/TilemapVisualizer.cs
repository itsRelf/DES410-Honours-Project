using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField] private Tilemap _floorTilemap;
    [SerializeField] private TileBase _tile;
    [SerializeField] private List<Sprite> _tiles;
    [SerializeField] private List<Color> _colors;
    [SerializeField] private List<TileData> _tileData;
    public void GetPixelDataFromTexture(Texture2D tex, Vector2Int roomPosition)
    {
        for (var x = 0; x < tex.width; x++)
        {
            for (var y = 0; y < tex.height; y++)
            {
                GenerateTile(x,y, tex, roomPosition);
            }
        }
    }

    public void GenerateTile(int x, int y, Texture2D tex, Vector2Int roomPosition)
    {
        Color pixelColor = tex.GetPixel(x, y);
        //var position = PositionFromTileGrid(x, y);
        var tilePosition = _floorTilemap.WorldToCell((Vector3Int)(roomPosition + new Vector2Int(x,y)));
        if (pixelColor.a == 0)
            return;
        foreach (var tile in _tileData)
        {
            Debug.Log(pixelColor == tile.AssignedColor);
            if (tile.AssignedColor == pixelColor)
            {
                //_floorTilemap.SetTile(tilePosition, tile.Tile);
            }
        }
    }

    public Vector2Int PositionFromTileGrid(int x, int y)
    {
        // Assuming each tile is 16x16 pixels
        int tileSize = 16;

        // Assuming the grid in Unity is set to 16x16 units per grid cell
        float unitsPerGridCell = 16f;

        // Calculate the offset based on half of the tile size
        Vector2 offset = new Vector2(tileSize / 2f, -tileSize / 2f) / unitsPerGridCell;

        // Calculate the position of the tile in world coordinates
        Vector2Int ret = new Vector2Int(tileSize * x, -tileSize * y) + Vector2Int.RoundToInt(offset);

        return ret;
    }


    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, _floorTilemap, _tile);
    }

    public void PaintFloorTiles(Vector2Int position, int tileVariable)
    {

    }

    private static void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    private static void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }
}
