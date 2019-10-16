using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

////REVIEW: should we automatically pool/retain up to maxPlayerCount player instances?

////TODO: add support for reacting to players missing devices

namespace HouraiTeahouse.FantasyCrescendo {

public class LocalPlayerJoined {
    public LocalPlayer Player;
}

public class LocalPlayerLeft {
    public LocalPlayer Player;
}

public class InputManager : MonoBehaviour {

    /// <summary>
    /// The current number of active players.
    /// </summary>
    /// <remarks>
    /// This count corresponds to all <see cref="PlayerInput"/> instances that are currently enabled.
    /// </remarks>
    public int PlayerCount => LocalPlayer.ActivePlayerCount;

    /// <summary>
    /// Maximum number of players allowed concurrently in the game.
    /// </summary>
    /// <remarks>
    /// If this limit is reached, joining is turned off automatically.
    ///
    /// By default this is set to -1. Any negative value deactivates the player limit and allows
    /// arbitrary many players to join.
    /// </remarks>
    public const int MaxPlayerCount = MatchInput.kMaxSupportedPlayers;

    /// <summary>
    /// Whether new players can currently join.
    /// </summary>
    /// <remarks>
    /// While this is true, new players can join via the mechanism determined by <see cref="JoinBehavior"/>.
    /// </remarks>
    /// <seealso cref="EnableJoining"/>
    /// <seealso cref="DisableJoining"/>
    public bool JoiningEnabled => _allowJoining;

    /// <summary>
    /// Determines the mechanism by which players can join when joining is enabled (<see cref="JoiningEnabled"/>).
    /// </summary>
    /// <remarks>
    /// </remarks>
    public PlayerJoinBehavior JoinBehavior {
        get => _joinBehavior;
        set {
            if (_joinBehavior == value)
                return;
            var joiningEnabled = _allowJoining;
            if (joiningEnabled)
                DisableJoining();
            _joinBehavior = value;
            if (joiningEnabled)
                EnableJoining();
        }
    }

    public PlayerNotifications notificationBehavior {
        get => NotificationBehavior;
        set => NotificationBehavior = value;
    }

    public InputActionAsset Actions => _actions;

    public static InputManager Instance { get; private set; }

    public void EnableJoining() {
        if (!_unpairedDeviceUsedDelegateHooked) {
            if (_unpairedDeviceUsedDelegate == null)
                _unpairedDeviceUsedDelegate = OnUnpairedDeviceUsed;
            InputUser.onUnpairedDeviceUsed += _unpairedDeviceUsedDelegate;
            _unpairedDeviceUsedDelegateHooked = true;
            ++InputUser.listenForUnpairedDeviceActivity;
        }

        _allowJoining = true;
    }

    public void DisableJoining() {
        if (_unpairedDeviceUsedDelegateHooked) {
            InputUser.onUnpairedDeviceUsed -= _unpairedDeviceUsedDelegate;
            _unpairedDeviceUsedDelegateHooked = false;
            --InputUser.listenForUnpairedDeviceActivity;
        }

        _allowJoining = false;
    }

    /// <summary>
    /// Join a new player based on input received through an <see cref="InputAction"/>.
    /// </summary>
    /// <param name="context"></param>
    /// <remarks>
    /// </remarks>
    public void JoinPlayerFromAction(InputAction.CallbackContext context) {
        if (!CheckIfPlayerCanJoin()) return;
        var device = context.control.device;
        JoinPlayer(pairWithDevice: device);
    }

    public void JoinPlayerFromActionIfNotAlreadyJoined(InputAction.CallbackContext context) {
        if (!CheckIfPlayerCanJoin()) return;

        var device = context.control.device;
        if (LocalPlayer.FindFirstPairedToDevice(device) != null)
            return;

        JoinPlayer(pairWithDevice: device);
    }

