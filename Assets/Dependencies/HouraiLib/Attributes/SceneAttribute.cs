using System;
using UnityEngine;

namespace HouraiTeahouse {
    /// <summary>
    /// PropertyAttribute with a drawer that exposes a SceneAsset object field.
    /// MUST be a string field.
    /// Saves the path of the SceneAsset to the field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SceneAttribute : PropertyAttribute {
    }
}