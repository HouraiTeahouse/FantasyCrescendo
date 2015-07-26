using UnityEngine;
using System.Collections;

public class BeforeMenuInputInterface : InputInterface {

	public ScreenManager screenManager = null;
	public Animator nextScreenAnimator = null;

	// Use this for initialization
	public override void processInputs()
	{
		if( Input.GetButton( "jump" ) )
		{
			screenManager.OpenPanel( nextScreenAnimator );
		}
	}
}
