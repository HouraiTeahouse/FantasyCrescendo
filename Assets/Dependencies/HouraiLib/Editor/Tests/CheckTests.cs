using System;
using NUnit.Framework;

namespace HouraiTeahouse {

    internal class CheckTests {

        [Test]
        public void CheckArgumentNullTest() {
            Assert.Catch<ArgumentNullException>(delegate {
                object obj = null;
                Assert.Null(Argument.NotNull(obj));
            });
            Assert.DoesNotThrow(delegate { Assert.NotNull(Argument.NotNull(new object())); });
        }

        [Test]
        public void CheckArgumentTest() {
            var name = "test";
            Assert.Catch<ArgumentException>(delegate { Argument.Check(name, 3 > 5); });
            Assert.DoesNotThrow(delegate { Argument.Check(name, 5 > 3); });
        }

        [Test]
        public void CheckRangeListTest() {
            var test = new int[30];
            Assert.False(Check.Range(-1, test));
            Assert.True(Check.Range(0, test));
            Assert.True(Check.Range(15, test));
            Assert.False(Check.Range(30, test));
        }

        [Test]
        public void CheckNotEmptyTest() {
            Assert.DoesNotThrow(delegate { Argument.NotEmpty(new[] {0}); });
            Assert.Catch<ArgumentException>(delegate { Argument.NotEmpty<int[]>(null); });
            Assert.Catch<ArgumentException>(delegate { Argument.NotEmpty(new object[] {}); });
            int[] test = {1, 2, 3};
            Assert.AreEqual(test, Argument.NotEmpty(test));
        }

    }

}
