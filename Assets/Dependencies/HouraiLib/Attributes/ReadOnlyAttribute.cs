using System;
using UnityEngine;

namespace HouraiTeahouse {
    /// <summary>
    /// PropertyAttribute to mark serializable fields as read-only.
    /// They will appear in the editor as a simple label.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyAttribute : PropertyAttribute {
    }
}