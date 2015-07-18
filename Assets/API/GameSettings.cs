using Genso.API;
using UnityEngine;

public class GameSettings : Singleton<GameSettings> {

    #pragma warning disable 0649
    [System.Serializable]
    private class PlayerData {

        public Color Color;
        public Sprite Sprite;

    }

    [System.Serializable]
    private class DebugData {

        public Color OffensiveHitboxColor = Color.red;
        public Color DamageableHitboxColor = Color.yellow;
        public Color InvincibleHitboxColor = Color.green;
        public Color IntangiblHitboxColor = Color.blue;

    }
    #pragma warning restore 0649

    public static int MaxPlayers {
        get { return Instance._playerData.Length;  }
    }

    public static LayerMask HurtboxLayers
    {
        get { return Instance._hurtboxLayers; }
    }

    [SerializeField]
    private LayerMask _hurtboxLayers;

    [SerializeField]
    private PlayerData[] _playerData;

    [SerializeField]
    private DebugData _debug;

    public static Color GetHitboxColor(HitboxType type) {
        DebugData debugData = Instance._debug;
        switch (type) {
            case HitboxType.Offensive:
                return debugData.OffensiveHitboxColor;
            case HitboxType.Damageable:
                return debugData.DamageableHitboxColor;
            case HitboxType.Invincible:
                return debugData.IntangiblHitboxColor;
            case HitboxType.Intangible:
                return debugData.InvincibleHitboxColor;
            default:
                return Color.white;
        }
    }
     
    public static PlayerIndicator CreatePlayerIndicator(int playerNumber) {
        PlayerIndicator newIndicator = new GameObject("P" + (playerNumber + 1) + " Indicator" ).AddComponent<PlayerIndicator>();
        newIndicator.Color = GetPlayerColor(playerNumber);
        newIndicator.Sprite = (playerNumber >= 0 && playerNumber <= MaxPlayers)
                                  ? Instance._playerData[playerNumber].Sprite
                                  : null;
        return newIndicator;
    }

    public static Color GetPlayerColor(int playerNumber) {
        GameSettings instance = Instance;
        if (instance == null)
            return Color.white;
         return playerNumber < 0 || playerNumber >= MaxPlayers ? Color.white : instance._playerData[playerNumber].Color;
    }

    void OnLevelWasLoaded(int level) {
        GameObject[] staticManagers = GameObject.FindGameObjectsWithTag("Static Managers");
        foreach (GameObject tagged in staticManagers) {
            if(tagged != gameObject)
                Destroy(tagged);
        }


    }

}
