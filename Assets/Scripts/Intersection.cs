using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection : MonoBehaviour
{
    public int row;
    public int column;

    public static Intersection CreateComponent(GameObject location, int r, int c)
    {
        Intersection i = location.AddComponent<Intersection>();
        i.row = r;
        i.column = c;
        return i;
    }

    void Phase1()
    {
        var s = gameObject.GetComponent<SpriteRenderer>();

        if (BoardManager.currentPlayer == Player.White)
        {
            BoardManager.BoardState[row, column] = Cell.White;
            s.color = new Color(255, 255, 255);
            BoardManager.currentPlayer = Player.Black;
            BoardManager.whiteUnplacedPieces--;
        }

        else
        {
            BoardManager.BoardState[row, column] = Cell.Black;
            s.color = new Color(0, 0, 0);
            BoardManager.currentPlayer = Player.White;
            BoardManager.blackUnplacedPieces--;
        }

//        CheckMill(row, column);
    }

    void Phase2()
    {
        return;
    }

    void Phase3()
    {
        return;
    }

    void Mill()
    {
        return;
    }

    void OnMouseDown()
    {
        Cell cellCast = BoardManager.currentPlayer == Player.White ? Cell.White : Cell.Black;

        if (BoardManager.millFormed == true && BoardManager.BoardState[row, column] != cellCast)
        {
            Mill();
        }
        else if (BoardManager.blackUnplacedPieces > 0)
        {
            if (BoardManager.BoardState[row, column] != Cell.Vacant)
                return;
            Phase1();
        }
        else if (BoardManager.blackRemainingPieces > 3 && BoardManager.whiteRemainingPieces > 3)
        {
            Phase2();
        }
        else
        {
            Phase3();
        }
    }
}