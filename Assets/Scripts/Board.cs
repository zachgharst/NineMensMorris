using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Cell { Invalid, Vacant, White, Black };

public class Board : MonoBehaviour
{
    public Sprite sprite;

    Cell[,] BoardState = {
        {  Cell.Vacant, Cell.Invalid, Cell.Invalid,  Cell.Vacant, Cell.Invalid, Cell.Invalid,  Cell.Vacant },
        { Cell.Invalid,  Cell.Vacant, Cell.Invalid,  Cell.Vacant, Cell.Invalid,  Cell.Vacant, Cell.Invalid },
        { Cell.Invalid, Cell.Invalid,  Cell.Vacant,  Cell.Vacant,  Cell.Vacant, Cell.Invalid, Cell.Invalid },
        {  Cell.Vacant,  Cell.Vacant,  Cell.Vacant, Cell.Invalid,  Cell.Vacant,  Cell.Vacant,  Cell.Vacant },
        { Cell.Invalid, Cell.Invalid,  Cell.Vacant,  Cell.Vacant,  Cell.Vacant, Cell.Invalid, Cell.Invalid },
        { Cell.Invalid,  Cell.Vacant, Cell.Invalid,  Cell.Vacant, Cell.Invalid,  Cell.Vacant, Cell.Invalid },
        {  Cell.Vacant, Cell.Invalid, Cell.Invalid,  Cell.Vacant, Cell.Invalid, Cell.Invalid,  Cell.Vacant },
    };

    void Start()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardState[i, j] != Cell.Invalid)
                    SpawnTile(i, j, BoardState[i, j]);
            }
        }
    }

    private void SpawnTile(int x, int y, Cell status)
    {
        GameObject g = new GameObject((char)(x + 97) + "" + (y + 1));
        g.transform.position = new Vector2(x, y);
        var s = g.AddComponent<SpriteRenderer>();
        s.sprite = sprite;

        if (status == Cell.Vacant)
        {
            s.color = new Color(0, 0, 255);
        }

        else if (status == Cell.White)
        {
            s.color = new Color(255, 255, 255);
        }

        else if (status == Cell.Black)
        {
            s.color = new Color(0, 0, 0);
        }
    }
}