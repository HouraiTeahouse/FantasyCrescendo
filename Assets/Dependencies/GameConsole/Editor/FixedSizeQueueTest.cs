using System;
using NUnit.Framework;

namespace HouraiTeahouse.Console {

    internal class FixedSizeQueueTest {

        readonly int[] testArray = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};

        public void CheckEqual(FixedSizeQueue<int> queue, int i) {
            foreach (int a in queue) {
                Assert.AreEqual(testArray[i], a);
                i++;
            }
        }

        [Test]
        public void EnqueueTest() {
            var queue3 = new FixedSizeQueue<int>(testArray.Length * 2, testArray);
            for (var i = 0; i < testArray.Length * 2; i++) {
                if (i >= testArray.Length)
                    Assert.AreEqual(testArray.Length * 2, queue3.Count);
                else
                    Assert.AreEqual(testArray.Length + i, queue3.Count);
                queue3.Enqueue(0);
            }
        }

        [Test]
        public void DequeueTest() {
            var queue = new FixedSizeQueue<int>();
            Assert.Catch<InvalidOperationException>(() => queue.Dequeue());
            int size = testArray.Length;
            var queue2 = new FixedSizeQueue<int>(size, testArray);
            foreach (int i in testArray) {
                size--;
                Assert.AreEqual(i, queue2.Dequeue());
                Assert.AreEqual(size, queue2.Count);
            }
        }

        [Test]
        public void PeekTest() {
            var queue = new FixedSizeQueue<int>(testArray.Length, testArray);
            int size = queue.Count;
            int limit = queue.Limit;
            Assert.AreEqual(queue.Peek(), testArray[0]);
            Assert.AreEqual(size, queue.Count);
            Assert.AreEqual(limit, queue.Limit);
        }

        [Test]
        public void ConstructorTest() {
            var queue1 = new FixedSizeQueue<int>();
            Assert.AreEqual(queue1.Limit, FixedSizeQueue<int>.DefaultSize);
            Assert.AreEqual(queue1.Count, 0);
            const int size = 20;
            var queue2 = new FixedSizeQueue<int>(size);
            Assert.AreEqual(size, queue2.Limit);
            Assert.AreEqual(0, queue2.Count);
            var queue3 = new FixedSizeQueue<int>(testArray.Length, testArray);
            Assert.AreEqual(testArray.Length, queue3.Limit);
            Assert.AreEqual(testArray.Length, queue3.Count);
            CheckEqual(queue3, 0);
            int half = testArray.Length / 2;
            var queue4 = new FixedSizeQueue<int>(half, testArray);
            Assert.AreEqual(half, queue4.Limit);
            Assert.AreEqual(half, queue4.Count);
            CheckEqual(queue4, half);
        }

        [Test]
        public void LimitTest() {
            const int limit = 5;
            const int newLimit = 10;
            var queue = new FixedSizeQueue<int>(10, testArray) {Limit = limit};
            Assert.AreEqual(queue.Limit, limit);
            Assert.AreEqual(queue.Count, limit);
            queue.Limit = newLimit;
            Assert.AreEqual(queue.Limit, newLimit);
            Assert.AreEqual(queue.Count, limit);
        }

        [Test]
        public void ClearTest() {
            var queue = new FixedSizeQueue<int>(10, testArray);
            queue.Clear();
            Assert.AreEqual(queue.Limit, 10);
            Assert.AreEqual(queue.Count, 0);
            if (!queue.IsNullOrEmpty())
                Assert.Fail();
        }

    }

}