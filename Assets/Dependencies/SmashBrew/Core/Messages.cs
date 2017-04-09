using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew {

    public class UpdatePlayerMessage : MessageBase {

        public int ID;
        public int TypeID;
        public uint GameObjectNetId;
        public PlayerSelectionMessage Selection;

        public void UpdatePlayer(Player player) {
            if (player == null)
                return;
            ID = player.ID;
            if (Check.Range(TypeID, PlayerType.Types))
                player.Type = PlayerType.Types[TypeID];
            else
                Log.Error("Attempted to update player {0} to player type {1}, which does not exist", player, TypeID);
            player.Selection = Selection.ToSelection();
            var instanceId = new NetworkInstanceId(GameObjectNetId);
            var objects = ClientScene.objects;
            if(objects.ContainsKey(instanceId)) {
                player.PlayerObject = objects[instanceId].gameObject;
            } else {
                player.PlayerObject = null;
            }
        }

        public static UpdatePlayerMessage FromPlayer(Player player) {
            uint netId = 0;
            if(player.NetworkIdentity != null) {
                netId = player.NetworkIdentity.netId.Value;
            }
            return new UpdatePlayerMessage {
                ID = player.ID,
                TypeID = player.Type.ID,
                Selection = PlayerSelectionMessage.FromSelection(player.Selection),
                GameObjectNetId = netId
            };
        }
    }

    public class PlayerSelectionMessage : MessageBase {
        public uint CharacterID;
        public int Pallete;
        public int CPULevel;

        public static PlayerSelectionMessage FromSelection(PlayerSelection selection) {
            return new PlayerSelectionMessage {
                CharacterID = selection.Character != null ? selection.Character.Id : 0,
                Pallete = selection.Pallete,
                CPULevel = selection.CPULevel,
            };
        }

        public PlayerSelection ToSelection() {
            return new PlayerSelection {
                Character = DataManager.GetCharacter(CharacterID),
                CPULevel = CPULevel,
                Pallete = Pallete
            };
        }
    }

}