    public void JoinPlayer(int playerIndex = -1, string controlScheme = null, InputDevice pairWithDevice = null) {
        if (!CheckIfPlayerCanJoin(playerIndex))
            return;

        LocalPlayer.Create(
            playerIndex: playerIndex, 
            controlScheme: controlScheme, 
            pairWithDevice: pairWithDevice);
    }

    public void JoinPlayer(int playerIndex = -1, string controlScheme = null, params InputDevice[] pairWithDevices)
    {
        if (!CheckIfPlayerCanJoin(playerIndex))
            return;

        LocalPlayer.Create(
            playerIndex: playerIndex, 
            controlScheme: controlScheme, 
            pairWithDevices: pairWithDevices);
    }

    [SerializeField] internal PlayerNotifications NotificationBehavior;
    [SerializeField] internal bool _allowJoining = true;
    [SerializeField] internal PlayerJoinBehavior _joinBehavior;
    [Tooltip("Input actions associated with the player.")]
    [SerializeField] internal InputActionAsset _actions;
    [Tooltip("UI InputModule that should have it's input actions synchronized to this PlayerInput's actions.")]
    [SerializeField] internal InputSystemUIInputModule _UIInputModule;

    [NonSerialized] bool _unpairedDeviceUsedDelegateHooked;
    [NonSerialized] Action<InputControl, InputEventPtr> _unpairedDeviceUsedDelegate;

    bool CheckIfPlayerCanJoin(int playerIndex = -1) {
        if (PlayerCount >= MaxPlayerCount) {
            Debug.LogError("Have reached maximum player count of " + MaxPlayerCount, this);
            return false;
        }

        // If we have a player index, make sure it's unique.
        if (playerIndex != -1) {
            for (var i = 0; i < PlayerCount; ++i) {
                if (LocalPlayer.All[i].PlayerIndex == playerIndex) {
                    Debug.LogError(
                        $"Player index #{playerIndex} is already taken by player {LocalPlayer.All[i]}",
                        this);
                    return false;
                }
            }
        }

        return true;
    }

    void OnUnpairedDeviceUsed(InputControl control, InputEventPtr eventPtr) {
        if (!_allowJoining)
            return;

        if (_joinBehavior == PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed) {
            // Make sure it's a button that was actuated.
            if (!(control is ButtonControl))
                return;

            // Make sure it's a device that is usable by the player's actions. We don't want
            // to join a player who's then stranded and has no way to actually interact with the game.
            if (!IsDeviceUsableWithPlayerActions(control.device))
                return;

            JoinPlayer(pairWithDevice: control.device);
        }
    }

    void OnEnable() {
        if (Instance == null) {
            Instance = this;
        } else {
            Debug.LogWarning("Multiple InputManagers in the game. There should only be one PlayerInputManager", this);
            return;
        }

        // Join all players already in the game.
        for (var i = 0; i < PlayerCount; ++i)
            NotifyPlayerJoined(LocalPlayer.All[i]);

        if (_allowJoining)
            EnableJoining();
    }

    void OnDisable() {
        if (Instance == this)
            Instance = null;

        if (_allowJoining)
            DisableJoining();
    }

    bool IsDeviceUsableWithPlayerActions(InputDevice device) {
        Debug.Assert(device != null);

        var playerInput = PlayerPrefab.GetComponentInChildren<PlayerInput>();
        if (playerInput == null)
            return true;

        var actions = playerInput.actions;
        if (actions == null)
            return true;

        foreach (var actionMap in actions.actionMaps)
            if (actionMap.IsUsableWithDevice(device))
                return true;

        return false;
    }

    /// <summary>
    /// Called by <see cref="PlayerInput"/> when it is enabled.
    /// </summary>
    /// <param name="player"></param>
    internal void NotifyPlayerJoined(LocalPlayer player) {
        Debug.Assert(player != null);
        Mediator.Global.Publish(new LocalPlayerJoined { Player = player });
    }

