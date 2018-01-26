using HouraiTeahouse.EditorAttributes;
using System;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {
		
[CreateAssetMenu(menuName = "Config/Gameplay Config")]
public class GameplayConfig : ScriptableObject {

	[SerializeField] [Type(typeof(IInputSource<MatchInput>), CommonName="InputSource")] 
	string inputSource;

	public IInputSource<MatchInput> CreateInputSource(MatchConfig config) {
		var sourceType = Type.GetType(inputSource);
		return (IInputSource<MatchInput>)Activator.CreateInstance(sourceType, config);
	}

}

}