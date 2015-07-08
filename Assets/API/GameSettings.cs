using UnityEngine;

public class GameSettings : Singleton<GameSettings> {

    [System.Serializable]
    private class PlayerData {

        public Color Color;
        public Sprite Sprite;

    }

    public static int MaxPlayers {
        get { return Instance._playerData.Length;  }
    }

    [SerializeField]
    private PlayerData[] _playerData;

    public static PlayerIndicator CreatePlayerIndicator(int playerNumber) {
        PlayerIndicator newIndicator = new GameObject("P" + (playerNumber + 1) + " Indicator" ).AddComponent<PlayerIndicator>();
        newIndicator.Color = GetPlayerColor(playerNumber);
        newIndicator.Sprite = (playerNumber > 0 && playerNumber <= MaxPlayers)
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

}
