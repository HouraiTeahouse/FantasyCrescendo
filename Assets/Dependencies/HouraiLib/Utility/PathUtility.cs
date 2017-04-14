using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HouraiTeahouse {

    public static class PathUtility {

        public static string Combine(params string[] parts) {
            return Combine(parts as IEnumerable<string>);
        }

        public static string Combine(IEnumerable<string> parts) {
            if (!parts.Any())
                return string.Empty;
            var path = parts.First();
            foreach (var part in parts.Skip(1))
                path = Path.Combine(path, part);
            return path;
        }

    }

}

