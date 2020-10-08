using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Cell { Invalid, Vacant, White, Black };

public class BoardManager : MonoBehaviour
{
    public Sprite sprite;
    public static int turn = 0;

    public static Cell[,] BoardState = {
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
                    DrawTile(i, j, BoardState[i, j]);
            }
        }
    }

    private void DrawTile(int x, int y, Cell status)
    {
        GameObject g = new GameObject((char)(x + 97) + "" + (y + 1));
        g.transform.position = new Vector2(x - 3, y - 3);
        g.transform.SetParent(this.transform);
        g.AddComponent(typeof(Man));
        g.AddComponent<BoxCollider>();
        var s = g.AddComponent<SpriteRenderer>();
        s.sprite = sprite;

        if (status == Cell.White)
        {
            s.color = new Color(255, 255, 255, 1);
        }

        else if (status == Cell.Black)
        {
            s.color = new Color(0, 0, 0, 1);
        }

        else
        {
            s.color = new Color(0, 0, 0, 0);
        }
    }
}