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
        List<string> selection = new List<string>();
        int randMove;
        int randSelection;
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

        if (currentPlayer == Player.Black)
        {
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
                        if (possibleMillFormed)
                        {
                            moves.Add((char)(j + 97) + "" + (i + 1));
                        }
                        BoardState[i, j] = Cell.Vacant;
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


            if (blackUnplacedPieces == 0)
            {
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        if (BoardState[i, j] == Cell.Black)
                        {
                            /* Take the coordinate (i, j) and create the equivalent string a1 that refers to the intersection. */
                            selection.Add((char)(j + 97) + "" + (i + 1));
                        }
                    }
                }

                /* Pick a random move from the list generated. */
                randSelection = Random.Range(0, selection.Count);

                /* Make that move. */
                g = FindIntersection(selection[randSelection]);
                intersection = g.GetComponent<Intersection>();
                intersection.JumpTable();

                computerMove(g, randSelection);
            }
        }

    }

    public static void computerMove(GameObject g, int randomSelection)
    {
        List<string> movesOfPieces = new List<string>();
        int randomMove;
        List<string> selectionOfPiece = new List<string>();
        Intersection intersection;

        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardState[i, j] == Cell.Vacant)
                {
                    /* Take the coordinate (i, j) and create the equivalent string a1 that refers to the intersection. */
                    movesOfPieces.Add((char)(j + 97) + "" + (i + 1));
                }
            }
        }

        /* Pick a random move from the list generated. */
        randomMove = Random.Range(0, movesOfPieces.Count);

        /* Make that move. */
        g = FindIntersection(movesOfPieces[randomMove]);
        intersection = g.GetComponent<Intersection>();

        if (isAdjacentForComputer(selectionOfPiece, movesOfPieces))
        {
            intersection.JumpTable();
        }
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

    /* dame da ne
    dame yo dame na no yo */
    public static bool isAdjacentForComputer(List<string> starting, List<string> ending)
    {
        if (ending.Equals("a6"))                 // a7
        {
            List<string> startPoint1 = starting;
            List<string> n1 = new List<string>();
            n1.Add("d7");
            List<string> n2 = new List<string>();
            n2.Add("a4");
            if (starting.Equals(n1) || starting.Equals(n2))
            {
                return true;
            }
        }
        if (ending.Equals("d7"))                 // d7
        {
            List<string> startPoint2 = starting;
            List<string> n3 = new List<string>(); // a7
            n3.Add("a7");
            List<string> n4 = new List<string>(); // g7
            n4.Add("g7");
            List<string> n5 = new List<string>(); // d6
            n5.Add("d6");
            if (startPoint2.Equals(n3) || startPoint2.Equals(n4) || startPoint2.Equals(n5))
            {
                return true;
            }
        }
        if (ending.Equals("g7"))                 // g7
        {
            List<string> startPoint3 = starting;
            List<string> n6 = new List<string>(); // d7
            n6.Add("d7");
            List<string> n7 = new List<string>(); // g4
            n7.Add("g4");
            if (startPoint3.Equals(n6) || startPoint3.Equals(n7))
            {
                return true;
            }
        }
        if (ending.Equals("b6"))                 // b6
        {
            List<string> startPoint4 = starting;
            List<string> n8 = new List<string>(); // d6
            n8.Add("d6");
            List<string> n9 = new List<string>(); // b4
            n9.Add("b4");
            if (startPoint4.Equals(n8) || startPoint4.Equals(n9))
            {
                return true;
            }
        }
        if (ending.Equals("d6"))                 // d6
        {
            List<string> startPoint5 = starting;
            List<string> n10 = new List<string>(); // d7
            n10.Add("d7");
            List<string> n11 = new List<string>(); // b6
            n11.Add("b6");
            List<string> n12 = new List<string>(); // f6
            n12.Add("f6");
            List<string> n13 = new List<string>(); // d5
            n13.Add("d5");
            if (startPoint5.Equals(n10) || startPoint5.Equals(n11) || startPoint5.Equals(n12) || startPoint5.Equals(n13))
            {
                return true;
            }
        }
        if (ending.Equals("f6"))                // f6
        {
            List<string> startPoint6 = starting;
            List<string> n14 = new List<string>(); // d6
            n14.Add("d6");
            List<string> n15 = new List<string>(); // f4
            n15.Add("f4");
            if (startPoint6.Equals(n14) || startPoint6.Equals(n15))
            {
                return true;
            }
        }
        if (ending.Equals("c5"))                // c5
        {
            List<string> startPoint7 = starting;
            List<string> n16 = new List<string>(); // d5
            n16.Add("d5");
            List<string> n17 = new List<string>(); // c4
            n17.Add("c4");
            if (startPoint7.Equals(n16) || startPoint7.Equals(n17))
            {
                return true;
            }
        }
        if (ending.Equals("d5"))                 // d5
        {
            List<string> startPoint8 = starting;
            List<string> n18 = new List<string>(); // d6
            n18.Add("d6");
            List<string> n19 = new List<string>(); // c5
            n19.Add("c5");
            List<string> n20 = new List<string>(); // e5
            n20.Add("e5");
            if (startPoint8.Equals(n18) || (startPoint8.Equals(n19)) || (startPoint8.Equals(n20)))
            {
                return true;
            }
        }
        if (ending.Equals("e5"))                 // e5
        {
            List<string> startPoint9 = starting;
            List<string> n21 = new List<string>(); // d5
            n21.Add("d5");
            List<string> n22 = new List<string>(); // e4
            n22.Add("e4");
            if (startPoint9.Equals(n21) || startPoint9.Equals(n22))
            {
                return true;
            }
        }
        if (ending.Equals("a4"))                 // a4
        {
            List<string> startPoint10 = starting;
            List<string> n23 = new List<string>(); // a7
            n23.Add("a7");
            List<string> n24 = new List<string>(); // b1
            n24.Add("b1");
            List<string> n25 = new List<string>(); // a1
            n25.Add("a1");
            if (startPoint10.Equals(n23) || startPoint10.Equals(n24) || startPoint10.Equals(n25))
            {
                return true;
            }
        }
        if (ending.Equals("b4"))                 // b4
        {
            List<string> startPoint11 = starting;
            List<string> n26 = new List<string>(); // b6
            n26.Add("b6");
            List<string> n27 = new List<string>(); // a4
            n27.Add("a4");
            List<string> n28 = new List<string>(); // c4
            n28.Add("c4");
            List<string> n29 = new List<string>(); // b2
            n29.Add("b2");
            if (startPoint11.Equals(n26) || startPoint11.Equals(n27) || startPoint11.Equals(n28) || startPoint11.Equals(n29))
            {
                return true;
            }
        }
        if (ending.Equals("c4"))                 // c4
        {
            List<string> startPoint12 = starting;
            List<string> n30 = new List<string>(); // c5
            n30.Add("c5");
            List<string> n31 = new List<string>(); // b4
            n31.Add("b4");
            List<string> n32 = new List<string>(); // c3
            n32.Add("c3");
            if (startPoint12.Equals(n30) || startPoint12.Equals(n31) || startPoint12.Equals(n32))
            {
                return true;
            }
        }
        if (ending.Equals("e4"))                 // e4
        {
            List<string> startPoint13 = starting;
            List<string> n33 = new List<string>(); // e5
            n33.Add("e5");
            List<string> n34 = new List<string>(); // f4
            n34.Add("f4");
            List<string> n35 = new List<string>(); // e3
            n35.Add("e3");
            if (startPoint13.Equals(n33) || startPoint13.Equals(n34) || startPoint13.Equals(n35))
            {
                return true;
            }
        }
        if (ending.Equals("f4"))                 // f4
        {
            List<string> startPoint14 = starting;
            List<string> n36 = new List<string>(); // f6
            n36.Add("f6");
            List<string> n37 = new List<string>(); // e4
            n37.Add("e4");
            List<string> n38 = new List<string>(); // g4
            n38.Add("g4");
            List<string> n39 = new List<string>(); // f2
            n39.Add("f2");
            if (startPoint14.Equals(n36) || startPoint14.Equals(n37) || startPoint14.Equals(n38) || startPoint14.Equals(n39))
            {
                return true;
            }
        }
        if (ending.Equals("g4"))                 // g4
        {
            List<string> startPoint15 = starting;
            List<string> n40 = new List<string>(); // g7
            n40.Add("g7");
            List<string> n41 = new List<string>(); // f4
            n41.Add("f4");
            List<string> n42 = new List<string>(); // g1
            n42.Add("g1");
            if (startPoint15.Equals(n40) || startPoint15.Equals(n41) || startPoint15.Equals(n42))
            {
                return true;
            }
        }
        if (ending.Equals("c3"))                 // c3
        {
            List<string> startPoint16 = starting;
            List<string> n43 = new List<string>(); // c4
            n43.Add("c4");
            List<string> n44 = new List<string>(); // d3
            n44.Add("d3");
            if (startPoint16.Equals(n43) || startPoint16.Equals(n44))
            {
                return true;
            }
        }
        if (ending.Equals("d3"))                 // d3
        {
            List<string> startPoint17 = starting;
            List<string> n45 = new List<string>(); // c3
            n45.Add("c3");
            List<string> n46 = new List<string>(); // e3
            n46.Add("e3");
            List<string> n47 = new List<string>(); // d2
            n47.Add("d2");
            if (startPoint17.Equals(n45) || startPoint17.Equals(n46) || startPoint17.Equals(n47))
            {
                return true;
            }
        }
        if (ending.Equals("e3"))                 // e3
        {
            List<string> startPoint18 = starting;
            List<string> n48 = new List<string>(); // d3
            n48.Add("d3");
            List<string> n49 = new List<string>(); // e4
            n49.Add("e4");
            if (startPoint18.Equals(n48) || startPoint18.Equals(n49))
            {
                return true;
            }
        }
        if (ending.Equals("b2"))                 // b2
        {
            List<string> startPoint19 = starting;
            List<string> n50 = new List<string>(); // b4
            n50.Add("b4");
            List<string> n51 = new List<string>(); // d2
            n51.Add("d2");
            if (startPoint19.Equals(n50) || startPoint19.Equals(n51))
            {
                return true;
            }
        }
        if (ending.Equals("d2"))                 // d2
        {
            List<string> startPoint20 = starting;
            List<string> n52 = new List<string>(); // d3
            n52.Add("d3");
            List<string> n53 = new List<string>(); // b2
            n53.Add("b2");
            List<string> n54 = new List<string>(); // f2
            n54.Add("f2");
            List<string> n55 = new List<string>(); // d1
            n55.Add("d1");
            if (startPoint20.Equals(n52) || startPoint20.Equals(n53) || startPoint20.Equals(n54) || startPoint20.Equals(n55))
            {
                return true;
            }
        }
        if (ending.Equals("f2"))                 // f2
        {
            List<string> startPoint21 = starting;
            List<string> n56 = new List<string>(); // f4
            n56.Add("f4");
            List<string> n57 = new List<string>(); // d2
            n57.Add("d2");
            if (startPoint21.Equals(n56) || startPoint21.Equals(n57))
            {
                return true;
            }
        }
        if (ending.Equals("a1"))                 // a1
        {
            List<string> startPoint22 = starting;
            List<string> n58 = new List<string>(); // a4
            n58.Add("a4");
            List<string> n59 = new List<string>(); // d1
            n59.Add("d1");
            if (startPoint22.Equals(n58) || startPoint22.Equals(n59))
            {
                return true;
            }
        }
        if (ending.Equals("d1"))                 // d1
        {
            List<string> startPoint23 = starting;
            List<string> n60 = new List<string>(); // d2
            n60.Add("d2");
            List<string> n61 = new List<string>(); // a1
            n61.Add("a1");
            List<string> n62 = new List<string>(); // g1
            n62.Add("g1");
            if (startPoint23.Equals(n60) || startPoint23.Equals(n61) || startPoint23.Equals(n62))
            {
                return true;
            }
        }
        if (ending.Equals("g1"))                 // g1
        {
            List<string> startPoint24 = starting;
            List<string> n63 = new List<string>(); // g4
            n63.Add("g4");
            List<string> n64 = new List<string>(); // d1
            n64.Add("d1");
            if (startPoint24.Equals(n63) || startPoint24.Equals(n64))
            {
                return true;
            }
        }
        return false;
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
