using System;
using NUnit.Framework;

namespace HouraiTeahouse {

    internal class FixedSizeQueueTest {

        readonly int[] testArray = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};

        public void CheckEqual(FixedSizeQueue<int> queue, int i) {
            foreach (int a in queue) {
                Assert.AreEqual(testArray[i], a);
                i++;
            }
        }

        [Test]
        public void constructor_positive_size_doesnt_throw() {
            Assert.DoesNotThrow(() => new FixedSizeQueue<int>(100));
            Assert.DoesNotThrow(() => new FixedSizeQueue<int>(0));
        }

        [Test]
        public void constructor_negative_size_throws() {
            Assert.Catch<ArgumentException>(() => new FixedSizeQueue<int>(-1));
        }

        [Test]
        public void constructor_truncates_if_larger() {
            int half = testArray.Length / 2;
            var queue = new FixedSizeQueue<int>(half, testArray);
            Assert.AreEqual(half, queue.Limit);
            Assert.AreEqual(half, queue.Count);
            Assert.Less(queue.Count, testArray.Length);
        }

        [Test]
        public void constructor_matches_if_size_is_larger() {
            var queue = new FixedSizeQueue<int>(testArray.Length * 2, testArray);
            Assert.AreEqual(testArray.Length, queue.Count);
            Assert.Less(queue.Count, queue.Limit);
        }

        [Test]
        public void enqueue() {
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
        public void constructor_parameterless_defaults() {
            var queue = new FixedSizeQueue<int>();
            Assert.AreEqual(queue.Limit, FixedSizeQueue<int>.DefaultSize);
            Assert.AreEqual(queue.Count, 0);
        }

        [Test]
        public void constructor_expected_value() {
            var queue = new FixedSizeQueue<int>(testArray.Length, testArray);
            CheckEqual(queue, 0);
        }

        [Test]
        public void constructor_limit_set() {
            const int size = 20;
            var queue2 = new FixedSizeQueue<int>(size);
            Assert.AreEqual(size, queue2.Limit);
            Assert.AreEqual(0, queue2.Count);
            var queue3 = new FixedSizeQueue<int>(testArray.Length, testArray);
            Assert.AreEqual(testArray.Length, queue3.Limit);
            Assert.AreEqual(testArray.Length, queue3.Count);
            CheckEqual(queue3, 0);
        }

        [Test]
        public void dequeue_throw_on_empty() {
            var queue = new FixedSizeQueue<int>();
            Assert.Catch<InvalidOperationException>(() => queue.Dequeue());
        }

        [Test]
        public void dequeue_expected_value() {
            int size = testArray.Length;
            var queue = new FixedSizeQueue<int>(size, testArray);
            foreach (int i in testArray) {
                size--;
                Assert.AreEqual(i, queue.Dequeue());
            }
        }

        [Test]
        public void dequeue_size_decrease() {
            int size = testArray.Length;
            var queue = new FixedSizeQueue<int>(size, testArray);
            foreach (int i in testArray) {
                size--;
                Assert.AreEqual(i, queue.Dequeue());
                Assert.AreEqual(size, queue.Count);
            }
        }

        [Test]
        public void peek_expected_value() {
            var queue = new FixedSizeQueue<int>(testArray.Length, testArray);
            Assert.AreEqual(queue.Peek(), testArray[0]);
        }

        [Test]
        public void peek_doesnt_change_limit() {
            var queue = new FixedSizeQueue<int>(testArray.Length, testArray);
            int limit = queue.Limit;
            queue.Peek();
            Assert.AreEqual(limit, queue.Limit);
        }

        [Test]
        public void peek_doesnt_change_size() {
            var queue = new FixedSizeQueue<int>(testArray.Length, testArray);
            int size = queue.Count;
            queue.Peek();
            Assert.AreEqual(size, queue.Count);
        }

        [Test]
        public void limit_without_truncating() {
            const int limit = 5;
            const int newLimit = 10;
            var queue = new FixedSizeQueue<int>(10, testArray) {Limit = limit};
            queue.Limit = newLimit;
            Assert.AreEqual(queue.Limit, newLimit);
            Assert.AreEqual(queue.Count, limit);
        }

        [Test]
        public void limit_trucates() {
            const int limit = 10;
            const int newLimit = 5;
            var queue = new FixedSizeQueue<int>(10, testArray) {Limit = limit};
            queue.Limit = newLimit;
            Assert.AreEqual(queue.Limit, newLimit);
            Assert.AreEqual(queue.Count, newLimit);
        }

        [Test]
        public void clear() {
            var queue = new FixedSizeQueue<int>(10, testArray);
            queue.Clear();
            Assert.AreEqual(queue.Limit, 10);
            Assert.AreEqual(queue.Count, 0);
        }

    }

}
