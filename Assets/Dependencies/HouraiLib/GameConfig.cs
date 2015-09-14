using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Hourai {

    [DefineCategories("Tags", "PlayerPrefs")]
    public abstract class GameConfig : BetterScriptableObject {
        
        [Serialize, Show, Tags, Category("Tags"), Default("HumanPlayer")]
        private string _playerTag;

        public string PlayerTag {
            get { return _playerTag; }
        }

        [Serialize, Show, Tags, Category("Tags"), Default("Respawn")]
        private string _respawnTag;

        public string RespawnTag {
            get { return _respawnTag; }
        }

        [Serialize, Show, Tags, Category("Tags"), Default("GUI")]
        private string _guiTag;

        public string GUITag {
            get { return _guiTag; }
        }

        [Serialize, Show, Category("PlayerPrefs"), Display(Dict.HorizontalPairs)]
        private Dictionary<string, bool> bools;

        [Serialize, Show, Category("PlayerPrefs"), Display(Dict.HorizontalPairs)]
        private Dictionary<string, int> ints;

        [Serialize, Show, Category("PlayerPrefs"), Display(Dict.HorizontalPairs)]
        private Dictionary<string, string> strings;

        [Serialize, Show, Category("PlayerPrefs"), Display(Dict.HorizontalPairs)]
        private Dictionary<string, float> floats;

        protected virtual void OnEnable() {
            FillPref(bools, Prefs.SetBool);
            FillPref(ints, Prefs.SetInt);
            FillPref(strings, Prefs.SetString);
            FillPref(floats, Prefs.SetFloat);
        }

        private void FillPref<T>(Dictionary<string, T> dictionary, Action<string, T> set) {
            foreach (KeyValuePair<string, T> b in dictionary.Where(b => !PlayerPrefs.HasKey(b.Key)))
                set.SafeInvoke(b.Key, b.Value);
        }

    }

}

