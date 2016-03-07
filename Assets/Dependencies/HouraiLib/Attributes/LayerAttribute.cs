using System;
using UnityEngine;

namespace HouraiTeahouse {

    /// <summary>
    /// A PropertyAttribute that exposes a Layer control on the editor
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class LayerAttribute : PropertyAttribute {

    }

}
