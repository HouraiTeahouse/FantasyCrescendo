using UnityEngine;

namespace Crescendo.API
{
    
    public class BGMGroup : ScriptableObject
    {
        [System.Serializable]
        private class BGMData
        {
            private const string delimiter = "/";
            private const string suffix = "weight";

            [SerializeField, ResourcePath(typeof(AudioClip))]
            private string _bgm;

            private Resource<AudioClip> _bgmResource;
            public Resource<AudioClip> BGM
            {
                get { return _bgmResource; }
            }

            [SerializeField, Range(0f, 1f)]
            private float _baseWeight = 1f;

            private string playerPrefsKey;
            private float _weight;

            public float Weight
            {
                get { return _weight; }
            }

            public void Initialize(string stageName)
            {
                _bgmResource = new Resource<AudioClip>(_bgm);
                playerPrefsKey = stageName + delimiter + _bgm + "_" + suffix;
                Debug.Log(playerPrefsKey);

                if (PlayerPrefs.HasKey(playerPrefsKey))
                    _weight = PlayerPrefs.GetFloat(playerPrefsKey);
                else
                {
                    PlayerPrefs.SetFloat(playerPrefsKey, _baseWeight);
                    _weight = _baseWeight;
                }
            }

            public override string ToString()
            {
                return _bgm + " - (" + _baseWeight + ")";
            }

        }

        [SerializeField, Tooltip("The name of the BGM group")]
        private string _name;
        public string Name
        {
            get { return _name; }
        }

        [SerializeField]
        private BGMData[] backgroundMusicData;
        private WeightedRNG<Resource<AudioClip>> selection;

        void OnEnable()
        {
            selection = new WeightedRNG<Resource<AudioClip>>();
            foreach (var bgmData in backgroundMusicData)
            {
                bgmData.Initialize(Name);
                selection[bgmData.BGM] = bgmData.Weight;
            }
        }

        public void PlayRandom()
        {
            BGM.Play(selection.Select().Load());
            Resources.UnloadUnusedAssets();
        }
     
    }

}
