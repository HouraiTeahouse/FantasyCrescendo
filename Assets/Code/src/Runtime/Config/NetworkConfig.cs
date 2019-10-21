
using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.Attributes; 
using System;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {
		
[CreateAssetMenu(menuName = "Config/Network Config")]
public class NetworkConfig : ScriptableObject {

  [Range(1, 60)] public uint InputSendRate = 1;
  [Range(1, 60)] public uint StateSendRate = 1;

}

}