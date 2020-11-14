/*
    UMKC CS 449: Nine Men's Morris implementation
    Copyright (C) 2020 Forgetful Wanderers

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    https://github.com/ZDGharst/UMKC_ForgetfulWanderers/
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Linq;

public enum Cell { Invalid, Vacant, White, Black };
public enum Player { White, Black };

public class BoardManager : MonoBehaviour
{
    public static Player currentPlayer = Player.White;
    public static bool millFormed = false;
    public static bool movingPiece = false;
    public static int[] pieceSelected = { -1, -1 };

    public static int whiteUnplacedPieces = 9;
    public static int whiteRemainingPieces = 9;
    public static bool isWhitePhase3 = false;

    public static int blackUnplacedPieces = 9;
    public static int blackRemainingPieces = 9;
    public static bool isBlackPhase3 = false;

    public static int tempRow;
    public static int tempCol;


    public Sprite man;

    public static Cell[,] BoardState = {
        {  Cell.Vacant, Cell.Invalid, Cell.Invalid,  Cell.Vacant, Cell.Invalid, Cell.Invalid,  Cell.Vacant },
        { Cell.Invalid,  Cell.Vacant, Cell.Invalid,  Cell.Vacant, Cell.Invalid,  Cell.Vacant, Cell.Invalid },
        { Cell.Invalid, Cell.Invalid,  Cell.Vacant,  Cell.Vacant,  Cell.Vacant, Cell.Invalid, Cell.Invalid },
        {  Cell.Vacant,  Cell.Vacant,  Cell.Vacant, Cell.Invalid,  Cell.Vacant,  Cell.Vacant,  Cell.Vacant },
        { Cell.Invalid, Cell.Invalid,  Cell.Vacant,  Cell.Vacant,  Cell.Vacant, Cell.Invalid, Cell.Invalid },
        { Cell.Invalid,  Cell.Vacant, Cell.Invalid,  Cell.Vacant, Cell.Invalid,  Cell.Vacant, Cell.Invalid },
        {  Cell.Vacant, Cell.Invalid, Cell.Invalid,  Cell.Vacant, Cell.Invalid, Cell.Invalid,  Cell.Vacant },
    };

    private void InitGame()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardState[i, j] == Cell.Vacant)
                    CreateIntersections(i, j);
            }
        }
    }

    /* Create intersections at game start. */
    private void CreateIntersections(int x, int y)
    {
        /* Create a blank object with the name and position corresponding to the location of the intersection. */
        GameObject g = new GameObject((char)(x + 97) + "" + (y + 1));
        g.transform.position = new Vector2(x - 3, y - 3);
        g.transform.SetParent(this.transform);

        /* Speciality constructor. */
        Intersection.CreateComponent(g, x, y);

        /* Make intersection clickable. */
        g.AddComponent<BoxCollider>();

        /* Add sprite details to intersection. */
        var s = g.AddComponent<SpriteRenderer>();
        s.sprite = man;
        s.color = new Color(0, 0, 0, 0);
    }

    /* Reset the game back to a fresh start. */
    private void ResetBoard()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardState[i, j] != Cell.Invalid)
                    BoardState[i, j] = Cell.Vacant;
            }
        }
        foreach (Intersection i in Resources.FindObjectsOfTypeAll(typeof(Intersection)) as Intersection[])
        {
            i.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        }

        currentPlayer = Player.White;
        millFormed = false;
        movingPiece = false;
        pieceSelected[0] = -1;
        pieceSelected[1] = -1;

        whiteUnplacedPieces = 9;
        whiteRemainingPieces = 9;
        isWhitePhase3 = false;

        blackUnplacedPieces = 9;
        blackRemainingPieces = 9;
        isBlackPhase3 = false;
    }

    public static Player GetOppositePlayer()
    {
        if (currentPlayer == Player.White)
        {
            return Player.Black;
        }
        return Player.White;
    }

    public static bool CheckMill(Player p, int row, int column)
    {
        Cell cellCast = p == Player.White ? Cell.White : Cell.Black;
        bool intersectionPartOfMill = false;

        /* If we're in row 4, we have six spaces to check instead of three; special case. */
        if (row == 3)
        {
            if(column < 3)
            {
                if(BoardState[3, 0] == cellCast && BoardState[3, 1] == cellCast && BoardState[3, 2] == cellCast)
                {
                    intersectionPartOfMill = true;
                    print("Mill formed within the special row on the left");
                }
            }
            else
            {
                if (BoardState[3, 4] == cellCast && BoardState[3, 5] == cellCast && BoardState[3, 6] == cellCast)
                {
                    intersectionPartOfMill = true;
                    print("Mill formed within the special row on the right");
                }
            }
        }

        /* The general case for rows. */
        else
        {
            /* Check row for a mill. */
            int piecesInLine = 0;
            for (int i = 0; i < 7; i++)
            {
                if (BoardState[row, i] == cellCast)
                {
                    piecesInLine++;
                }
            }
            if (piecesInLine == 3)
            {
                intersectionPartOfMill = true;
                print("Mill formed within row");
            }
        }

        /* If we're in column d, we have six spaces to check instead of three; special case. */
        if (column == 3)
        {
            if (row < 3)
            {
                if (BoardState[0, 3] == cellCast && BoardState[1, 3] == cellCast && BoardState[2, 3] == cellCast)
                {
                    intersectionPartOfMill = true;
                    print("Mill formed within the special column on the bottom");
                }
            }
            else
            {
                if (BoardState[4, 3] == cellCast && BoardState[5, 3] == cellCast && BoardState[6, 3] == cellCast)
                {
                    intersectionPartOfMill = true;
                    print("Mill formed within the special column on the top");
                }
            }
        }

        /* The general case for columns. */
        else
        {
            /* Check row for a mill. */
            int piecesInLine = 0;
            for (int i = 0; i < 7; i++)
            {
                if (BoardState[i, column] == cellCast)
                {
                    piecesInLine++;
                }
            }
            if (piecesInLine == 3)
            {
                intersectionPartOfMill = true;
                print("Mill formed within column");
            }
        }

        return intersectionPartOfMill;
    }

    /* A man that is part of a mill can only be removed if all pieces of that player are part of a mill. This method checks to see if all pieces that a player owns are part of a mill. */
    public static bool AllMenInMill()
    {
        Player playerToCheck = GetOppositePlayer();
        Cell oppositePlayerCell = BoardManager.currentPlayer == Player.White ? Cell.Black : Cell.White;

        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardState[i, j] == oppositePlayerCell && !CheckMill(playerToCheck, i, j))
                {
                    return false;
                }
            }
        }
        return true;
    }

    /* Phase 1: Player is adding a piece to the board. */
    public static void Phase1Action(GameObject g, int row, int column)
    {
        var s = g.GetComponent<SpriteRenderer>();

        if (currentPlayer == Player.White)
        {
            BoardState[row, column] = Cell.White;
            s.color = new Color(255, 255, 255, 1);
            whiteUnplacedPieces--;
        }

        else
        {
            BoardState[row, column] = Cell.Black;
            s.color = new Color(0, 0, 0, 1);
            blackUnplacedPieces--;
        }

        millFormed = CheckMill(currentPlayer, row, column);
        if (millFormed != true)
        {
            currentPlayer = GetOppositePlayer();
        }
    }

    public static void Phase2Selection(GameObject g, int row, int column)
    {
        var s = g.GetComponent<SpriteRenderer>();
        tempRow = row;      // used for checking selected piece's starting position
        tempCol = column;

        if(BoardState[row, column] == Cell.Black || BoardState[row, column] == Cell.White)
        {
            BoardState[row, column] = Cell.Vacant;
            s.color = new Color(0, 0, 0, 0);
            movingPiece = true;
        }

        else
        {
            print("Invaild spot, please try again.");
        }

    }

    public static void Phase2Movement(GameObject g, int row, int column)
    {
        var s = g.GetComponent<SpriteRenderer>();

        if (currentPlayer == Player.White)  // white's turn after picking up a piece
        {

            if (BoardState[row, column] == Cell.Vacant 
                && !checkSamePosition(row, tempRow, column, tempCol)
                && isAdjacent(tempRow, tempCol, row, column))
            {
                BoardState[row, column] = Cell.White;
                s.color = new Color(255, 255, 255, 1);
                movingPiece = false;

                millFormed = CheckMill(currentPlayer, row, column);
                if (millFormed != true)
                {
                    currentPlayer = GetOppositePlayer();
                }
            }

            else if(checkSamePosition(row, tempRow, column, tempCol))
            {
                BoardState[row, column] = Cell.White;
                s.color = new Color(255, 255, 255, 1);
                movingPiece = false;
            }

            else
            {
                print("Invaild spot, please try again.");
            }
        }

        else                                // black's turn after picking up a piece
        {

            if (BoardState[row, column] == Cell.Vacant && !checkSamePosition(row, tempRow, column, tempCol))
            {
                BoardState[row, column] = Cell.Black;
                s.color = new Color(0, 0, 0, 1);
                movingPiece = false;

                millFormed = CheckMill(currentPlayer, row, column);
                if (millFormed != true)
                {
                    currentPlayer = GetOppositePlayer();
                }
            }

            else
            {
                BoardState[row, column] = Cell.Black;
                s.color = new Color(0, 0, 0, 1);
                movingPiece = false;

                print("Invaild spot, please try again.");
            }
        }
    }

    /* Remove piece after validating that the piece can be removed by milling player. */
    public static void Mill(GameObject g, int row, int column)
    {
        /* Remove the piece from the board, set the cell to vacant, and reduce the remaining pieces of that player. */
        BoardState[row, column] = Cell.Vacant;
        g.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        if(currentPlayer == Player.White)
        {
            blackRemainingPieces--;
        }
        else
        {
            whiteRemainingPieces--;
        }

        /* Consume the mill and swap control. */
        millFormed = false;
        currentPlayer = GetOppositePlayer();
    }

    private void GameOver()
    {
        return;
    }

    public static bool checkSamePosition(int r1, int r2, int c1, int c2)
    {
        if (r1 == r2 && c1 == c2)
        {
            return true;
        }

        return false;
    }

    private void Start()        // function called when we the scene is first loaded
    {
        InitGame();
    }

    private void Update()       // called every frame
    {
        if (Input.GetKeyDown("r"))
        {
            ResetBoard();
        }

        /* Could move this to the Mill() method. */
        if (whiteRemainingPieces < 3 || blackRemainingPieces < 3)
            GameOver();
    }
            // PLEASE ignore this thing, I couldn't think of a way to get it working with multi-dimensional arrays
    public static bool isAdjacent(int row, int column, int tarRow, int tarCol)
    {
        if (tarRow == 0 && tarCol == 6)                 // a7
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 3, 6 };
            int[] neighbor2 = new int[] { 0, 3 };
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2))
            {
                return true;
            }
        }

        if (tarRow == 3 && tarCol == 6)                 // c7
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 0, 6 };
            int[] neighbor2 = new int[] { 6, 6 };
            int[] neighbor3 = new int[] { 3, 6 };
            if (src == neighbor1 || src == neighbor2 || src == neighbor3)
            {
                return true;
            }
        }

        if (tarRow == 6 && tarCol == 6)                 // g7
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 3, 6 };
            int[] neighbor2 = new int[] { 6, 3 };
            if (src == neighbor1 || src == neighbor2)
            {
                return true;
            }
        }

        if (tarRow == 1 && tarCol == 5)                 // b6
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 3, 5 };
            int[] neighbor2 = new int[] { 1, 3 };
            if (src == neighbor1 || src == neighbor2)
            {
                return true;
            }
        }

        if (tarRow == 3 && tarCol == 5)                 // d6
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 3, 6 };
            int[] neighbor2 = new int[] { 1, 5 };
            int[] neighbor3 = new int[] { 5, 5 };
            int[] neighbor4 = new int[] { 3, 4 };
            if (src == neighbor1 || src == neighbor2 || src == neighbor3 || src == neighbor4)
            {
                return true;
            }
        }

        if (tarRow == 5 && tarCol == 5)                 // f6
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 3, 5 };
            int[] neighbor2 = new int[] { 5, 3 };
            if (src == neighbor1 || src == neighbor2)
            {
                return true;
            }
        }

        if (tarRow == 2 && tarCol == 4)                 // c5
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 3, 4 };
            int[] neighbor2 = new int[] { 2, 3 };
            if (src == neighbor1 || src == neighbor2)
            {
                return true;
            }
        }

        if (tarRow == 3 && tarCol == 4)                 // d5
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 3, 5 };
            int[] neighbor2 = new int[] { 2, 4 };
            int[] neighbor3 = new int[] { 4, 4 };
            if (src == neighbor1 || src == neighbor2 || src == neighbor3)
            {
                return true;
            }
        }

        if (tarRow == 4 && tarCol == 4)                 // e5
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 3, 4 };
            int[] neighbor2 = new int[] { 4, 3 };
            if (src == neighbor1 || src == neighbor2)
            {
                return true;
            }
        }

        if (tarRow == 0 && tarCol == 3)                 // a4
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 0, 6 };
            int[] neighbor2 = new int[] { 1, 3 };
            int[] neighbor3 = new int[] { 0, 0 };
            if (src == neighbor1 || src == neighbor2 || src == neighbor3)
            {
                return true;
            }
        }

        if (tarRow == 1 && tarCol == 3)                 // b4
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 1, 5 };
            int[] neighbor2 = new int[] { 0, 3 };
            int[] neighbor3 = new int[] { 2, 3 };
            int[] neighbor4 = new int[] { 1, 1 };
            if (src == neighbor1 || src == neighbor2 || src == neighbor3 || src == neighbor4)
            {
                return true;
            }
        }

        if (tarRow == 2 && tarCol == 3)                 // c4
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 2, 4 };
            int[] neighbor2 = new int[] { 1, 3 };
            int[] neighbor3 = new int[] { 2, 2 };
            if (src == neighbor1 || src == neighbor2 || src == neighbor3)
            {
                return true;
            }
        }

        if (tarRow == 4 && tarCol == 3)                 // e4
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 4, 4 };
            int[] neighbor2 = new int[] { 5, 3 };
            int[] neighbor3 = new int[] { 4, 2 };
            if (src == neighbor1 || src == neighbor2 || src == neighbor3)
            {
                return true;
            }
        }

        if (tarRow == 5 && tarCol == 3)                 // f4
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 5, 5 };
            int[] neighbor2 = new int[] { 4, 3 };
            int[] neighbor3 = new int[] { 6, 3 };
            int[] neighbor4 = new int[] { 5, 1 };
            if (src == neighbor1 || src == neighbor2 || src == neighbor3 || src == neighbor4)
            {
                return true;
            }
        }

        if (tarRow == 6 && tarCol == 3)                 // g4
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 6, 0 };
            int[] neighbor2 = new int[] { 5, 3 };
            int[] neighbor3 = new int[] { 6, 6 };
            if (src == neighbor1 || src == neighbor2 || src == neighbor3)
            {
                return true;
            }
        }

        if (tarRow == 2 && tarCol == 2)                 // c3
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 2, 3 };
            int[] neighbor2 = new int[] { 3, 2 };
            if (src == neighbor1 || src == neighbor2)
            {
                return true;
            }
        }

        if (tarRow == 3 && tarCol == 2)                 // d3
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 2, 2 };
            int[] neighbor2 = new int[] { 4, 2 };
            int[] neighbor3 = new int[] { 3, 1 };
            if (src == neighbor1 || src == neighbor2 || src == neighbor3)
            {
                return true;
            }
        }

        if (tarRow == 4 && tarCol == 2)                 // e3
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 4, 3 };
            int[] neighbor2 = new int[] { 3, 2 };
            if (src == neighbor1 || src == neighbor2)
            {
                return true;
            }
        }

        if (tarRow == 1 && tarCol == 1)                 // b2
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 1, 3 };
            int[] neighbor2 = new int[] { 3, 1 };
            if (src == neighbor1 || src == neighbor2)
            {
                return true;
            }
        }

        if (tarRow == 3 && tarCol == 1)                 // d2
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 3, 2 };
            int[] neighbor2 = new int[] { 1, 1 };
            int[] neighbor3 = new int[] { 5, 1 };
            int[] neighbor4 = new int[] { 3, 0 };
            if (src == neighbor1 || src == neighbor2 || src == neighbor3 || src == neighbor4)
            {
                return true;
            }
        }

        if (tarRow == 5 && tarCol == 1)                 // f2
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 5, 3 };
            int[] neighbor2 = new int[] { 3, 1 };
            if (src == neighbor1 || src == neighbor2)
            {
                return true;
            }
        }

        if (tarRow == 0 && tarCol == 0)                 // a1
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 0, 3 };
            int[] neighbor2 = new int[] { 3, 0 };
            if (src == neighbor1 || src == neighbor2)
            {
                return true;
            }
        }

        if (tarRow == 3 && tarCol == 0)                 // d1
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 3, 1 };
            int[] neighbor2 = new int[] { 0, 0 };
            int[] neighbor3 = new int[] { 6, 0 };
            if (src == neighbor1 || src == neighbor2 || src == neighbor3)
            {
                return true;
            }
        }

        if (tarRow == 6 && tarCol == 0)                 // g1
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 6, 3 };
            int[] neighbor2 = new int[] { 3, 0 };
            if (src == neighbor1 || src == neighbor2)
            {
                return true;
            }
        }

        return true;
    }
}
