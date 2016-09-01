// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HouraiTeahouse {

    /// <summary> A PropertyAttribute for the Unity Editor. Marks a string field to store a path to an asset stored in a
    /// Resources folder. The resultant string can be used with Resources.Load to get said asset. The Unity Editor UI shows a
    /// object field instead of a string field. </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ResourceAttribute : PropertyAttribute {

        static readonly Type ObjectType = typeof(Object);

        /// <summary> Initializes a new instance of ResourceAttribute. </summary>
        /// <param name="type"> Optional type restriction on the type of Resource object to use. No restriction is applied if null
        /// or not derived from UnityEngine.Object </param>
        public ResourceAttribute(Type type = null) {
            TypeRestriction = ObjectType;
            if (type == null)
                return;
            if (ObjectType.IsAssignableFrom(type))
                TypeRestriction = type;
            else
                Log.Warning(
                    "Trying to get a resource type restriction on type: {0} is impossible. Use a type derived from UnityEngine.Object.",
                    type.FullName);
        }

        /// <summary> The type of asset to be stored. All instances of this type, including those of derived types, can be used. </summary>
        public Type TypeRestriction { get; private set; }

    }

}
