using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject tilePrefab;      // 타일 프리팹 (Tile 스크립트 포함)
    public Transform gridParent;      

    public int width = 6;
    public int height = 10;

    private Tile[,] tiles;

    private void Start()
    {
        tiles = new Tile[width, height];

        string[,] values = new string[width, height];
        TileType[,] types = new TileType[width, height];

        List<OperatorPos> equations = new List<OperatorPos>();
        int attempts = 0;

        while (equations.Count < 4 && attempts < 100)
        {
            attempts++;
            OperatorDirection dir = (equations.Count % 2 == 0) ? OperatorDirection.Horizontal : OperatorDirection.Vertical;
            OperatorPos op = GenerateRandomEquation(dir);

            if (!IsOverlapping(op, types))
            {
                PlaceEquation(op, values, types);
                equations.Add(op);
            }
        }

        CreateTiles(values, types);
    }

    OperatorPos GenerateRandomEquation(OperatorDirection dir)
    {
        int a = Random.Range(1, 10);
        int b = Random.Range(1, 10);
        string op = Random.value > 0.5f ? "+" : "*";
        int result = op == "+" ? a + b : a * b;

        Vector2Int startPos = new Vector2Int(
            Random.Range(0, width - (dir == OperatorDirection.Horizontal ? 5 : 0)),
            Random.Range(0, height - (dir == OperatorDirection.Vertical ? 5 : 0))
        );

        return new OperatorPos
        {
            startPos = startPos,
            direction = dir,
            elements = new string[] { a.ToString(), op, b.ToString(), "=", result.ToString() }
        };
    }

    bool IsOverlapping(OperatorPos op, TileType[,] types)
    {
        for (int i = 0; i < op.elements.Length; i++)
        {
            int x = op.startPos.x + (op.direction == OperatorDirection.Horizontal ? i : 0);
            int y = op.startPos.y + (op.direction == OperatorDirection.Vertical ? i : 0);

            if (x >= width || y >= height) return true;
            if (types[x, y] != TileType.Empty && types[x, y] != 0) return true;
        }
        return false;
    }

    void PlaceEquation(OperatorPos op, string[,] values, TileType[,] types)
    {
        for (int i = 0; i < op.elements.Length; i++)
        {
            int x = op.startPos.x + (op.direction == OperatorDirection.Horizontal ? i : 0);
            int y = op.startPos.y + (op.direction == OperatorDirection.Vertical ? i : 0);

            values[x, y] = op.elements[i];
            types[x, y] = DetermineTileType(op.elements[i]);
        }
    }

    void CreateTiles(string[,] values, TileType[,] types)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                string val = values[x, y];
                TileType type = types[x, y];

                if (string.IsNullOrEmpty(val)) continue;

                GameObject obj = Instantiate(tilePrefab, gridParent);
                obj.transform.localPosition = new Vector3(x * 100, -y * 100, 0);

                Tile tile = obj.GetComponent<Tile>();
                tile.SetTile(type, val);

                tiles[x, y] = tile;
            }
        }
    }

    TileType DetermineTileType(string s)
    {
        if (s == "+" || s == "-" || s == "*" || s == "/") return TileType.Operator;
        if (s == "=") return TileType.Equal;
        return TileType.Number;
    }
}
