
# Overview
TODO(james7132): Document

# States
State is represented as simple data objects.

# Inputs
Inputs are represented as simple data objects. `PlayerInput` is a plain old C#
struct that contains a summary of all input for a player at a given frame.
`MatchInput` represents an aggregation of all player's inputs for a frame. You
can get the PlayerInput for a given player ID by indexing into it like any
array.  Despite being able to index MatchInput like an array of PlayerInputs, it
operates under value semantics. It should be noted that both structures only
contain input information and do not track the time at which they were generated.

Both `MatchInput` and `PlayerInput` implement `IValidatable`. If `IsValid` is
false, the input is not entirely generated locally, or, when in a netowrk match,
has not been verified to be the correct input by the server.

Inputs are produced by objects of the interface `IInputSource<I>`. The current
input for a frame can be sampled with the `SampleInput()` method. Interfacing
the game with a new input framework (i.e. InControl, Rewired, or Unity's New
Input System) involves implementing a new IInputSource and selecting it in the
game's global configuration files.

# Simulation
Simulations are represented as the interface `ISimulation<S, I>`. The sole
method it defines is `S Simuilate(S, I)`. This defines a mapping of the state S
and input I to a new state S. It should be noted that these simulations usually
do not mutate the input state, instead creating a new state based on the input.

In practice, the engine uses two classes `MatchInputContext` and
`PlayerInputContext` as the input types instead of the raw inputs. Both
represent the current and last frame's inputs, information some of the
simulations both need to properly run.

Simulations generally should be completely self contained and the simulation
itself should be stateless. As a general rule, the only information needed to
compute the next state should be provided the arguments to `Simulate(...)`.
Exceptions to this rule usually include some unchanging global configuration
options or manipulating the intermediate state of a higher level simulation. An
example of the latter exception is hitboxes. The Match level simulation of
hitboxes are self contained, as in the state of hitboxes from one call to
Simulate to another are hold no state, but within one call to the Match level
simulate are multiple calls to player level simulations that may manipulate the
global state of the hitbox simulation.

# Views
TODO(james7132): Document

# Configuration
TODO(james7132): Document
