using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public Transform gridParent;

    public int width = 6;
    public int height = 10;

    private Tile[,] tiles;
    private string[,] values;
    private TileType[,] types;

    private void Start()
    {
        tiles = new Tile[width, height];
        values = new string[width, height];
        types = new TileType[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                types[x, y] = TileType.Empty;

        List<OperatorPos> equations = new List<OperatorPos>();
        int equationCount = 5; // 원하는 수식 개수
        int attempts = 0;

        while (equations.Count < equationCount && attempts < 500)
        {
            attempts++;

            OperatorDirection dir = (Random.value > 0.5f) ? OperatorDirection.Horizontal : OperatorDirection.Vertical;
            OperatorPos op = GenerateRandomEquation(dir);

            if (!IsOverlapping(op))
            {
                bool crossesAny = false;
                foreach (var eq in equations)
                {
                    if (IsCrossing(op, eq))
                    {
                        crossesAny = true;
                        break;
                    }
                }

                if (equations.Count == 0 || crossesAny)
                {
                    PlaceEquation(op);
                    equations.Add(op);
                }
            }
        }

        CreateTiles();
    }

    bool IsCrossing(OperatorPos newEq, OperatorPos existingEq)
    {
        for (int i = 0; i < newEq.elements.Length; i++)
        {
            int x = newEq.startPos.x + (newEq.direction == OperatorDirection.Horizontal ? i : 0);
            int y = newEq.startPos.y + (newEq.direction == OperatorDirection.Vertical ? i : 0);

            string val = newEq.elements[i];
            if (x < 0 || x >= width || y < 0 || y >= height) continue;

            if (values[x, y] == val)
                return true;
        }
        return false;
    }

    OperatorPos GenerateRandomEquation(OperatorDirection dir)
    {
        int a = Random.Range(1, 10);
        int b = Random.Range(1, 10);
        string[] ops = { "+", "-", "x", "%" };
        string op = ops[Random.Range(0, ops.Length)];

        int result = 0;
        switch (op)
        {
            case "+":
                result = a + b;
                break;
            case "-":
                result = a - b;
                break;
            case "x":
                result = a * b;
                break;
            case "%":
                while (b == 0 || a % b != 0)
                {
                    a = Random.Range(1, 10);
                    b = Random.Range(1, 10);
                }
                result = a / b;
                break;
        }

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

    bool IsOverlapping(OperatorPos op)
    {
        for (int i = 0; i < op.elements.Length; i++)
        {
            int x = op.startPos.x + (op.direction == OperatorDirection.Horizontal ? i : 0);
            int y = op.startPos.y + (op.direction == OperatorDirection.Vertical ? i : 0);

            if (x >= width || y >= height)
                return true;

            TileType currentType = DetermineTileType(op.elements[i]);

            if (types[x, y] == TileType.Empty)
                continue;

            if (types[x, y] != currentType)
                return true;

            if (values[x, y] != op.elements[i])
                return true;
        }
        return false;
    }

    void PlaceEquation(OperatorPos op)
    {
        for (int i = 0; i < op.elements.Length; i++)
        {
            int x = op.startPos.x + (op.direction == OperatorDirection.Horizontal ? i : 0);
            int y = op.startPos.y + (op.direction == OperatorDirection.Vertical ? i : 0);

            values[x, y] = op.elements[i];
            types[x, y] = DetermineTileType(op.elements[i]);
        }
    }

    void CreateTiles()
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
                tile.SetTile(type, val, new Vector2Int(x, y));

                tiles[x, y] = tile;
            }
        }
    }

    TileType DetermineTileType(string s)
    {
        if (s == "+" || s == "-" || s == "x" || s == "%") return TileType.Operator;
        if (s == "=") return TileType.Equal;
        return TileType.Number;
    }
}