    /// <summary>
    /// Called by <see cref="PlayerInput"/> when it is disabled.
    /// </summary>
    /// <param name="player"></param>
    internal void NotifyPlayerLeft(LocalPlayer player) {
        Debug.Assert(player != null);
        Mediator.Global.Publish(new LocalPlayerLeft { Player = player });
    }
}

public class LocalPlayer {

    public LocalPlayer(int playerIndex, string controlScheme, params InputDevice[] devices) {
        PlayerIndex = playerIndex;
        Devices = new List<InputDevice>(devices);
        ControlScheme = controlScheme;
        All.Add(this);
    }

    public static LocalPlayer FindFirstPairedToDevice(InputDevice device) {
        if (device == null)
            throw new ArgumentNullException(nameof(device));

        foreach (LocalPlayer localPlayer in All) {
            if (localPlayer.Devices.Contains(device))
                return localPlayer;
        }

        return null;
    }

    public bool active => _inputActive;

    /// <summary>
    /// Input actions associated with the player.
    /// </summary>
    /// <remarks>
    /// Note that every player will maintain a unique copy of the given actions such that
    /// each player receives an identical copy. When assigning the same actions to multiple players,
    /// the first player will use the given actions as is but any subsequent player will make a copy
    /// of the actions using <see cref="Object.Instantiate(Object)"/>.
    ///
    /// The asset may contain an arbitrary number of action maps. However, the first action map in the
    /// asset is treated special ...
    ///
    /// Notifications will be send for all actions in the asset, not just for those in the first action
    /// map. This means that if additional maps are manually enabled and disabled,
    ///
    /// There is one exception to this, however. For any action from the asset that is also referenced
    /// by an <see cref="InputSystemUIInputModule"/> sitting on the <see cref="GameObject"/> of
    /// <see cref="uiEventSystem"/>, no notification will be triggered when the action is fired.
    /// </remarks>
    /// <seealso cref="InputUser.actions"/>
    public static InputActionAsset Actions => InputManager.Instance.Actions;

    public string controlScheme {
        get {
            if (!_inputUser.valid)
                return null;

            var scheme = _inputUser.controlScheme;
            return scheme?.name;
        }
    }

    public string defaultControlScheme {
        get => DefaultControlScheme;
        set => DefaultControlScheme = value;
    }

    public string defaultActionMap {
        get => DefaultActionMap;
        set => DefaultActionMap = value;
    }

    /// <summary>
    /// UI InputModule that should have it's input actions synchronized to this PlayerInput's actions.
    /// </summary>
    public InputSystemUIInputModule uiInputModule => InputManager.Instance.UIInputModule;

    /// <summary>
    /// The internal user tied to the player.
    /// </summary>
    public InputUser User => _inputUser;

    /// <summary>
    /// The devices used by the player.
    /// </summary>
    /// <seealso cref="InputUser.pairedDevices"/>
    public ReadOnlyArray<InputDevice> Devices {
        get {
            if (!_inputUser.valid)
                return new ReadOnlyArray<InputDevice>();

            return _inputUser.pairedDevices;
        }
    }

    public bool hasMissingRequiredDevices => User.hasMissingRequiredDevices;

    public static ReadOnlyArray<LocalPlayer> All => new ReadOnlyArray<LocalPlayer>(_allActivePlayers, 0, ActivePlayerCount);

    public static bool isSinglePlayer =>
        ActivePlayerCount <= 1 &&
        (PlayerInputManager.instance == null || !PlayerInputManager.instance.joiningEnabled);

    public void ActivateInput() {
        // Enable default action map, if set.
        if (Actions != null && !string.IsNullOrEmpty(DefaultActionMap)) {
            var actionMap = Actions.TryGetActionMap(DefaultActionMap);
            if (actionMap != null) {
                actionMap.Enable();
                EnabledActionMap = actionMap;
            }
            else
                Debug.LogError($"Cannot find action map '{DefaultActionMap}' in '{Actions}'");
        }
        _inputActive = true;
    }

