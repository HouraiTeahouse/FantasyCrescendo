using NUnit.Framework;
using UnityEngine;

namespace HouraiTeahouse {

    public class VectorTest {

        [Test]
        public void MultTest() {
            Assert.AreEqual(20 * Vector2.up, (5 * Vector2.one).Mult(4 * Vector2.up));
            Assert.AreEqual(Vector3.zero, (5 * Vector3.up).Mult(4 * Vector3.forward));
            Assert.AreEqual(new Vector4(0, 0, 3, 0), new Vector4(1, 3, 2, 5).Mult(new Vector4(0, 0, 1.5f, 0)));
        }

        [Test]
        public void MaxTest() {
            Assert.AreEqual(20, new Vector2(20, 3).Max());
            Assert.AreEqual(20, new Vector3(10, 3, 20).Max());
            Assert.AreEqual(20, new Vector4(0, 5, 20, 3).Max());
        }

        [Test]
        public void MinTest() {
            Assert.AreEqual(3, new Vector2(20, 3).Min());
            Assert.AreEqual(3, new Vector3(10, 3, 20).Min());
            Assert.AreEqual(0, new Vector4(0, 5, 20, 3).Min());
        }

    }

}