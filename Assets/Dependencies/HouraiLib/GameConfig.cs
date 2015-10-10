using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Hourai {
    
    public abstract class GameConfig : ScriptableObject {
        
        [SerializeField]
        private string _playerTag;

        public string PlayerTag {
            get { return _playerTag; }
        }

        [SerializeField]
        private string _respawnTag;

        public string RespawnTag {
            get { return _respawnTag; }
        }

        [SerializeField]
        private string _guiTag;

        public string GUITag {
            get { return _guiTag; }
        }

        //[SerializeField]
        //private Dictionary<string, bool> bools;

        //[SerializeField]
        //private Dictionary<string, int> ints;

        //[SerializeField]
        //private Dictionary<string, string> strings;

        //[SerializeField]
        //private Dictionary<string, float> floats;

        //protected virtual void OnEnable() {
        //    FillPref(bools, Prefs.SetBool);
        //    FillPref(ints, Prefs.SetInt);
        //    FillPref(strings, Prefs.SetString);
        //    FillPref(floats, Prefs.SetFloat);
        //}

        //private void FillPref<T>(Dictionary<string, T> dictionary, Action<string, T> set) {
        //    if (set == null)
        //        return;
        //    foreach (KeyValuePair<string, T> b in dictionary.Where(b => !PlayerPrefs.HasKey(b.Key)))
        //        set(b.Key, b.Value);
        //}

    }

}

