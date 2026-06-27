using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TileType;

public class DualGridTilemap : MonoBehaviour {


    protected static Vector3Int[] NEIGHBOURS = new Vector3Int[] {
        new Vector3Int(0, 0, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 1, 0)
    };

    protected Dictionary<Tuple<TileType, TileType, TileType, TileType>, Tile> neighbourTupleToTile;

    // Provide references to each tilemap in the inspector
    public Tilemap placeholderTilemap;
    public Tilemap displayTilemap;

    // Provide the dirt and grass placeholder tiles in the inspector
    public Tile grassPlaceholderTile;
    public Tile dirtPlaceholderTile;
    public Tile stonePlaceholderTile;
    public Tile waterPlaceholderTile;

    // Provide the 16 tiles in the inspector
    public Tile[] tiles;

    void Start() {
        neighbourTupleToTile = new();
        AddRules(Grass, Dirt, 0);      // tiles[0–15]
        AddRules(Stone, Grass, 16);    // tiles[16–31]
        AddRules(Stone, Dirt, 32);     // tiles[32–47]
        AddRules(Stone, Water, 48);    // tiles[48–63]
        AddRules(Water, Grass, 64);    // tiles[64–79]
        AddRules(Water, Dirt, 80);     // tiles[80–95]
        FillAreaWithGrass(20);
        RefreshDisplayTilemap();
    }

    private void AddRules(TileType A, TileType B, int offset){
        // This dictionary stores the "rules", each 4-neighbour configuration corresponds to a tile
        // |_1_|_2_|
        // |_3_|_4_|
        neighbourTupleToTile.Add(new(A, A, A, A), tiles[offset + 6]);
        neighbourTupleToTile.Add(new(B, B, B, A), tiles[offset + 13]);
        neighbourTupleToTile.Add(new(B, B, A, B), tiles[offset + 0]);
        neighbourTupleToTile.Add(new(B, A, B, B), tiles[offset + 8]);
        neighbourTupleToTile.Add(new(A, B, B, B), tiles[offset + 15]);
        neighbourTupleToTile.Add(new(B, A, B, A), tiles[offset + 1]);
        neighbourTupleToTile.Add(new(A, B, A, B), tiles[offset + 11]);
        neighbourTupleToTile.Add(new(B, B, A, A), tiles[offset + 3]);
        neighbourTupleToTile.Add(new(A, A, B, B), tiles[offset + 9]);
        neighbourTupleToTile.Add(new(B, A, A, A), tiles[offset + 5]);
        neighbourTupleToTile.Add(new(A, B, A, A), tiles[offset + 2]);
        neighbourTupleToTile.Add(new(A, A, B, A), tiles[offset + 10]);
        neighbourTupleToTile.Add(new(A, A, A, B), tiles[offset + 7]);
        neighbourTupleToTile.Add(new(B, A, A, B), tiles[offset + 14]);
        neighbourTupleToTile.Add(new(A, B, B, A), tiles[offset + 4]);
        neighbourTupleToTile.Add(new(B, B, B, B), tiles[offset + 12]);
        FillAreaWithGrass(20);
        RefreshDisplayTilemap();  
    }

    private (TileType A, TileType B, int offset) GetPair(TileType topLeft, TileType topRight, TileType botLeft, TileType botRight)
    {
        // Collect all tile types in the block
        TileType[] arr = {topLeft, topRight, botLeft, botRight};

        // Find the first non-None tile
        TileType A = TileType.None;
        for (int i = 0; i < 4; i++)
        {
            if (arr[i] != None)
            {
                A = arr[i];
                break;
            }
        }

        if (A == None)
            return (None, None, -1);

        // Find a second different tile
        TileType B = A;
        for (int i = 0; i < 4; i++)
        {
            if (arr[i] != None && arr[i] != A)
            {
                B = arr[i];
                break;
            }
        }

        // If all tiles are the same
        if (A == B)
            return (A, A, 0);

        // Determine offset
        if ((A == Grass & B == Dirt) | (A == Dirt & B == Grass)) return (Grass, Dirt, 0);
        if ((A == Stone & B == Grass) | (A == Grass & B == Stone)) return (Stone, Grass, 16);
        if ((A == Stone & B == Dirt) | (A == Dirt & B == Stone)) return (Stone, Dirt, 32);
        if ((A == Stone & B == Water) | (A == Water & B == Stone)) return (Stone, Water, 48);
        if ((A == Water & B == Grass) | (A == Grass & B == Water)) return (Water, Grass, 64);
        if ((A == Water & B == Dirt) | (A == Dirt & B == Water)) return (Water, Dirt, 80);
        return (None, None, -1);
    }

    public void SetCell(Vector3Int coords, Tile tile) {
        placeholderTilemap.SetTile(coords, tile);
        setDisplayTile(coords);
    }

    private TileType getPlaceholderTileTypeAt(Vector3Int coords) {
        TileBase t = placeholderTilemap.GetTile(coords);
        if (t == grassPlaceholderTile) return Grass;
        if (t == dirtPlaceholderTile) return Dirt;
        if (t == stonePlaceholderTile) return Stone;
        if (t == waterPlaceholderTile) return Water;
        return None;
    }

    protected Tile calculateDisplayTile(Vector3Int coords) {
        // 4 neighbours
        TileType topRight = getPlaceholderTileTypeAt(coords - NEIGHBOURS[0]);
        TileType topLeft = getPlaceholderTileTypeAt(coords - NEIGHBOURS[1]);
        TileType botRight = getPlaceholderTileTypeAt(coords - NEIGHBOURS[2]);
        TileType botLeft = getPlaceholderTileTypeAt(coords - NEIGHBOURS[3]);

        // Determine which tile pair we are in
        var (A, B, offset) = GetPair(topLeft, topRight, botLeft, botRight);

        if (offset < 0)
            return null;

        Tuple<TileType, TileType, TileType, TileType> neighbourTuple = new(topLeft, topRight, botLeft, botRight);

        if (neighbourTupleToTile.TryGetValue(neighbourTuple, out Tile tile))
            return tile;

        return null;
    }

    protected void setDisplayTile(Vector3Int pos) {
        for (int i = 0; i < NEIGHBOURS.Length; i++) {
            Vector3Int newPos = pos + NEIGHBOURS[i];
            displayTilemap.SetTile(newPos, calculateDisplayTile(newPos));
        }
    }

    private void FillAreaWithGrass(int radius)
    {
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                // Only fill empty tiles
                if (placeholderTilemap.GetTile(pos) == null)
                {
                    placeholderTilemap.SetTile(pos, grassPlaceholderTile);
                }
            }
        }
    }

    // The tiles on the display tilemap will recalculate themselves based on the placeholder tilemap
    public void RefreshDisplayTilemap() {
        for (int i = -50; i < 50; i++) {
            for (int j = -50; j < 50; j++) {
                setDisplayTile(new Vector3Int(i, j, 0));
            }
        }
    }
}

public enum TileType {
    None,
    Grass,
    Dirt,
    Stone,
    Water
}