    public void PassivateInput() {
        EnabledActionMap?.Disable();
        _inputActive = false;
    }

    public void SwitchCurrentActionMap(string mapNameOrId)
    {
        // Must be enabled.
        if (!_enabled) {
            Debug.LogError($"Cannot switch to actions '{mapNameOrId}'; input is not enabled");
            return;
        }

        // Must have actions.
        if (Actions == null) {
            Debug.LogError($"Cannot switch to actions '{mapNameOrId}'; no actions set on PlayerInput");
            return;
        }

        // Must have map.
        var actionMap = Actions.TryGetActionMap(mapNameOrId);
        if (actionMap == null) {
            Debug.LogError($"Cannot find action map '{mapNameOrId}' in actions '{Actions}'");
            return;
        }

        currentActionMap = actionMap;
    }

    public static LocalPlayer GetPlayerByIndex(int playerIndex)
    {
        for (var i = 0; i < ActivePlayerCount; ++i)
            if (_allActivePlayers[i].playerIndex == playerIndex)
                return _allActivePlayers[i];
        return null;
    }

    internal bool AutoSwitchControlScheme;
    internal string DefaultControlScheme;////REVIEW: should we have IDs for these so we can rename safely?
    internal string DefaultActionMap;

    // Value object we use when sending messages via SendMessage() or BroadcastMessage(). Can be ignored
    // by the receiver. We reuse the same object over and over to avoid allocating garbage.
    InputValue InputValueObject;

    internal InputActionMap EnabledActionMap;

    public InputActionMap currentActionMap {
        get => EnabledActionMap;
        set {
            EnabledActionMap?.Disable();
            EnabledActionMap = value;
            EnabledActionMap?.Enable();
        }
    }

    public int PlayerIndex { get; private set; }= -1;
    bool _inputActive;
    bool _enabled;
    Dictionary<string, string> ActionMessageNames;
    InputUser _inputUser;
    Action<InputAction.CallbackContext> ActionTriggeredDelegate;

    public static int ActivePlayerCount { get; private set; }
    internal static LocalPlayer[] _allActivePlayers;
    internal static Action<InputUser, InputUserChange, InputDevice> s_UserChangeDelegate;
    internal static Action<InputControl, InputEventPtr> s_UnpairedDeviceUsedDelegate;
    internal static bool s_OnUnpairedDeviceHooked;

    // The following information is used when the next PlayerInput component is enabled.

    void InitializeActions() {
        if (Actions == null)
            return;

        ////REVIEW: should we *always* Instantiate()?
        // Check if we need to duplicate our actions by looking at all other players. If any
        // has the same actions, duplicate.
        for (var i = 0; i < ActivePlayerCount; ++i)
            if (_allActivePlayers[i].Actions == Actions && _allActivePlayers[i] != this)
            {
                var oldActions = Actions;
                Actions = Object.Instantiate(Actions);
                for (var actionMap = 0; actionMap < oldActions.actionMaps.Count; actionMap++)
                {
                    for (var binding = 0; binding < oldActions.actionMaps[actionMap].bindings.Count; binding++)
                        Actions.actionMaps[actionMap].ApplyBindingOverride(binding, oldActions.actionMaps[actionMap].bindings[binding]);
                }

                break;
            }

        if (uiInputModule != null)
            uiInputModule.actionsAsset = Actions;
    }

    void UninitializeActions() {
        if (Actions == null)
            return;

        if (ActionTriggeredDelegate != null)
            foreach (var actionMap in Actions.actionMaps)
                actionMap.actionTriggered -= ActionTriggeredDelegate;
    }

