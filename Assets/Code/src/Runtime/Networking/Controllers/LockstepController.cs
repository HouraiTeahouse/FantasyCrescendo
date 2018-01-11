using System.Collections.Generic;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

//public abstract class AbstractNetworkClientController : GameController {

  //protected INetworkClient Client { get; }

  //protected internal AbstractNetworkClientController(INetworkClient client) {
    //Client = client;
  //}

//}

//public abstract class AbstractNetworkServerController : GameController {

  //protected INetworkServer Server { get; }

  //protected internal AbstractNetworkServerController(INetworkServer server) {
    //Server = server;
  //}

//}

//public class LockstepClientController : AbstractNetworkClientController {

  //public IInputSource<GameInput> InputSource { get; set; }

  //GameInput? latestInput;
  //GameInput[] cachedInputSet;

  //public LockstepClientController(INetworkClient client) : base(client) {
    //cachedInputSet = new GameInput[0];
    //Client.ReceivedInputs += (time, inputs) => {
      //if (time < Timestep){
        //return;
      //}
      //if (latestInput == null) {
        //latestInput = inputs.First();
      //} else {
        //latestInput.Value.MergeWith(inputs.First());
      //}
    //};
  //}

  //public override void Update() {
    //if (latestInput == null) {
      //latestInput = InputSource.SampleInput();
    //} else if (latestInput.Value.IsValid) {
      //CurrentState = Simulation.Simulate(CurrentState, latestInput.Value);
      //latestInput = InputSource.SampleInput();
      //Timestep++;
    //}
    //cachedInputSet[0] = latestInput.Value;
    //Client.SendInput(Timestep, cachedInputSet);
  //}

//}

//public class LockstepServerController : AbstractNetworkServerController  {

  //public LockstepServerController(INetworkServer server) : base(server) {
  //}

//}

}
