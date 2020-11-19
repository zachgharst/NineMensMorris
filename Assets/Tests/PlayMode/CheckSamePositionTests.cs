using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CheckSamePositionTests
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
        public void CheckSamePositionTestValidInput()
        {
            int targetRow = 6;
            int targetCol = 6;
            int sourceRow = 6;
            int sourceCol = 6;
            Assert.IsTrue(BoardManager.CheckSamePosition(sourceRow, targetRow, sourceCol, targetCol));
        }

        [Test]
        public void CheckSamePositionTestNotSameNode()
        {
            int targetRow = 3;
            int targetCol = 6;
            int sourceRow = 0;
            int sourceCol = 6;
            Assert.IsFalse(BoardManager.CheckSamePosition(sourceRow, targetRow, sourceCol, targetCol));
        }
    }
}
