using UnityEngine;

public class GameSettings : ScriptableObject {

    [SerializeField]
    private Color[] playerColors;

    [SerializeField]
    private Sprite[] playerIndicatorSprites;

    private static GameSettings _instance;
    private static GameSettings Instance {
        get {
            if (_instance == null) {
                _instance = Resources.Load<GameSettings>("GameSettings");
                if(_instance == null)
                    Debug.LogError("GameSettings is not found. Please create new GameSettings in the Resources folder.");
            }
            return _instance;
        }
    }

    public static PlayerIndicator CreatePlayerIndicator(int playerNumber) {
        PlayerIndicator newIndicator = new GameObject("P" + (playerNumber + 1) + " Indicator" ).AddComponent<PlayerIndicator>();
        newIndicator.Color = GetPlayerColor(playerNumber);
        Sprite[] sprites = Instance.playerIndicatorSprites;
        newIndicator.Sprite = (playerNumber >= 0 && playerNumber < sprites.Length)
                                  ? sprites[playerNumber]
                                  : sprites[sprites.Length - 1];
        return newIndicator;
    }

    public static Color GetPlayerColor(int playerNumber) {
        GameSettings instance = Instance;
        if (instance == null)
            return Color.white;
        Color[] colors = instance.playerColors;
        if (playerNumber >= colors.Length)
            return playerNumber <= 0 ? Color.white : colors[colors.Length - 1];
        return colors[playerNumber];
    }

}
