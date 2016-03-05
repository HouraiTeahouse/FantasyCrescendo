using System;
using UnityEngine;

namespace HouraiTeahouse {

    /// <summary>
    /// A propperty attribute that provides a Tag selector for strings.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class TagAttribute : PropertyAttribute {
    }

}
