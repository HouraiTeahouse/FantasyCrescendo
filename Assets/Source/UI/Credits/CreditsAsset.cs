using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public ReadOnlyCollection<Category> Categories {
            get { return new ReadOnlyCollection<Category>(_categories); }
        }

        [SerializeField]
        Category[] _categories;

    }


}
