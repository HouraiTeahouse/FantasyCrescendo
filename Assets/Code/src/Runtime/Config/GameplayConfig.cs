using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.EditorAttributes; 
using System;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {
		
[CreateAssetMenu(menuName = "Config/Gameplay Config")]
public class GameplayConfig : ScriptableObject {

	[SerializeField] [Type(typeof(IMatchInputSource), CommonName="InputSource")] 
	string inputSource;

  [SerializeField] float _maxLedgeHangTime = 10;
  [SerializeField] float _ledgeGrabCooldown = 0.5f;

  public short MaxLedgeHangTime => (short)(_maxLedgeHangTime / Time.fixedDeltaTime);
  public short LedgeGrabCooldown => (short)(_ledgeGrabCooldown / Time.fixedDeltaTime);

	public IMatchInputSource CreateInputSource(MatchConfig config) {
		var sourceType = Type.GetType(inputSource);
		return (IMatchInputSource)Activator.CreateInstance(sourceType, config);
	}

}

}