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

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Intersection : MonoBehaviour
{
    public int row;
    public int column;
    private Text whiteEventText;
    private Text blackEventText;
    private TextManager textManager;


    void Start()
    {
        whiteEventText = GameObject.Find("WhiteEventText").GetComponent<Text>();
        blackEventText = GameObject.Find("BlackEventText").GetComponent<Text>();
        textManager = GameObject.Find("StatusText").GetComponent<TextManager>();
    }

    /* Special pointer constructor. */
    public static Intersection CreateComponent(GameObject location, int c, int r)
    {
        Intersection i = location.AddComponent<Intersection>();
        i.column = c;
        i.row = r;
        return i;
    }

    public void JumpTable()
    {
        /* Get the cell equivalent for the opposite player. */
        Cell currentPlayerCell = BoardManager.currentPlayer == Player.White ? Cell.White : Cell.Black;
        Cell oppositePlayerCell = BoardManager.currentPlayer == Player.White ? Cell.Black : Cell.White;

        /* A mill has been formed then this click represents the removal of a piece. */
        if (BoardManager.millFormed == true)
        {
            /* If a mill has been formed, then the click must be on an opposing cell. */
            if (BoardManager.BoardState[row, column] == oppositePlayerCell)
            {
                /* Piece removed can't be part of a mill... */
                if (!BoardManager.CheckMill(BoardManager.GetOppositePlayer(), row, column))
                {
                    BoardManager.Mill(gameObject, row, column);
                }
                /* ...unless all opposing pieces are part of mills. */
                else
                {
                    if (BoardManager.AllMenInMill())
                    {
                        BoardManager.Mill(gameObject, row, column);
                    }
                    else
                    {
                        print("There exists a piece not part of a mill. Please click on a piece not part of a mill.");
                    }
                }
            }
            else
            {
                print("Please click on an opposing piece");
            }
        }

        /* If black has unplayed pieces, still in phase 1. */
        else if (BoardManager.blackUnplacedPieces > 0)
        {
            if (BoardManager.BoardState[row, column] != Cell.Vacant)
            {
                return;
            }
            BoardManager.Phase1Placement(gameObject, row, column);
        }

        /* If moving piece is set to true, then pieces can be moved in phase 2. */
        else if (BoardManager.movingPiece == true)
        {
            if (BoardManager.BoardState[row, column] != Cell.Vacant)
            {
                return;
            }

            BoardManager.PieceMovement(gameObject, row, column);
        }

        /* Last possible combination: selecting a piece in phase 2/3. */
        else
        {
            if (BoardManager.BoardState[row, column] == currentPlayerCell)
            {
                if ((BoardManager.currentPlayer == Player.White && BoardManager.isWhitePhase3) ||
                    (BoardManager.currentPlayer == Player.Black && BoardManager.isBlackPhase3) ||
                    (BoardManager.HasAvailableVacantNeighbor(row, column)))
                {
                    BoardManager.PieceSelection(gameObject, row, column);
                }
                else
                {
                    print("This piece has no vacant spaces to move to.");
                }
            }
        }
    }

    public void OnMouseDown()
    {
        /* If it's the computer player's turn or the game is over, then the clicks do nothing! */
        if((ComputerOpponent.isActive && BoardManager.currentPlayer == ComputerOpponent.computerPlayer) || BoardManager.gameOver == true)
        {
            return;
        }

        JumpTable();
    }
}