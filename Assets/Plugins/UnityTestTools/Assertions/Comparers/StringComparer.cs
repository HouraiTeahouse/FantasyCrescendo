using System;

namespace UnityTest {

    public class StringComparer : ComparerBaseGeneric<string> {

        public enum CompareType {

            Equal,
            NotEqual,
            Shorter,
            Longer

        }

        public CompareType compareType;
        public StringComparison comparisonType = StringComparison.Ordinal;
        public bool ignoreCase = false;

        protected override bool Compare(string a, string b) {
            if (ignoreCase) {
                a = a.ToLower();
                b = b.ToLower();
            }
            switch (compareType) {
                case CompareType.Equal:
                    return string.Compare(a, b, comparisonType) == 0;
                case CompareType.NotEqual:
                    return string.Compare(a, b, comparisonType) != 0;
                case CompareType.Longer:
                    return string.Compare(a, b, comparisonType) > 0;
                case CompareType.Shorter:
                    return string.Compare(a, b, comparisonType) < 0;
            }
            throw new Exception();
        }

    }

}