using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse {

    [CreateAssetMenu(menuName = "Fantasy Crescendo/Credits Asset")]
    public class CreditsAsset : ScriptableObject {

        [Serializable]
        public class Category {
            public string Name;
            public string[] Contributors;

            public override string ToString() {
                if (Contributors != null && Contributors.Length == 1)
                    return Name + ": " + Contributors[0];
                return Name;
            }
        }

        [SerializeField]
        Category[] _categories;

    }


}
