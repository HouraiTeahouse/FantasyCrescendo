using System;
using System.Collections.Generic;
using System.Linq;

namespace HouraiTeahouse {

    public static class UriUtility {

        public static string Combine(params string[] parts) {
            return Combine(parts as IEnumerable<string>);
        }

        public static string Combine(IEnumerable<string> parts) {
            if (!parts.Any())
                return string.Empty;
            var uri = new Uri(parts.First() + "/");
            foreach (var part in parts.Skip(1))
                uri = new Uri(uri,  part + "/");
            var path = uri.ToString();
            return path.Substring(0, path.Length - 1);
        }

    }

}

