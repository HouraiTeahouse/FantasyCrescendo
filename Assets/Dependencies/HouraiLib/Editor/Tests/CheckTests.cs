using System;
using NUnit.Framework;

namespace HouraiTeahouse {

    internal class CheckTests {

        [Test]
        public void CheckArgumentNullTest() {
            var name = "test";
            Assert.Catch<ArgumentNullException>(delegate {
                object obj = null;
                Assert.Null(Check.NotNull(name, obj));
            });
            Assert.DoesNotThrow(delegate {
                Assert.NotNull(Check.NotNull(name, new object()));
            });
        }

        [Test]
        public void CheckArgumentTest() {
            var name = "test";
            Assert.Catch<ArgumentException>(delegate {
                Check.Argument(name, 3 > 5);
            });
            Assert.DoesNotThrow(delegate {
                Check.Argument(name, 5 > 3);
            });
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
            Assert.DoesNotThrow(delegate {
                Check.NotEmpty(new[] {0});
            });
            Assert.Catch<InvalidOperationException>(delegate {
                Check.NotEmpty<int[]>(null);
            });
            Assert.Catch<InvalidOperationException>(delegate {
                Check.NotEmpty(new object[] {});
            });
            int[] test = {1, 2, 3};
            Assert.AreEqual(test, Check.NotEmpty(test));
        }
    }
}