    void CacheMessageNames() {
        if (Actions == null)
            return;

        if (ActionMessageNames != null)
            ActionMessageNames.Clear();
        else
            ActionMessageNames = new Dictionary<string, string>();

        foreach (var action in Actions)
        {
            action.MakeSureIdIsInPlace();

            var name = action.name;
            if (char.IsLower(name[0]))
                name = char.ToUpper(name[0]) + name.Substring(1);
            ActionMessageNames[action.id] = "On" + name;
        }
    }

    /// <summary>
    /// Initialize <see cref="User"/> and <see cref="Devices"/>.
    /// </summary>
    void AssignUserAndDevices() {
        // If we already have a user at this point, clear out all its paired devices
        // to start the pairing process from scratch.
        if (_inputUser.valid)
            _inputUser.UnpairDevices();

        // All our input goes through actions so there's no point setting
        // anything up if we have none.
        if (Actions == null) {
            // If we have devices we are meant to pair with, do so.  Otherwise, don't
            // do anything as we don't know what kind of input to look for.
            if (s_InitPairWithDevicesCount > 0) {
                for (var i = 0; i < s_InitPairWithDevicesCount; ++i)
                    _inputUser = InputUser.PerformPairingWithDevice(s_InitPairWithDevices[i], _inputUser);
            } else {
                // Make sure user is invalid.
                _inputUser = new InputUser();
            }

            return;
        }

        // If we have control schemes, try to find the one we should use.
        if (Actions.controlSchemes.Count > 0) {
            if (!string.IsNullOrEmpty(s_InitControlScheme)) {
                // We've been given a control scheme to initialize this. Try that one and
                // that one only. Might mean we end up with missing devices.

                var controlScheme = Actions.TryGetControlScheme(s_InitControlScheme);
                if (controlScheme == null) {
                    Debug.LogError($"No control scheme '{s_InitControlScheme}' in '{Actions}'", this);
                } else {
                    TryToActivateControlScheme(controlScheme.Value);
                }
            } else if (!string.IsNullOrEmpty(DefaultControlScheme)) {
                // There's a control scheme we should try by default.

                var controlScheme = Actions.TryGetControlScheme(DefaultControlScheme);
                if (controlScheme == null) {
                    Debug.LogError($"Cannot find default control scheme '{DefaultControlScheme}' in '{Actions}'", this);
                } else {
                    TryToActivateControlScheme(controlScheme.Value);
                }
            }

            // If we did not end up with a usable scheme by now but we've been given devices to pair with,
            // search for a control scheme matching the given devices.
            if (s_InitPairWithDevicesCount > 0 && (!_inputUser.valid || _inputUser.controlScheme == null)) {
                foreach (var controlScheme in Actions.controlSchemes) {
                    for (var i = 0; i < s_InitPairWithDevicesCount; ++i) {
                        var device = s_InitPairWithDevices[i];
                        if (controlScheme.SupportsDevice(device) && TryToActivateControlScheme(controlScheme))
                            break;
                    }
                }
            }
            // If we don't have a working control scheme by now and we haven't been instructed to use
            // one specific control scheme, try each one in the asset one after the other until we
            // either find one we can use or run out of options.
            else if ((!_inputUser.valid || _inputUser.controlScheme == null) && string.IsNullOrEmpty(s_InitControlScheme))
            {
                foreach (var controlScheme in Actions.controlSchemes) {
                    if (TryToActivateControlScheme(controlScheme))
                        break;
                }
            }
        } else {
            // There's no control schemes in the asset. If we've been given a set of devices,
            // we run with those (regardless of whether there's bindings for them in the actions or not).
            // If we haven't been given any devices, we go through all bindings in the asset and whatever
            // device is present that matches the binding and that isn't used by any other player, we'll
            // pair to the player.

            if (s_InitPairWithDevicesCount > 0) {
                for (var i = 0; i < s_InitPairWithDevicesCount; ++i)
                    _inputUser = InputUser.PerformPairingWithDevice(s_InitPairWithDevices[i], _inputUser);
            } else {
                using (var availableDevices = InputUser.GetUnpairedInputDevices()) {
                    foreach (var actionMap in Actions.actionMaps) {
                        foreach (var binding in actionMap.bindings) {
                            // See if the binding matches anything available.
                            InputDevice matchesDevice = null;
                            foreach (var device in availableDevices)
                            {
                                if (InputControlPath.TryFindControl(device, binding.effectivePath) != null)
                                {
                                    matchesDevice = device;
                                    break;
                                }
                            }

                            if (matchesDevice == null)
                                continue;

                            if (_inputUser.valid && _inputUser.pairedDevices.ContainsReference(matchesDevice))
                            {
                                // Already paired to this device.
                                continue;
                            }

                            _inputUser = InputUser.PerformPairingWithDevice(matchesDevice, _inputUser);
                        }
                    }
                }
            }
        }

        // If we don't have a valid user at this point, we don't have any paired devices.
        if (_inputUser.valid)
            _inputUser.AssociateActionsWithUser(Actions);
    }

