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
    private void ComputerTurn()
    {
        GameObject g;
        Player humanPlayer = BoardManager.GetOppositePlayer();
        Cell humanPlayerCell = humanPlayer == Player.White ? Cell.White : Cell.Black;
        List<string> moves = new List<string>();
        List<string> selection = new List<string>();
        int randMove;
        int randSelection;
        Intersection intersection;

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

        if (BoardManager.currentPlayer == Player.Black)
        {
            /* Priority 3: Blocking Mills
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


            if (BoardManager.blackUnplacedPieces == 0)
            {
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        if (BoardManager.BoardState[i, j] == Cell.Black)
                        {
                            /* Take the coordinate (i, j) and create the equivalent string a1 that refers to the intersection. */
                            selection.Add((char)(j + 97) + "" + (i + 1));
                        }
                    }
                }

                /* Pick a random move from the list generated. */
                randSelection = Random.Range(0, selection.Count);

                /* Make that move. */
                g = BoardManager.FindIntersection(selection[randSelection]);
                intersection = g.GetComponent<Intersection>();
                intersection.JumpTable();

                computerMove(g, randSelection);
            }
        }

    }

    private void computerMove(GameObject g, int randomSelection)
    {
        List<string> movesOfPieces = new List<string>();
        int randomMove;
        List<string> selectionOfPiece = new List<string>();
        Intersection intersection;

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
        randomMove = Random.Range(0, movesOfPieces.Count);

        /* Make that move. */
        g = BoardManager.FindIntersection(movesOfPieces[randomMove]);
        intersection = g.GetComponent<Intersection>();

        if (isAdjacentForComputer(selectionOfPiece, movesOfPieces))
        {
            intersection.JumpTable();
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
                ComputerTurn();
            }
        }
    }

}
