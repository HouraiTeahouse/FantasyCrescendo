using System;
using System.Collections;
using UnityEngine;

namespace Crescendo.API {

    public static class DebugUtil {

        public static void Log(object obj)
        {
            var enumerable = obj as IEnumerable;
            var iterator = obj as IEnumerator;
            Debug.Log(obj);
            if (enumerable != null)
            {
                Debug.Log(obj);
                foreach (object contained in enumerable)
                    Debug.Log(contained);
            }
            else if (iterator != null)
            {
                while (iterator.MoveNext())
                {
                    Debug.Log(iterator.Current);
                }
            }
        }

    }

}