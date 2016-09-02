using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.Editor {

    public class BasePropertyDrawer<T> : PropertyDrawer where T : PropertyAttribute {

        public new T attribute {
            get { return base.attribute as T; }
        }

    }

}