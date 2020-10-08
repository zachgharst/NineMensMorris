using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Man : MonoBehaviour
{
    int i = 0;
    int j = 0;

    void OnMouseDown()
    {
        var s = gameObject.GetComponent<SpriteRenderer>();

        if (BoardManager.turn == 0)
        {
            BoardManager.BoardState[i, j] = Cell.White;
            s.color = new Color(255, 255, 255);
        }
        else
        {
            BoardManager.BoardState[i, j] = Cell.Black;
            s.color = new Color(0, 0, 0);
        }

        BoardManager.turn = (BoardManager.turn + 1) % 2;
    }
}