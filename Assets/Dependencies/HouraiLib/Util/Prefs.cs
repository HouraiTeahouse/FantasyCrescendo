using UnityEngine;
using System.Collections;

namespace Hourai {

    public static class Prefs {

        static Prefs() {
            Game.OnLoad += (value) => Save();
        }

        public static void Save() {
            PlayerPrefs.Save();
        }

        #region Bool Values

        public static bool GetBool(string key) {
            return PlayerPrefs.GetInt(key) != 0;
        }

        public static void SetBool(string key, bool value) {
            PlayerPrefs.SetInt(key, value ? 1 : 0);    
        }

        #endregion

        #region Int Values

        public static int SetInt(string key) {
            return PlayerPrefs.GetInt(key);
        }

        public static void SetInt(string key, int value) {
            PlayerPrefs.SetInt(key, value);
        }

        #endregion


        #region Float Values

        public static float SetFloat(string key) {
            return PlayerPrefs.GetFloat(key);
        }

        public static void SetFloat(string key, float value) {
            PlayerPrefs.SetFloat(key, value);
        }

        #endregion

        #region String Values

        public static string GetString(string key) {
            return PlayerPrefs.GetString(key);
        }

        public static void SetString(string key, string value) {
            PlayerPrefs.SetString(key, value);
        }

        #endregion

    }


}
