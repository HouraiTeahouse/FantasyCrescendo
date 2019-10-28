using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using HouraiTeahouse.Networking;

public class LobbyTest : MonoBehaviour {

    public IntegrationManager manager;
    public uint capacity;
    public uint stage;
    public uint stocks;
    public uint time;
    public uint seed;

    void Start() {
        foreach (var integration in manager.Integrations) {
            CreateLobbyTest(integration);
        }
    }

    async void CreateLobbyTest(IIntegrationClient client) {
        Lobby lobby = null;
        try {
            var createParams = new LobbyCreateParams {
                Capacity = capacity,
                Type = LobbyType.Public,
                Metadata = new Dictionary<string, object> {
                    {"stage", stage},
                    {"stocks", stocks},
                    {"time", time},
                    {"seed", seed},
                }
            };
            lobby = await client.LobbyManager.CreateLobby(createParams);
            // await Task.Delay(2);
            Debug.Log($"{lobby.GetType()}:\nId: {lobby.Id}\nMembers:\n{GetMembers(lobby)}\nCapacity: {lobby.Capacity}:\nMetadata:\n{GetMetadata(lobby)}");
        } catch (Exception e) {
            Debug.LogException(e);
        } finally {
            if (lobby != null) {
                lobby.Delete();
            }
        }
    }

    string GetMembers(Lobby lobby) {
        return string.Join("\n", lobby.Members.Select(m => m.ToString()));
    }

    string GetMetadata(Lobby lobby) {
        var metadata = lobby.GetAllMetadata();
        return string.Join("\n", metadata.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
    }

}
