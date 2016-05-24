using System;
using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace HouraiTeahouse {

   internal class CollectionTests {

        [Test]
        public void IsNullOrEmptyTest() {
            List<int> test = null;
            Assert.True(test.IsNullOrEmpty());
            Assert.True(new List<int>().IsNullOrEmpty());
            Assert.False(new List<int>(new[] {0}).IsNullOrEmpty());
        }

        [Test]
        public void IsEmptyTest() {
            Assert.True(new List<int>().IsEmpty());
            Assert.False(new List<int>(new [] {0}).IsEmpty());
            Assert.Catch<ArgumentNullException>(delegate {
                List<int> test = null;
                test.IsEmpty();
            });
        }
 
        [Test]
        public void EmptyOrNullTest() {
            List<int> test = null;
            Assert.NotNull(test.EmptyIfNull());
            Assert.AreNotEqual(test, test.EmptyIfNull());
            test = new List<int>();
            Assert.NotNull(test.EmptyIfNull());
            Assert.AreEqual(test, test.EmptyIfNull());  
        }
 
        [Test]
        public void IgnoreNullsTest() {
            object[] test = {new object(), null, null, new object(), null, new object(), new object(), new object()};
            foreach (object obj in test.IgnoreNulls())
                Assert.NotNull(obj);
        }

        [Test]
        public void ArgMaxTest() {
            int[] test = {3, 2, 2, 9, 2, -1, 1, 0, 12, 3, 5, 2};
            Assert.AreEqual(8, test.ArgMax());
            var test2 = new Dictionary<int, int>{{3, 5}, {4, 3}, {7, 1}, {1, 3}};
            Assert.AreEqual(3, test2.ArgMax());
            test = null;
            test2 = null;
            Assert.Catch<ArgumentNullException>(() => test.ArgMax());
            Assert.Catch<ArgumentNullException>(() => test2.ArgMax());
        }

       [Test]
       public void ArgMinTest() {
            int[] test = {3, 2, 2, 9, 2, -1, 1, 0, 12, 3, 5, 2};
            Assert.AreEqual(5, test.ArgMin());
            var test2 = new Dictionary<int, int>{{3, 5}, {4, 3}, {7, 1}, {1, 3}};
            Assert.AreEqual(7, test2.ArgMin());
            test = null;
            test2 = null;
            Assert.Catch<ArgumentNullException>(() => test.ArgMin());
            Assert.Catch<ArgumentNullException>(() => test2.ArgMin());
       }

        [Test]
        public void RandomTest() {
            int[] test = null;
            Assert.Catch<ArgumentNullException>(() => test.Random());
            test = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
            Assert.DoesNotThrow(() => test.Random());
            Assert.DoesNotThrow(() => test.Random(-1, test.Length * 2));
            Random.seed = 42;
            Assert.AreEqual(4, test.Random());
            Assert.AreEqual(3, test.Random());
            Assert.AreEqual(6, test.Random());
            Assert.AreEqual(8, test.Random());
        }
    }
}
