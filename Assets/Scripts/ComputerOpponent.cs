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
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ComputerOpponent : MonoBehaviour
{
    public static Player computerPlayer;
    public static bool isActive;
    public static double computerTime = 1.5;
    private Text whiteEventText;
    private Text blackEventText;
    private TextManager textManager;

    void Start()
    {
        whiteEventText = GameObject.Find("WhiteEventText").GetComponent<Text>();
        blackEventText = GameObject.Find("BlackEventText").GetComponent<Text>();
        textManager = GameObject.Find("StatusText").GetComponent<TextManager>();
    }

    /* Choose a new color for the computer and reset its turn timer. */
    public static void Reset()
    {
        computerPlayer = Random.Range(0, 2) == 1 ? Player.White : Player.Black;
        computerTime = 1.5;
    }

    /* Takes in as input a list of moves then chooses one at random and performs a
     * click on that intersection. It also returns the piece that was clicked on.
     * If the list is empty, it returns the empty string. */
    private string MakeRandomMoveFromList(List<string> moves)
    {
        if(moves.Count > 0)
        {
            /* Select a random move from the list of calculated moves. */
            int selectRandomMove = Random.Range(0, moves.Count);

            /* Perform a click on that piece. */
            GameObject g = BoardManager.FindIntersection(moves[selectRandomMove]);
            Intersection intersection = g.GetComponent<Intersection>();
            intersection.JumpTable();
            return moves[selectRandomMove];
        }
        else
        {
            return "";
        }
    }

    /* Decide from the state of the game which action should happen next. */
    private void DecisionTree()
    {
        /* Get the cell equivalent for the opposite player. */
        Cell currentPlayerCell = BoardManager.currentPlayer == Player.White ? Cell.White : Cell.Black;
        Cell oppositePlayerCell = BoardManager.currentPlayer == Player.White ? Cell.Black : Cell.White;

        /* A mill has been formed then this click represents the removal of a piece. */
        if (BoardManager.millFormed == true)
        {
            ComputerMill();
        }

        /* If black has unplayed pieces, still in phase 1. */
        else if (BoardManager.blackUnplacedPieces > 0)
        {
            ComputerPlacement();
        }

        else if ( (BoardManager.whiteRemainingPieces == 3 && computerPlayer == Player.White) ||
            (BoardManager.blackRemainingPieces == 3 && computerPlayer == Player.Black) )
        {
            if (BoardManager.movingPiece == true)
            {
                ComputerPlacement();
            }
            else
            {
                ComputerPhaseThree();
            }
        }

        /* If moving a piece and more than three remaining pieces, phase 2 movement. */
        else if(BoardManager.movingPiece == true)
        {
            ComputerPhaseTwoMove();
        }

        /* Last possible combination: phase 2. */
        else
        {
            ComputerPhaseTwoSelect();
        }
    }

    /* The computer has formed a mill and must remove a piece. */
    private void ComputerMill()
    {
        bool allPlayerMenInMill = BoardManager.AllMenInMill();
        bool isNodeNotPartOfMill;
        Player humanPlayer = BoardManager.GetOppositePlayer();
        Cell humanPlayerCell = humanPlayer == Player.White ? Cell.White : Cell.Black;
        List<string> possibleMills = new List<string>();

        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardManager.BoardState[i, j] == humanPlayerCell)
                {
                    /* A piece can only be removed if it is not part of a mill
                     * OR if all pieces of that player are in a mill. */
                    isNodeNotPartOfMill = !BoardManager.CheckMill(humanPlayer, i, j);
                    if (allPlayerMenInMill || isNodeNotPartOfMill)
                    {
                        /* Add the node to the list of possible mills. */
                        possibleMills.Add((char)(j + 97) + "" + (i + 1));
                    }
                }
            }
        }

        MakeRandomMoveFromList(possibleMills);
    }

    /* The computer is placing its pieces in phase 1. */
    private void ComputerPlacement()
    {
        Player humanPlayer = BoardManager.GetOppositePlayer();
        Cell humanPlayerCell = humanPlayer == Player.White ? Cell.White : Cell.Black;
        Cell computerPlayerCell = computerPlayer == Player.White ? Cell.White : Cell.Black;
        List<string> moves = new List<string>();

        bool possibleMillFormed;

        /* Priority 2: Forming Mills
        * Iterate across the entire board and create a list of nodes that gives the player a mill next turn. */
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardManager.BoardState[i, j] == Cell.Vacant)
                {
                    BoardManager.BoardState[i, j] = computerPlayerCell;
                    possibleMillFormed = BoardManager.CheckMill(computerPlayer, i, j);
                    /* Take the coordinate (i, j) and create the equivalent string a1 that refers to the intersection. */
                    if (possibleMillFormed)
                    {
                        moves.Add((char)(j + 97) + "" + (i + 1));
                    }
                    BoardManager.BoardState[i, j] = Cell.Vacant;
                }
            }
        }

        if (MakeRandomMoveFromList(moves) != "")
        {
            return;
        }

        /* Priority 3: Blocking Mills
        * Iterate across the entire board and create a list of nodes that gives the player a mill next turn. */
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardManager.BoardState[i, j] == Cell.Vacant)
                {
                    BoardManager.BoardState[i, j] = humanPlayerCell;
                    possibleMillFormed = BoardManager.CheckMill(humanPlayer, i, j);
                    if (possibleMillFormed)
                    {
                        moves.Add((char)(j + 97) + "" + (i + 1));
                    }
                    BoardManager.BoardState[i, j] = Cell.Vacant;
                }
            }
        }

        if (MakeRandomMoveFromList(moves) != "")
        {
            return;
        }

        /* Priority 4: Placing Near Other Pieces
         * Iterate across the entire board and create a list of adjacent nodes that the computer should start building mills next to. */
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardManager.BoardState[i, j] == computerPlayerCell)
                {
                    List<string> possibleMoves = BoardManager.getAdjacencyList(i, j);
                    foreach(string possibleMove in possibleMoves)
                    {
                        int row = (int)(possibleMove[1] - '1');
                        int column = (int)(possibleMove[0] - 'a');
                        if (BoardManager.BoardState[row, column] == Cell.Vacant)
                        {
                            moves.Add(possibleMove);
                        }
                    }
                }
            }
        }

        if (MakeRandomMoveFromList(moves) != "")
        {
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

        MakeRandomMoveFromList(moves);
    }

    /* This will choose a piece to move then move it. We should probably
     * separate this into two different functions; one that chooses a piece to move,
     * and then one that actually makes that move. TODO */
    private void ComputerPhaseTwoSelect()
    {
        Cell computerPlayerCell = computerPlayer == Player.White ? Cell.White : Cell.Black;
        List<string> possiblePieceToSelect = new List<string>();

        /* Priority 2: Forming Mills
         * Iterate across the entire board and create a list of nodes that gives the player a mill next turn. */
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardManager.BoardState[i, j] == Cell.Vacant)
                {
                    BoardManager.BoardState[i, j] = computerPlayerCell;
                    if (BoardManager.CheckMill(computerPlayer, i, j))
                    {
                        List<string> possibleMoves = BoardManager.getAdjacencyList(i, j);

                        foreach (string possibleMove in possibleMoves)
                        {
                            int row = (int)(possibleMove[1] - '1');
                            int column = (int)(possibleMove[0] - 'a');

                            if(BoardManager.BoardState[row, column] == computerPlayerCell)
                            {
                                BoardManager.BoardState[row, column] = Cell.Vacant;

                                if (BoardManager.CheckMill(computerPlayer, i, j))
                                {
                                    possiblePieceToSelect.Add((char)(column + 97) + "" + (row + 1));
                                }

                                BoardManager.BoardState[row, column] = computerPlayerCell;
                            }
                        }
                    }
                    BoardManager.BoardState[i, j] = Cell.Vacant;
                }
            }
        }

        if (MakeRandomMoveFromList(possiblePieceToSelect) != "")
        {
            return;
        }

        /* Priority 5: Randomly Move
         * Iterate across the entire board and create a list of all vacant spaces. */
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardManager.BoardState[i, j] == computerPlayerCell && BoardManager.HasAvailableVacantNeighbor(i, j))
                {
                    possiblePieceToSelect.Add((char)(j + 97) + "" + (i + 1));
                }
            }
        }

        MakeRandomMoveFromList(possiblePieceToSelect);
    }

    private void ComputerPhaseTwoMove()
    {
        Cell computerPlayerCell = computerPlayer == Player.White ? Cell.White : Cell.Black;
        int row = BoardManager.tempRow;
        int column = BoardManager.tempCol;
        List<string> possibleMovesOfPiece = BoardManager.getAdjacencyList(row, column);
        List<string> vacantNeighborsOfSelectedPiece = new List<string>();

        /* Priority 2: Forming Mills
         * Iterate across the entire board and create a list of nodes that gives the player a mill next turn. */
        foreach (string str in possibleMovesOfPiece)
        {
            int adjacentRow = (int)(str[1] - '1');
            int adjacentColumn = (int)(str[0] - 'a');
            if (BoardManager.BoardState[adjacentRow, adjacentColumn] == Cell.Vacant)
            {
                BoardManager.BoardState[adjacentRow, adjacentColumn] = computerPlayerCell;
                if(BoardManager.CheckMill(computerPlayer, adjacentRow, adjacentColumn))
                {
                    vacantNeighborsOfSelectedPiece.Add(str);
                }
                BoardManager.BoardState[adjacentRow, adjacentColumn] = Cell.Vacant;
            }
        }

        if (MakeRandomMoveFromList(vacantNeighborsOfSelectedPiece) != "")
        {
            return;
        }

        /* Priority 5: Randomly Move
         * Iterate across the entire board and create a list of all vacant spaces. */
        foreach (string str in possibleMovesOfPiece)
        {
            int adjacentRow = (int)(str[1] - '1');
            int adjacentColumn = (int)(str[0] - 'a');

            if (BoardManager.BoardState[adjacentRow, adjacentColumn] == Cell.Vacant)
            {
                vacantNeighborsOfSelectedPiece.Add(str);
            }
        }

        MakeRandomMoveFromList(vacantNeighborsOfSelectedPiece);
    }

    private void ComputerPhaseThree()
    {
        Cell computerPlayerCell = computerPlayer == Player.White ? Cell.White : Cell.Black;
        List<string> selectionOfPiece = new List<string>();

        /* Priority 2: Forming Mills
        * Iterate across the entire board and create a list of nodes that gives the player a mill next turn. */
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardManager.BoardState[i, j] == Cell.Vacant)
                {
                    BoardManager.BoardState[i, j] = computerPlayerCell;
                    if(BoardManager.CheckMill(computerPlayer, i, j))
                    {
                        for (int i2 = 0; i2 < 7; i2++)
                        {
                            for (int j2 = 0; j2 < 7; j2++)
                            {
                                if(BoardManager.BoardState[i2, j2] == computerPlayerCell && i != i2 && j != j2)
                                {
                                    BoardManager.BoardState[i2, j2] = Cell.Vacant;
                                    if (BoardManager.CheckMill(computerPlayer, i, j))
                                    {
                                        selectionOfPiece.Add((char)(j2 + 97) + "" + (i2 + 1));
                                    }
                                    BoardManager.BoardState[i2, j2] = computerPlayerCell;
                                }
                            }
                        }
                    }
                    BoardManager.BoardState[i, j] = Cell.Vacant;
                }
            }
        }

        if (MakeRandomMoveFromList(selectionOfPiece) != "")
        {
            return;
        }

        /* Priority 5: Randomly Move
         * Iterate across the entire board and create a list of all vacant spaces. */
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

        MakeRandomMoveFromList(selectionOfPiece);
    }

    private void Update()
    {
        if (isActive && BoardManager.currentPlayer == computerPlayer && !BoardManager.gameOver)
        {
            if (computerTime > 0)
            {
                textManager.updateEventText("I'm Thinking...", computerPlayer);
                computerTime -= Time.deltaTime;
            }
            else
            {
                computerTime = 1.5;
                textManager.updateEventText("", computerPlayer);
                DecisionTree();
            }
        }
    }

}
