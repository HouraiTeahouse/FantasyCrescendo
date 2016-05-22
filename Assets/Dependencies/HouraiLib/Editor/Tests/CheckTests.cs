using System;
using NUnit.Framework;

namespace HouraiTeahouse.Tests {

    public class CheckTests {

        [Test]
        public void CheckArgumentNullTest() {
            Assert.Catch<ArgumentNullException>(delegate {
                object obj = null;
                Check.ArgumentNull("test", obj);
            });
        }

        [Test]
        public void CheckArgumentTest() {
            Assert.Catch<ArgumentException>(delegate {
                Check.Argument("test", 3 > 5);
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
    }
}
