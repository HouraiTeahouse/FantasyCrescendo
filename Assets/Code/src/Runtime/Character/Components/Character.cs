using HouraiTeahouse.Attributes;
using HouraiTeahouse.FantasyCrescendo.Players;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class Character : MonoBehaviour, IPlayerSimulation, IPlayerView {

    [Header("Config")]
    [SerializeField] CharacterControllerBuilder _states;
    [SerializeField] internal CharacterMovement _movement;
    [SerializeField] internal CharacterPhysics _physics;
    [SerializeField] internal CharacterIndicator _indicator;
    [SerializeField] internal CharacterRespawn _respawn;
    [SerializeField] internal CharacterShield _shield;

    [Header("Debug")]
    [ReadOnly] [SerializeField] PlayerConfig _config;
    [ReadOnly] [SerializeField] PlayerState _state;

    public PlayerConfig Config => _config;
    public PlayerState State => _state;
    public bool IsView { get; private set; }
    public StateController StateController { get; private set; }

    CharacterComponent[] _components;
    CharacterContext context;
    Dictionary<uint, State> stateMap;

    void Awake() {
        context = new CharacterContext();
        _components = new CharacterComponent[] {
            _movement, _physics, _indicator, _respawn, _shield
        };
        foreach (var component in _components) {
            component.Character = this;
            component.PreInit(this);
        }
    }

    void OnEnable() => RegisterCamera(true);
    void OnDisable() => RegisterCamera(false);

    public virtual Task Initialize(PlayerConfig config, bool isView = false) {
        _config = config;
        IsView = isView;
        var tasks = new List<Task>();
        tasks.Add(InitializeStateMachine());
        foreach (var component in _components) {
            tasks.Add(component.Init(this));
        }
        if (IsView) RegisterCamera(true);
        return Task.WhenAll(tasks);
    }

    public virtual void Presimulate(in PlayerState state) {
        foreach (var component in _components) {
            component.Presimulate(state);
        }
        UpdateView(state);
    }

    public virtual void Simulate(ref PlayerState state, in PlayerInputContext input) {
        _state = state;
        UpdateTime(ref state);

        var controllerState = GetControllerState(state);
        context.State = state;
        context.Input = input;
        context.ShieldBroken = _shield.IsShieldBroken(state);
        context.IsGrounded = _physics.IsGrounded;
        context.CanJump = _movement.CanJump(state);
        context.StateLength = controllerState.Data.Length;

        controllerState = StateController.UpdateState(controllerState, context);
        controllerState.Simulate(ref context.State, input);
        state = context.State;

        state.StateID = controllerState.Id;

        foreach (var component in _components) {
            component.Simulate(ref state, input);
        }
    }

    public virtual void UpdateView(in PlayerState state) {
        _state = state;
        gameObject.SetActive(state.IsActive);
        _components.UpdateView(state);
        GetControllerState(state)?.UpdateView(state);
        foreach (var component in _components) {
            component.UpdateView(state);
        }
    }

    static void UpdateTime(ref PlayerState state) {
        state.StateTick++;
        if (state.RespawnTimeRemaining > 0) {
            state.RespawnTimeRemaining--;
        }
    }

    public void Dispose() {
        Destroy(gameObject);
    }

    public virtual void ResetState(ref PlayerState state) {
        state.StateID = StateController.DefaultState.Id;
        state.Damage = _config.DefaultDamage;
        state.ShieldDamage = 0;
    }

    public State GetControllerState(in PlayerState state) =>  GetControllerState(state.StateID);

    public State GetControllerState(uint id) {
        if (stateMap.TryGetValue(id, out State controllerState)) {
            return controllerState;
        }
        return null;
    }

    void RegisterCamera(bool register) {
        if (!IsView) return;
        var target = FindObjectOfType<MatchCameraTarget>();
        if (target != null) {
            if (register) {
                target.RegisterTarget(transform);
            } else {
                target.UnregisterTarget(transform);
            }
        }
    }

    Task InitializeStateMachine() {
        _states = Instantiate(_states); // Create a per-player copy of the builder.
        StateController = _states.BuildCharacterControllerImpl(new StateControllerBuilder());
        stateMap = StateController.States.ToDictionary(s => s.Id, s => s);
        return Task.WhenAll(stateMap.Values.Select(s => s.Initalize(this)).Where(t => t != null));
    }

}

}
