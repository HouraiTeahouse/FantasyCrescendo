using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.EditorAttributes; 
using System;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {
		
[CreateAssetMenu(menuName = "Config/Gameplay Config")]
public class GameplayConfig : ScriptableObject {

	[SerializeField] [Type(typeof(IInputSource), CommonName="InputSource")] 
	string inputSource;

	public IInputSource CreateInputSource(MatchConfig config) {
		var sourceType = Type.GetType(inputSource);
		return (IInputSource)Activator.CreateInstance(sourceType, config);
	}

}

}