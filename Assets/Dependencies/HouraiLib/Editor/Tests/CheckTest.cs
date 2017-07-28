using System;
using NUnit.Framework;

namespace HouraiTeahouse {

    public class ArgumentTests {

        [Test]
        public void not_null_throws_on_null() {
            Assert.Catch<ArgumentNullException>(delegate {
                object obj = null;
                Assert.Null(Argument.NotNull(obj));
            });
        }

        [Test]
        public void not_null_doesnt_throw_on_non_null() {
            Assert.DoesNotThrow(delegate { Assert.NotNull(Argument.NotNull(new object())); });
        }

        [Test]
        public void check_throws_on_false() {
            Assert.Catch<ArgumentException>(delegate { Argument.Check("test", false); });
        }

        [Test]
        public void check_doesnt_throw_on_true() {
            Assert.DoesNotThrow(delegate { Argument.Check("test", true); });
        }

    }

    public class CheckTest {

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