    void UnassignUserAndDevices() {
        if (_inputUser.valid)
            _inputUser.UnpairDevicesAndRemoveUser();
        if (Actions != null)
            Actions.devices = null;
    }

    bool TryToActivateControlScheme(InputControlScheme controlScheme) {
        ////FIXME: this will fall apart if account management is involved and a user needs to log in on device first

        // Pair any devices we may have been given.
        if (s_InitPairWithDevicesCount > 0)
        {
            ////REVIEW: should AndPairRemainingDevices() require that there is at least one existing
            ////        device paired to the user that is usable with the given control scheme?

            // First make sure that all of the devices actually work with the given control scheme.
            // We're fine having to pair additional devices but we don't want the situation where
            // we have the player grab all the devices in s_InitPairWithDevices along with a control
            // scheme that fits none of them and then AndPairRemainingDevices() supplying the devices
            // actually needed by the control scheme.
            for (var i = 0; i < s_InitPairWithDevicesCount; ++i) {
                var device = s_InitPairWithDevices[i];
                if (!controlScheme.SupportsDevice(device))
                    return false;
            }

            // We're good. Give the devices to the user.
            for (var i = 0; i < s_InitPairWithDevicesCount; ++i) {
                var device = s_InitPairWithDevices[i];
                _inputUser = InputUser.PerformPairingWithDevice(device, _inputUser);
            }
        }

        if (!_inputUser.valid)
            _inputUser = InputUser.CreateUserWithoutPairedDevices();

        _inputUser.ActivateControlScheme(controlScheme).AndPairRemainingDevices();
        if (User.hasMissingRequiredDevices) {
            _inputUser.ActivateControlScheme(null);
            _inputUser.UnpairDevices();
            return false;
        }

        return true;
    }

    void AssignPlayerIndex() {
        if (PlayerIndex != -1) return;
        var minPlayerIndex = int.MaxValue;
        var maxPlayerIndex = int.MinValue;

        for (var i = 0; i < ActivePlayerCount; ++i) {
            var playerIndex = _allActivePlayers[i].PlayerIndex;
            minPlayerIndex = Math.Min(minPlayerIndex, (int)playerIndex);
            maxPlayerIndex = Math.Max(maxPlayerIndex, (int)playerIndex);
        }

        if (minPlayerIndex != int.MaxValue && minPlayerIndex > 0) {
            // There's an index between 0 and the current minimum available.
            PlayerIndex = minPlayerIndex - 1;
        } else if (maxPlayerIndex != int.MinValue) {
            // There may be an index between the minimum and maximum available.
            // Search the range. If there's nothing, create a new maximum.
            for (var i = minPlayerIndex; i < maxPlayerIndex; ++i) {
                if (GetPlayerByIndex(i) == null) {
                    PlayerIndex = i;
                    return;
                }
            }

            PlayerIndex = maxPlayerIndex + 1;
        } else
            PlayerIndex = 0;
    }

