using UnityEngine;
using System.Collections;

public class ReadyToFightUI : MonoBehaviour {

	public GameObject readyToFightButton = null;
	private Mediator mediator = null;
	
	void Start()
	{
		if( readyToFightButton == null )
		{
			Debug.Log( "Please set all game objects needed by the ReadyToFightUI component" );
			return;
		}
		DataManager dataManager = DataManager.getDataManagerInstance();
		if( dataManager == null )
		{
			Debug.Log( "The ReadyToFightUI component couldn't find the data manager" );
		}
		
		mediator = dataManager.mediator;
		mediator.Subscribe<DataCommands.ReadyToFight>( this.onReadyToFight );
	}

	public void onReadyToFight( DataCommands.ReadyToFight cmd )
	{
		readyToFightButton.SetActive( cmd.isReady );
	}



}
