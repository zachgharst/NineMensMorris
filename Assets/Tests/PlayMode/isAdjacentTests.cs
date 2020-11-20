using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class isAdjacentTests
    {
        [SetUp]
        public void SetUp()
        {
            GameObject g = new GameObject("BoardManager");
            g.AddComponent<BoardManager>();
            BoardManager.InitGame();
        }

        [TearDown]
        public void TearDown()
        {
            BoardManager.ResetBoard();
        }

        [Test]
        public void adjacencyValidInputTest()
        {
            int tTargetRow = 0;
            int tTargetCol = 0;
            int tSourceRow = 0;
            int tSourceCol = 3;
            Assert.IsTrue(BoardManager.isAdjacent(tSourceRow, tSourceCol, tTargetRow, tTargetCol));
        }
    }
}
