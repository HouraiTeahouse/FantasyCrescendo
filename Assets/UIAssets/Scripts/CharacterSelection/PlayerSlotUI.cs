using UnityEngine;
using UnityEngine.UI;

public class PlayerSlotUI : MonoBehaviour {

    public Text levelText = null;
    private Mediator mediator;
    public GameObject playerImage = null;
    public GameObject playerModeBtn = null;
    public int playerNumber;
    public PlayerOptions.PlayerType type = PlayerOptions.PlayerType.PLAYER;

    private void Start() {
        DataManager dm = DataManager.getDataManagerInstance();

        if (dm == null) {
            Debug.LogError("Unable to find the data manager object.");
            Destroy(gameObject);
        }

        mediator = dm.mediator;

        if (mediator == null || levelText == null || playerModeBtn == null || playerImage == null) {
            Debug.LogError("Fill all gameObjects needed by this object.");
            Destroy(gameObject);
        }

        mediator.Subscribe<DataCommands.ChangePlayerLevelCommand>(onChangePlayerLevel);
        mediator.Subscribe<DataCommands.ChangePlayerMode>(onChangePlayerMode);
    }

    public void updateUIMode(PlayerOptions.PlayerType pt) {
        GameObject levelTextParent = levelText.transform.parent.gameObject;
        var buttonText = playerModeBtn.GetComponentInChildren<Text>();
        if (buttonText == null) {
            Debug.LogError("Unable to get player slot button text.");
            return;
        }

        type = pt;
        if (type == PlayerOptions.PlayerType.CPU) {
            buttonText.text = "CPU";
            levelTextParent.SetActive(true);
        } else if (type == PlayerOptions.PlayerType.DISABLED) {
            buttonText.text = "NONE";
            levelTextParent.SetActive(false);
        } else if (type == PlayerOptions.PlayerType.PLAYER) {
            buttonText.text = "PLAYER " + (playerNumber + 1);
            levelTextParent.SetActive(true);
        } else
            Debug.LogError("Invalid player type in player slot.");
    }

    public void setPlayerNumber(int n) {
        playerNumber = n;
    }

    public void changePlayerMode() {
        mediator.Publish(
                         new DataCommands.ChangePlayerMode {playerNum = playerNumber});
    }

    public void changeLevel(int level) {
        mediator.Publish(
                         new DataCommands.ChangePlayerLevelCommand {newLevel = level, playerNum = playerNumber});
    }

    public void onChangePlayerLevel(DataCommands.ChangePlayerLevelCommand cmd) {
        if (cmd.playerNum != playerNumber)
            return;
        levelText.text = "lv " + cmd.newLevel;
    }

    public void onChangePlayerMode(DataCommands.ChangePlayerMode cmd) {
        if (cmd.playerNum != playerNumber)
            return;
        updateUIMode(PlayerOptions.getNextType(type));
    }

}