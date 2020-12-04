/*
 *  UMKC CS 449: Nine Men's Morris implementation
 *  Copyright (C) 2020 Forgetful Wanderers
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  https://github.com/ZDGharst/UMKC_ForgetfulWanderers/
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public enum Cell { Invalid, Vacant, White, Black };
public enum Player { White, Black };

public class BoardManager : MonoBehaviour
{
    public static int turn = 0;

    public static Player currentPlayer = Player.White;

    public static Player computerPlayer = Player.Black;
    public static bool computerIsActive = true;
    public double computerTime = 1.5;

    public static bool millFormed = false;
    public static bool movingPiece = false;

    public static int whiteUnplacedPieces = 9;
    public static int whiteRemainingPieces = 9;
    public static bool isWhitePhase3 = false;

    public static int blackUnplacedPieces = 9;
    public static int blackRemainingPieces = 9;
    public static bool isBlackPhase3 = false;

    public static int tempRow;
    public static int tempCol;
    public static bool gameOver = false;

    public Sprite man;
    public Sprite manSelected;
    public static GameObject lastSelected;

    public static Cell[,] BoardState = {
        {  Cell.Vacant, Cell.Invalid, Cell.Invalid,  Cell.Vacant, Cell.Invalid, Cell.Invalid,  Cell.Vacant },
        { Cell.Invalid,  Cell.Vacant, Cell.Invalid,  Cell.Vacant, Cell.Invalid,  Cell.Vacant, Cell.Invalid },
        { Cell.Invalid, Cell.Invalid,  Cell.Vacant,  Cell.Vacant,  Cell.Vacant, Cell.Invalid, Cell.Invalid },
        {  Cell.Vacant,  Cell.Vacant,  Cell.Vacant, Cell.Invalid,  Cell.Vacant,  Cell.Vacant,  Cell.Vacant },
        { Cell.Invalid, Cell.Invalid,  Cell.Vacant,  Cell.Vacant,  Cell.Vacant, Cell.Invalid, Cell.Invalid },
        { Cell.Invalid,  Cell.Vacant, Cell.Invalid,  Cell.Vacant, Cell.Invalid,  Cell.Vacant, Cell.Invalid },
        {  Cell.Vacant, Cell.Invalid, Cell.Invalid,  Cell.Vacant, Cell.Invalid, Cell.Invalid,  Cell.Vacant },
    };

    public static void InitGame()
    {
        GameObject g = GameObject.Find("BoardManager");
        BoardManager b = g.GetComponent<BoardManager>();
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardState[i, j] == Cell.Vacant)
                {
                    b.CreateIntersections(i, j);
                }
            }
        }

        ResetBoard();
    }

    /* Create intersections at game start. */
    public void CreateIntersections(int x, int y)
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

    public static GameObject FindIntersection(string str)
    {
        return GameObject.Find(str);
    }

    /* Reset the game back to a fresh start. */
    public static void ResetBoard()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardState[i, j] != Cell.Invalid)
                {
                    BoardState[i, j] = Cell.Vacant;
                }
            }
        }
        foreach (Intersection i in Resources.FindObjectsOfTypeAll(typeof(Intersection)) as Intersection[])
        {
            i.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        //    i.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Sliced;
        //    i.GetComponent<SpriteRenderer>().size = new Vector2(3.5f, 1.5f);
        }

        turn = 0;
        currentPlayer = Player.White;
        computerPlayer = Random.Range(0, 2) == 1 ? Player.White : Player.Black;

        millFormed = false;
        movingPiece = false;

        whiteUnplacedPieces = 9;
        whiteRemainingPieces = 9;
        isWhitePhase3 = false;

        blackUnplacedPieces = 9;
        blackRemainingPieces = 9;
        isBlackPhase3 = false;

        gameOver = false;
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
                }
            }
            else
            {
                if (BoardState[3, 4] == cellCast && BoardState[3, 5] == cellCast && BoardState[3, 6] == cellCast)
                {
                    intersectionPartOfMill = true;
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
                }
            }
            else
            {
                if (BoardState[4, 3] == cellCast && BoardState[5, 3] == cellCast && BoardState[6, 3] == cellCast)
                {
                    intersectionPartOfMill = true;
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

    /* Returns true if there is at least one neighbor that is vacant to the input intersection. */
    public static bool HasAvailableVacantNeighbor(int row, int col)
    {
        /* Check the neighbor to the left. */
        for(int i = col - 1; i > -1; i--)
        {
            if (row == 3 && i == 3)
            {
                break;
            }
            if (BoardState[row, i] != Cell.Invalid)
            {
                if (BoardState[row, i] == Cell.Vacant)
                {
                    return true;
                }
                break;
            }
        }

        /* Check the neighbor to the right. */
        for(int i = col + 1; i < 7; i++)
        {
            if (row == 3 && i == 3)
            {
                break;
            }
            if (BoardState[row, i] != Cell.Invalid)
            {
                if (BoardState[row, i] == Cell.Vacant)
                {
                    return true;
                }
                break;
            }
        }

        /* Check the neighbor above. */
        for(int i = row + 1; i < 7; i++)
        {
            if (i == 3 && col == 3)
            {
                break;
            }
            if (BoardState[i, col] != Cell.Invalid)
            {
                if (BoardState[i, col] == Cell.Vacant)
                {
                    return true;
                }
                break;
            }
        }

        /* Check the neighbor below. */
        for(int i = row - 1; i > -1; i--)
        {
            if (i == 3 && col == 3)
            {
                break;
            }
            if (BoardState[i, col] != Cell.Invalid)
            {
                if (BoardState[i, col] == Cell.Vacant)
                {
                    return true;
                }
                break;
            }
        }

        return false;
    }

    /* Checks if a player has any moves available. */
    public static bool CheckAvailableMove(Player p)
    {
        Cell playerCell = p == Player.White ? Cell.White : Cell.Black;
        if( (blackUnplacedPieces > 0) ||
            (p == Player.White && isWhitePhase3) ||
            (p == Player.Black && isBlackPhase3) )
        {
            return true;

        }

        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if(BoardState[i, j] == playerCell)
                {
                    if(HasAvailableVacantNeighbor(i, j))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /* Phase 1: Player is adding a piece to the board. */
    public static void Phase1Placement(GameObject g, int row, int column)
    {
        var s = g.GetComponent<SpriteRenderer>();

        if (currentPlayer == Player.White)
        {
            BoardState[row, column] = Cell.White;
            s.color = new Color(1, 1, 1, 1);
            whiteUnplacedPieces--;
        }

        else
        {
            BoardState[row, column] = Cell.Black;
            s.color = new Color(0.3f, 0.3f, 0.3f, 1);
            blackUnplacedPieces--;
        }

        millFormed = CheckMill(currentPlayer, row, column);
        if (millFormed != true)
        {
            currentPlayer = GetOppositePlayer();
            turn++;
        }

        /* During the transition to phase 2, check if white has any available moves going
         * into their turn. If they don't, black wins. */
        if (blackUnplacedPieces == 0)
        {
            if(!CheckAvailableMove(Player.White))
            {
                GameOver(Player.Black);
            }
        }
    }

    /* Phase 2/3: Player is selecting a piece to move. */
    public static void PieceSelection(GameObject g, int row, int column)
    {
        GameObject go2 = GameObject.Find("BoardManager");
        BoardManager b = go2.GetComponent<BoardManager>();
        var s = g.GetComponent<SpriteRenderer>();

        lastSelected = g;
        tempRow = row;
        tempCol = column;

        if(BoardState[row, column] == Cell.Black || BoardState[row, column] == Cell.White)
        {
            BoardState[row, column] = Cell.Vacant;
            s.sprite = b.manSelected;
            movingPiece = true;
        }

        else
        {
            print("Invaild spot, please try again.");
        }
    }

    /* Phase 2/3: Player is moving a piece they have already selected. */
    public static void PieceMovement(GameObject g, int row, int column)
    {
        GameObject go2 = GameObject.Find("BoardManager");
        BoardManager b = go2.GetComponent<BoardManager>();
        var s = g.GetComponent<SpriteRenderer>();

        if (currentPlayer == Player.White)
        {

            if (CheckSamePosition(row, tempRow, column, tempCol))
            {
                BoardState[row, column] = Cell.White;
                movingPiece = false;

                s.color = new Color(1, 1, 1, 1);
                s.sprite = b.man;
            }

            else if (isWhitePhase3 || isAdjacent(tempRow, tempCol, row, column))
            {
                BoardState[row, column] = Cell.White;
                movingPiece = false;

                s.color = new Color(1, 1, 1, 1);
                lastSelected.GetComponent<SpriteRenderer>().sprite = b.man;
                lastSelected.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);

                millFormed = CheckMill(currentPlayer, row, column);
                if (millFormed != true)
                {
                    currentPlayer = GetOppositePlayer();
                    turn++;
                }
            }

            else
            {
                print("Invalid spot, please try again.");
            }

            /* After moving a piece, check if the opposing player has been locked in. */
            if (!CheckAvailableMove(Player.Black))
            {
                GameOver(Player.White);
            }
        }

        else
        {
            if (CheckSamePosition(row, tempRow, column, tempCol))
            {
                BoardState[row, column] = Cell.Black;
                movingPiece = false;

                s.color = new Color(0.3f, 0.3f, 0.3f, 1);
                s.sprite = b.man;
            }

            else if (isBlackPhase3 || isAdjacent(tempRow, tempCol, row, column))
            {
                BoardState[row, column] = Cell.Black;
                movingPiece = false;

                s.color = new Color(0.3f, 0.3f, 0.3f, 1);
                lastSelected.GetComponent<SpriteRenderer>().sprite = b.man;
                lastSelected.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);

                millFormed = CheckMill(currentPlayer, row, column);
                if (millFormed != true)
                {
                    currentPlayer = GetOppositePlayer();
                    turn++;
                }
            }

            else
            {
                print("Invalid spot, please try again.");
            }

            /* After moving a piece, check if the opposing player has been locked in. */
            if (!CheckAvailableMove(Player.White))
            {
                GameOver(Player.Black);
            }
        }
    }

    /* Remove piece after validating that the piece can be removed by milling player. 
     * Then, check for phase 3 condition or GameOver condition. */
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

        /* Check phase 3 and game over conditions. */
        if (currentPlayer == Player.White)
        {
            if (blackRemainingPieces == 3)
            {
                isBlackPhase3 = true;
            }

            if (blackRemainingPieces < 3 || !CheckAvailableMove(Player.Black))
            {
                GameOver(Player.White);
            }
        }
        else
        {
            if (whiteRemainingPieces == 3)
            {
                isWhitePhase3 = true;
            }

            if (whiteRemainingPieces < 3 || !CheckAvailableMove(Player.White))
            {
                GameOver(Player.Black);
            }
        }

        currentPlayer = GetOppositePlayer();
        turn++;
    }

    /* Computer AI Oppenent */
    public static void ComputerTurn()
    {
        GameObject g;
        Player humanPlayer = GetOppositePlayer();
        Cell humanPlayerCell = humanPlayer == Player.White ? Cell.White : Cell.Black;
        List<string> moves = new List<string>();
        int randMove;
        Intersection intersection;

        /* The computer has formed a mill and must pick a piece to remove. */
        if (millFormed)
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (BoardState[i, j] == humanPlayerCell)
                    {
                        moves.Add((char)(j + 97) + "" + (i + 1));
                    }
                }
            }

            if (moves.Count > 0)
            {
                randMove = Random.Range(0, moves.Count);

                g = FindIntersection(moves[randMove]);
                intersection = g.GetComponent<Intersection>();
                intersection.JumpTable();
                return;
            }
        }

        /* Priority 3: Blocking Mills
         * Iterate across the entire board and create a list of nodes that gives the player a mill next turn. */
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardState[i, j] == Cell.Vacant)
                {
                    BoardState[i, j] = humanPlayerCell;
                    bool possibleMillFormed = CheckMill(humanPlayer, i, j);
                    /* Take the coordinate (i, j) and create the equivalent string a1 that refers to the intersection. */
                    if(possibleMillFormed)
                    {
                        moves.Add((char)(j + 97) + "" + (i + 1));
                    }
                    BoardState[i, j] = Cell.Vacant;
                }
            }
        }

        if(moves.Count > 0)
        {
            randMove = Random.Range(0, moves.Count);

            g = FindIntersection(moves[randMove]);
            intersection = g.GetComponent<Intersection>();
            intersection.JumpTable();
            return;
        }

        /* Priority 5: Randomly Move
         * Iterate across the entire board and create a list of all vacant spaces. */
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardState[i, j] == Cell.Vacant)
                {
                    /* Take the coordinate (i, j) and create the equivalent string a1 that refers to the intersection. */
                    moves.Add((char)(j + 97) + "" + (i + 1));
                }
            }
        }

        /* Pick a random move from the list generated. */
        randMove = Random.Range(0, moves.Count);

        /* Make that move. */
        g = FindIntersection(moves[randMove]);
        intersection = g.GetComponent<Intersection>();
        intersection.JumpTable();
    }

    /* Initiates game over sequence; draw if no player. */
    private static void GameOver()
    {
        print("The game is a draw. Press R to start a new game.");

        return;
    }

    /* Initiates game over sequence; parameter p is the winning player. */
    private static void GameOver(Player p)
    {
        print("Game over! The winner is: " + p + ". Press R to start a new game.");
        gameOver = true;

        return;
    }

    public static bool CheckSamePosition(int r1, int r2, int c1, int c2)
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

        if (Input.GetKeyDown("o"))
        {
            computerIsActive = true;
        }

        if(computerIsActive && currentPlayer == computerPlayer)
        {
            if (computerTime > 0)
            {
                computerTime -= Time.deltaTime;
            }
            else
            {
                computerTime = 1.5;
                ComputerTurn();
            }
        }

        if (turn > 100)
        {
            GameOver();
        }
    }

            // PLEASE ignore this thing, I couldn't think of a way to get it working with multi-dimensional arrays
    public static bool isAdjacent(int row, int column, int tarRow, int tarCol)
    {
        if (tarRow == 6 && tarCol == 0)                 // a7
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 6, 3 };
            int[] neighbor2 = new int[] { 3, 0 };
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2))
            {
                return true;
            }
        }
        if (tarRow == 6 && tarCol == 3)                 // d7
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 6, 0 };
            int[] neighbor2 = new int[] { 6, 6 };
            int[] neighbor3 = new int[] { 5, 3 };
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2) || src.SequenceEqual(neighbor3))
            {
                return true;
            }
        }
        if (tarRow == 6 && tarCol == 6)                 // g7
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 6, 3 };
            int[] neighbor2 = new int[] { 3, 6 };
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2))
            {
                return true;
            }
        }
        if (tarRow == 5 && tarCol == 1)                 // b6
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 5, 3 };
            int[] neighbor2 = new int[] { 3, 1 };
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2))
            {
                return true;
            }
        }
        if (tarRow == 5 && tarCol == 3)                 // d6
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 6, 3 };
            int[] neighbor2 = new int[] { 5, 1 };
            int[] neighbor3 = new int[] { 5, 5 };
            int[] neighbor4 = new int[] { 4, 3 };
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2) || src.SequenceEqual(neighbor3) || src.SequenceEqual(neighbor4))
            {
                return true;
            }
        }
        if (tarRow == 5 && tarCol == 5)                 // f6
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 5, 3 };
            int[] neighbor2 = new int[] { 3, 5 };
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2))
            {
                return true;
            }
        }
        if (tarRow == 4 && tarCol == 2)                 // c5
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 4, 3 };
            int[] neighbor2 = new int[] { 3, 2 };
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2))
            {
                return true;
            }
        }
        if (tarRow == 4 && tarCol == 3)                 // d5
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 5, 3 };
            int[] neighbor2 = new int[] { 4, 2 };
            int[] neighbor3 = new int[] { 4, 4 };
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2) || src.SequenceEqual(neighbor3))
            {
                return true;
            }
        }
        if (tarRow == 4 && tarCol == 4)                 // e5
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 4, 3 };
            int[] neighbor2 = new int[] { 3, 4 };
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2))
            {
                return true;
            }
        }
        if (tarRow == 3 && tarCol == 0)                 // a4
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 6, 0};
            int[] neighbor2 = new int[] { 3, 1};
            int[] neighbor3 = new int[] { 0, 0};
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2) || src.SequenceEqual(neighbor3))
            {
                return true;
            }
        }
        if (tarRow == 3 && tarCol == 1)                 // b4
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 5, 1 };
            int[] neighbor2 = new int[] { 3, 0 };
            int[] neighbor3 = new int[] { 3, 2 };
            int[] neighbor4 = new int[] { 1, 1 };
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2) || src.SequenceEqual(neighbor3) || src.SequenceEqual(neighbor4))
            {
                return true;
            }
        }
        if (tarRow == 3 && tarCol == 2)                 // c4
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 4, 2 };
            int[] neighbor2 = new int[] { 3, 1 };
            int[] neighbor3 = new int[] { 2, 2 };
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2) || src.SequenceEqual(neighbor3))
            {
                return true;
            }
        }
        if (tarRow == 3 && tarCol == 4)                 // e4
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 4, 4 };
            int[] neighbor2 = new int[] { 3, 5};
            int[] neighbor3 = new int[] { 2, 4};
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2) || src.SequenceEqual(neighbor3))
            {
                return true;
            }
        }
        if (tarRow == 3 && tarCol == 5)                 // f4
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 5, 5};
            int[] neighbor2 = new int[] { 3, 4};
            int[] neighbor3 = new int[] { 3, 6};
            int[] neighbor4 = new int[] { 1, 5};
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2) || src.SequenceEqual(neighbor3) || src.SequenceEqual(neighbor4))
            {
                return true;
            }
        }
        if (tarRow == 3 && tarCol == 6)                 // g4
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 6, 6 };
            int[] neighbor2 = new int[] { 3, 5 };
            int[] neighbor3 = new int[] { 0, 6 };
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2) || src.SequenceEqual(neighbor3))
            {
                return true;
            }
        }
        if (tarRow == 2 && tarCol == 2)                 // c3
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 3, 2 };
            int[] neighbor2 = new int[] { 2, 3 };
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2))
            {
                return true;
            }
        }
        if (tarRow == 2 && tarCol == 3)                 // d3
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 2, 2 };
            int[] neighbor2 = new int[] { 2, 4 };
            int[] neighbor3 = new int[] { 1, 3 };
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2) || src.SequenceEqual(neighbor3))
            {
                return true;
            }
        }
        if (tarRow == 2 && tarCol == 4)                 // e3
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 2, 3 };
            int[] neighbor2 = new int[] { 3, 4 };
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2))
            {
                return true;
            }
        }
        if (tarRow == 1 && tarCol == 1)                 // b2
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 3, 1 };
            int[] neighbor2 = new int[] { 1, 3};
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2))
            {
                return true;
            }
        }
        if (tarRow == 1 && tarCol == 3)                 // d2
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 2, 3 };
            int[] neighbor2 = new int[] { 1, 1 };
            int[] neighbor3 = new int[] { 1, 5 };
            int[] neighbor4 = new int[] { 0, 3 };
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2) || src.SequenceEqual(neighbor3) || src.SequenceEqual(neighbor4))
            {
                return true;
            }
        }
        if (tarRow == 1 && tarCol == 5)                 // f2
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 3, 5 };
            int[] neighbor2 = new int[] { 1, 3 };
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2))
            {
                return true;
            }
        }
        if (tarRow == 0 && tarCol == 0)                 // a1
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 3, 0 };
            int[] neighbor2 = new int[] { 0, 3 };
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2))
            {
                return true;
            }
        }
        if (tarRow == 0 && tarCol == 3)                 // d1
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 1, 3 };
            int[] neighbor2 = new int[] { 0, 0 };
            int[] neighbor3 = new int[] { 0, 6 };
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2) || src.SequenceEqual(neighbor3))
            {
                return true;
            }
        }
        if (tarRow == 0 && tarCol == 6)                 // g1
        {
            int[] src = new int[] { row, column };
            int[] neighbor1 = new int[] { 3, 6};
            int[] neighbor2 = new int[] { 0, 3};
            if (src.SequenceEqual(neighbor1) || src.SequenceEqual(neighbor2))
            {
                return true;
            }
        }
        return false;
    }
}
