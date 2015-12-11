using UnityEngine;

namespace Hourai {

    public static class Prefs {

        public static void Save() {
            PlayerPrefs.Save();
        }

        public static bool HasKey(string key) {
            return PlayerPrefs.HasKey(key);
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

        public static int GetInt(string key) {
            return PlayerPrefs.GetInt(key);
        }

        public static void SetInt(string key, int value) {
            PlayerPrefs.SetInt(key, value);
        }

        #endregion


        #region Float Values

        public static float GetFloat(string key) {
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
