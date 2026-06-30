using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Tilemaps;

public partial class CursorController : MonoBehaviour {
    public DualGridTilemap dualGridTilemap;
    private int paintMode = 1;
    void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) paintMode = 1;
        if (Input.GetKeyDown(KeyCode.Alpha2)) paintMode = 2;
        if (Input.GetKeyDown(KeyCode.Alpha3)) paintMode = 3;
        if (Input.GetKeyDown(KeyCode.Alpha4)) paintMode = 4;
        if (Input.GetKeyDown(KeyCode.Alpha5)) paintMode = 5;
        if (Input.GetKeyDown(KeyCode.Alpha6)) paintMode = 6;

        var (primary, secondary) = GetPaintTiles();
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3Int tilePos = GetWorldPosTile(mouseWorldPos);
        transform.position = tilePos + new Vector3(0.5f, 0.5f, -1);

        if (Input.GetMouseButton(0)) {
            dualGridTilemap.SetCell(tilePos, primary);
        } else if (Input.GetMouseButton(1)) {
            dualGridTilemap.SetCell(tilePos, secondary);
        }
    }

    private (Tile primary, Tile secondary) GetPaintTiles(){
        return paintMode switch{
            1 => (dualGridTilemap.grassPlaceholderTile, dualGridTilemap.dirtPlaceholderTile),
            2 => (dualGridTilemap.grassPlaceholderTile, dualGridTilemap.waterPlaceholderTile),
            3 => (dualGridTilemap.stonePlaceholderTile, dualGridTilemap.grassPlaceholderTile),
            4 => (dualGridTilemap.stonePlaceholderTile, dualGridTilemap.dirtPlaceholderTile),
            5 => (dualGridTilemap.stonePlaceholderTile, dualGridTilemap.waterPlaceholderTile),
            6 => (dualGridTilemap.waterPlaceholderTile, dualGridTilemap.dirtPlaceholderTile),
            _ => (dualGridTilemap.grassPlaceholderTile, dualGridTilemap.dirtPlaceholderTile)
        };
    }

    public static Vector3Int GetWorldPosTile(Vector3 worldPos) {
        int xInt = Mathf.FloorToInt(worldPos.x);
        int yInt = Mathf.FloorToInt(worldPos.y);
        return new(xInt, yInt, 0);
    }
}
