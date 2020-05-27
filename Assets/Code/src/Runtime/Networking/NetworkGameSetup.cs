using System;
using System.Linq;
using UnityEngine;
using HouraiTeahouse.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class NetworkGameSetup : IDisposable {

    // Lobby Metadata Keys 
    const string kStageKey = "stage";
    const string kStocksKey = "stocks";
    const string kTimeKey = "time";
    
    // Member metadata keys
    const string kReadyKey = "ready";
    const string kConfigKey = "config";

    const uint kDefaultStocks = 3;

    /// <summary>
    /// The underlying lobby that is being setup.
    /// </summary>
    public Lobby Lobby { get; }
    
    /// <summary>
    /// The current match config for the lobby.
    /// </summary>
    public MatchConfig Config { get; private set; }

    /// <summary>
    /// Fires when the match config is updated by a local or remote player.
    /// </summary>
    public event Action<MatchConfig> OnConfigUpdated;

    /// <summary>
    /// Checks if the match can be started.
    /// Returns true if the config is valid and everyone is ready.
    /// </summary>
    public bool CanStart => Config.IsValid && IsEveryoneReady;

    /// <summary>
    /// Checks if everyone in the lobby is ready.
    /// </summary>
    /// <returns></returns>
    public bool IsEveryoneReady => Lobby.Members.All(IsReady);

    /// <summary>
    /// Sets up a lobby based on an initial match config.
    /// This should be called by the lobby owner upon creation of the lobby.
    /// </summary>
    /// <param name="lobby">the lobby to use</param>
    /// <param name="config">the initial configuration </param>
    /// <returns>The setup object.</returns>
    public static NetworkGameSetup InitializeLobby(Lobby lobby, MatchConfig config) {
        lobby.SetMetadata(kStageKey, config.StageID.ToString());
        lobby.SetMetadata(kStocksKey, config.Stocks.ToString());
        lobby.SetMetadata(kTimeKey, config.Time.ToString());
        return new NetworkGameSetup(lobby);
    }

    public NetworkGameSetup(Lobby lobby) {
        Lobby = lobby;

        Lobby.OnUpdated += CheckForConfigChange;
        Lobby.OnMemberJoin += OnMemberChange;
        Lobby.OnMemberLeave += OnMemberChange;
        Lobby.OnMemberUpdated += OnMemberChange;

        Config = GenerateConfig();
    }

    public void Dispose() {
        Lobby.OnUpdated -= CheckForConfigChange;
        Lobby.OnMemberJoin -= OnMemberChange;
        Lobby.OnMemberLeave -= OnMemberChange;
        Lobby.OnMemberUpdated -= OnMemberChange;
    }

    /// <summary>
    /// Updates the local player's config within the lobby.
    /// </summary>
    /// <param name="config">the player's config</param>
    public unsafe void UpdatePlayerConfig(PlayerConfig config) {
        Span<byte> buffer = stackalloc byte[256];
        var serializer = Serializer.Create(buffer);
        config.Serialize(ref serializer);
        var base64config = serializer.ToBase64String();
        Debug.Log(base64config);
        Debug.Log(base64config.Length);
        Lobby.Members.Me.SetMetadata(kConfigKey, base64config);
        CheckForConfigChange();
    }

    /// <summary>
    /// Sets whether ther local player is ready to start or not.
    /// </summary>
    /// <param name="ready">true if the local player is ready to start, /// false otherwise.</param>
    public void SetReady(bool ready) {
        Lobby.Members.Me.SetMetadata(kReadyKey, ready ? kReadyKey : String.Empty);
    }

    public bool IsReady(LobbyMember member) =>
        !String.IsNullOrEmpty(member.GetMetadata(kReadyKey));

    void OnMemberChange(LobbyMember _) => CheckForConfigChange();
    void CheckForConfigChange() {
        var config = GenerateConfig();
        if (config.Equals(Config)) return;
        OnConfigUpdated?.Invoke(config);
        Config = config;
    }

    MatchConfig GenerateConfig() {
        var config = new MatchConfig {
            StageID = Lobby.GetStageID(),
            Stocks = Lobby.GetStocks(),
            Time = Lobby.GetTime()
        };
        var members = Lobby.Members.OrderBy(m => m.Id);
        var playerCount = 0;
        foreach (var member in members) {
            var base64config = member.GetMetadata(kConfigKey);
            Debug.Log(base64config);
            Debug.Log(base64config.Length);
            try {
                config[playerCount] = Deserializer.FromBase64String<PlayerConfig>(base64config);
            } catch {
                config[playerCount] = new PlayerConfig();
            }
            config[playerCount].LocalPlayerID = (sbyte)((member == Lobby.Members.Me) ? 0 : -1);
            playerCount++;
        }
        config.PlayerCount = (uint)playerCount;
        return config;
    }

}

}
