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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ComputerOpponent : MonoBehaviour
{
    public static Player computerPlayer;
    public static bool isActive = true;
    public static double computerTime = 1.5;

    public static void Reset()
    {
        computerPlayer = Random.Range(0, 2) == 1 ? Player.White : Player.Black;
        computerTime = 1.5;
    }

    /* Computer AI Oppenent */
    private void ComputerPhaseOne()
    {
        GameObject g;
        Player humanPlayer = BoardManager.GetOppositePlayer();
        Cell humanPlayerCell = humanPlayer == Player.White ? Cell.White : Cell.Black;
        Cell computerPlayerCell = computerPlayer == Player.White ? Cell.White : Cell.Black;
        List<string> moves = new List<string>();
        List<string> selection = new List<string>();
        int randMove;
        Intersection intersection;

        if (BoardManager.blackRemainingPieces == 3)
        {
            computerPhaseThree();
            return;
        }

        if (BoardManager.blackUnplacedPieces == 0)
        {
            computerPhaseTwo();
            return;
        }

        /* The computer has formed a mill and must pick a piece to remove. */
        if (BoardManager.millFormed)
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (BoardManager.BoardState[i, j] == humanPlayerCell)
                    {
                        moves.Add((char)(j + 97) + "" + (i + 1));
                    }
                }
            }

            if (moves.Count > 0)
            {
                randMove = Random.Range(0, moves.Count);

                g = BoardManager.FindIntersection(moves[randMove]);
                intersection = g.GetComponent<Intersection>();
                intersection.JumpTable();
                return;
            }
        }
        /* Priority 3: Forming Mills
        * Iterate across the entire board and create a list of nodes that gives the player a mill next turn. */
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardManager.BoardState[i, j] == Cell.Vacant)
                {
                    BoardManager.BoardState[i, j] = computerPlayerCell;
                    bool possibleMillFormed = BoardManager.CheckMill(computerPlayer, i, j);
                    /* Take the coordinate (i, j) and create the equivalent string a1 that refers to the intersection. */
                    if (possibleMillFormed)
                    {
                        moves.Add((char)(j + 97) + "" + (i + 1));
                    }
                    BoardManager.BoardState[i, j] = Cell.Vacant;
                }
            }
        }

        if (moves.Count > 0)
        {
            randMove = Random.Range(0, moves.Count);

            g = BoardManager.FindIntersection(moves[randMove]);
            intersection = g.GetComponent<Intersection>();
            intersection.JumpTable();
            return;
        }

        /* Priority 2: Blocking Mills
        * Iterate across the entire board and create a list of nodes that gives the player a mill next turn. */
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardManager.BoardState[i, j] == Cell.Vacant)
                {
                    BoardManager.BoardState[i, j] = humanPlayerCell;
                    bool possibleMillFormed = BoardManager.CheckMill(humanPlayer, i, j);
                    /* Take the coordinate (i, j) and create the equivalent string a1 that refers to the intersection. */
                    if (possibleMillFormed)
                    {
                        moves.Add((char)(j + 97) + "" + (i + 1));
                    }
                    BoardManager.BoardState[i, j] = Cell.Vacant;
                }
            }
        }

        if (moves.Count > 0)
        {
            randMove = Random.Range(0, moves.Count);

            g = BoardManager.FindIntersection(moves[randMove]);
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
                if (BoardManager.BoardState[i, j] == Cell.Vacant)
                {
                    /* Take the coordinate (i, j) and create the equivalent string a1 that refers to the intersection. */
                    moves.Add((char)(j + 97) + "" + (i + 1));
                }
            }
        }

        /* Pick a random move from the list generated. */
        randMove = Random.Range(0, moves.Count);

        /* Make that move. */
        g = BoardManager.FindIntersection(moves[randMove]);
        intersection = g.GetComponent<Intersection>();
        intersection.JumpTable();
    }

    private void computerPhaseTwo()
    {
        GameObject g;
        Cell computerPlayerCell = computerPlayer == Player.White ? Cell.White : Cell.Black;
        List<string> movesOfPieces = new List<string>();
        int randomMove;
        string randSelection;
        List<string> selectionOfPiece = new List<string>();
        Intersection intersection;

        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardManager.BoardState[i, j] == computerPlayerCell)
                {
                    /* Take the coordinate (i, j) and create the equivalent string a1 that refers to the intersection. */
                    selectionOfPiece.Add((char)(j + 97) + "" + (i + 1));
                }
            }
        }

        /* Pick a random move from the list generated. */
        randSelection = selectionOfPiece[Random.Range(0, selectionOfPiece.Count)];
        g = BoardManager.FindIntersection(randSelection);
        intersection = g.GetComponent<Intersection>();
        intersection.JumpTable();

        movesOfPieces = BoardManager.getAdjacencyList(intersection.row, intersection.column);

        /*Intersection eliminateOccupied;
        foreach (string str in movesOfPieces)
        {
            eliminateOccupied = BoardManager.FindIntersection(str).GetComponent<Intersection>();
            if (BoardManager.BoardState[eliminateOccupied.row, eliminateOccupied.column] != Cell.Vacant)
            {
                movesOfPieces.Remove(str);
            }
        }*/

        /* Pick a random move from the list generated. */
        randomMove = Random.Range(0, movesOfPieces.Count);

        /* Make that move. */

        g = BoardManager.FindIntersection(movesOfPieces[randomMove]);
        intersection = g.GetComponent<Intersection>();
        intersection.JumpTable();
    }

    private void computerPhaseThree()
    {
        GameObject g;
        Cell computerPlayerCell = computerPlayer == Player.White ? Cell.White : Cell.Black;
        List<string> movesOfPieces = new List<string>();
        List<string> selectionOfPiece = new List<string>();
        int randMove;
        string randSelection;
        Intersection intersection;

        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardManager.BoardState[i, j] == computerPlayerCell)
                {
                    /* Take the coordinate (i, j) and create the equivalent string a1 that refers to the intersection. */
                    selectionOfPiece.Add((char)(j + 97) + "" + (i + 1));
                }
            }
        }

        randSelection = selectionOfPiece[Random.Range(0, selectionOfPiece.Count)];
        g = BoardManager.FindIntersection(randSelection);
        intersection = g.GetComponent<Intersection>();
        intersection.JumpTable();

        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardManager.BoardState[i, j] == Cell.Vacant)
                {
                    /* Take the coordinate (i, j) and create the equivalent string a1 that refers to the intersection. */
                    movesOfPieces.Add((char)(j + 97) + "" + (i + 1));
                }
            }
        }

        /* Pick a random move from the list generated. */
        randMove = Random.Range(0, movesOfPieces.Count);

        /* Make that move. */
        g = BoardManager.FindIntersection(movesOfPieces[randMove]);
        intersection = g.GetComponent<Intersection>();
        intersection.JumpTable();
    }

    private void Update()
    {
        if (isActive && BoardManager.currentPlayer == computerPlayer)
        {
            if (computerTime > 0)
            {
                computerTime -= Time.deltaTime;
            }
            else
            {
                computerTime = 1.5;
                ComputerPhaseOne();
            }
        }
    }

}