    public void Enable() {
        _enabled = true;

        AssignPlayerIndex();
        InitializeActions();
        AssignUserAndDevices();
        ActivateInput();

        // Add to global list and sort it by player index.
        ArrayHelpers.AppendWithCapacity(ref _allActivePlayers, ref ActivePlayerCount, this);
        for (var i = 1; i < ActivePlayerCount; ++i)
            for (var j = i; j > 0 && _allActivePlayers[j - 1].PlayerIndex > _allActivePlayers[j].PlayerIndex; --j) {
                LocalPlayer temp = _allActivePlayers[j];
                _allActivePlayers[j] = _allActivePlayers[j - 1];
                _allActivePlayers[j - 1] = temp;
            }

        // If it's the first player, hook into user change notifications.
        if (ActivePlayerCount == 1) {
            if (s_UserChangeDelegate == null)
                s_UserChangeDelegate = OnUserChange;
            InputUser.onChange += s_UserChangeDelegate;
        }

        // In single player, set up for automatic control scheme switching.
        // Otherwise make sure it's disabled.
        if (isSinglePlayer && !s_OnUnpairedDeviceHooked) {
            if (s_UnpairedDeviceUsedDelegate == null)
                s_UnpairedDeviceUsedDelegate = OnUnpairedDeviceUsed;
            InputUser.onUnpairedDeviceUsed += s_UnpairedDeviceUsedDelegate;
            ++InputUser.listenForUnpairedDeviceActivity;
            s_OnUnpairedDeviceHooked = true;
        } else if (s_OnUnpairedDeviceHooked) {
            InputUser.onUnpairedDeviceUsed -= s_UnpairedDeviceUsedDelegate;
            --InputUser.listenForUnpairedDeviceActivity;
            s_OnUnpairedDeviceHooked = false;
        }

        // Trigger join event.
        InputManager.Instance?.NotifyPlayerJoined(this);
    }

    public void Disable() {
        _enabled = false;

        // Remove from global list.
        var index = ArrayHelpers.IndexOfReference(_allActivePlayers, this, ActivePlayerCount);
        if (index != -1)
            ArrayHelpers.EraseAtWithCapacity(_allActivePlayers, ref ActivePlayerCount, index);

        // Unhook from change notifications if we're the last player.
        if (ActivePlayerCount == 0 && s_UserChangeDelegate != null)
            InputUser.onChange -= s_UserChangeDelegate;
        if (ActivePlayerCount == 0 && s_OnUnpairedDeviceHooked) {
            InputUser.onUnpairedDeviceUsed -= s_UnpairedDeviceUsedDelegate;
            --InputUser.listenForUnpairedDeviceActivity;
            s_OnUnpairedDeviceHooked = false;
        }

        // Trigger leave event.
        PlayerInputManager.instance?.NotifyPlayerLeft(this);

        PassivateInput();
        UnassignUserAndDevices();
        UninitializeActions();

        PlayerIndex = -1;
    }

    static void OnUnpairedDeviceUsed(InputControl control, InputEventPtr eventPtr) {
        // We only support automatic control scheme switching in single player mode.
        // OnEnable() should automatically unhook us.
        if (!isSinglePlayer)
            return;

        var player = All[0];
        if (Actions == null)
            return;

        // Go through all control schemes and see if there is one usable with the device.
        // If so, switch to it.
        var controlScheme = InputControlScheme.FindControlSchemeForDevice(control.device, Actions.controlSchemes);
        if (controlScheme != null) {
            // First remove the currently paired devices, then pair the device that was used,
            // and finally switch to the new control scheme and grab whatever other devices we're missing.
            player._inputUser.UnpairDevices();
            InputUser.PerformPairingWithDevice(control.device, user: player._inputUser);
            player._inputUser.ActivateControlScheme(controlScheme.Value).AndPairRemainingDevices();
        }
    }

}

}
