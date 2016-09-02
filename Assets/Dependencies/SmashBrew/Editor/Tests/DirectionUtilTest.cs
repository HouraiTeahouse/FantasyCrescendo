using NUnit.Framework;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    internal class DirectionUtilTest {

        [Test]
        public void OppositeTest() {
            Assert.AreEqual(Direction.Neutral, Direction.Neutral.Opposite());
            Assert.AreEqual(Direction.Backward, Direction.Forward.Opposite());
            Assert.AreEqual(Direction.Forward, Direction.Backward.Opposite());
            Assert.AreEqual(Direction.Down, Direction.Up.Opposite());
            Assert.AreEqual(Direction.Up, Direction.Down.Opposite());
        }

        [Test]
        public void ToVectorTest() {
            Assert.AreEqual(Vector2.zero, Direction.Neutral.ToVector());
            Assert.AreEqual(Vector2.up, Direction.Up.ToVector());
            Assert.AreEqual(Vector2.down, Direction.Down.ToVector());
            Assert.AreEqual(Vector2.right, Direction.Forward.ToVector());
            Assert.AreEqual(Vector2.left, Direction.Backward.ToVector());
        }

    }

}