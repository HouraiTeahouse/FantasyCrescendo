using HouraiTeahouse.FantasyCrescendo.Matches;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Networking {
		
[CreateAssetMenu(menuName="Game Mode/Networked Game Mode")]
public class NetworkGameMode : DefaultGameMode {

	protected override Match CreateMatch(MatchConfig config) => new NetworkMatch(NetworkManager.Instance.Lobby);

}

}