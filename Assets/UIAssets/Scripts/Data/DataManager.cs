using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataManager : MonoBehaviour {

	public int numberOfPlayers = 5;
	private List<CharacterSlot> availableCharacters = null;
	private List<string> availableStages = null;
	public List<PlayerOptions> playerOptions = null;
	public Mediator mediator = null;


	public static DataManager getDataManagerInstance()
	{
		GameObject go = GameObject.Find( "DataManager" );
		if( go == null )
		{
			Debug.LogError( "Couldn't find the data manager object." );
			return null;
		}
		return go.GetComponent<DataManager>();
	}


	// Use this for initialization
	void Awake () {
		availableCharacters = new List<CharacterSlot>();
		availableCharacters.Add( new CharacterSlot("Reimu") );
		availableCharacters.Add( new CharacterSlot("Marisa") );
		availableCharacters.Add( new CharacterSlot("Miko") );
		availableCharacters.Add( new CharacterSlot("Wriggle") );
		availableCharacters.Add( new CharacterSlot("Byakuren") );
		availableCharacters.Add( new CharacterSlot("Sukuna") );
		availableCharacters.Add( new CharacterSlot("Sanae") );
		availableCharacters.Add( new CharacterSlot("Youmu") );
		availableCharacters.Add( new CharacterSlot("Remilia") );
		availableCharacters.Add( new CharacterSlot("Sakuya") );
		availableCharacters.Add( new CharacterSlot("Utsuho") );
		availableCharacters.Add( new CharacterSlot("Nitori") );
		availableCharacters.Add( new CharacterSlot("Reisen") );
		availableCharacters.Add( new CharacterSlot("Aya") );
		availableCharacters.Add( new CharacterSlot("Cirno") );
		availableCharacters.Add( new CharacterSlot("Random") );


		availableStages = new List<string>();

		availableStages.Add( "Hakurei Shrine" );
		availableStages.Add( "Marisa's House" );
		availableStages.Add( "Clocktower" );
		availableStages.Add( "Misty Lake" );
		availableStages.Add( "Bamboo Forest" );
		availableStages.Add( "Hisoutensoku" );
		availableStages.Add( "Youkai Mountain" );
		availableStages.Add( "Genbu Ravine" );
		availableStages.Add( "Makai" );
		availableStages.Add( "Blazing Hell Reactor" );
		availableStages.Add( "Netherworld" );
		availableStages.Add( "Bhava-Agra" );
		availableStages.Add( "Netherworld" );
		availableStages.Add( "Old Hakurei Shrine" );
		availableStages.Add( "New Super Marisa Land" );
		availableStages.Add( "Twilight City" );
		
		
		playerOptions = new List<PlayerOptions>();
		int i;

		for( i = 0; i < numberOfPlayers; i++ )
		{
			PlayerOptions po = new PlayerOptions();
			po.setNumber( i );
			if( i == 0 )
			{
				po.setPlayerType( PlayerOptions.PlayerType.PLAYER );
			}
			playerOptions.Add( po );
		}
		initializeMediator();
	}

	/// <summary>
	/// Check if the battle can start, that is when there is at least
	/// two players or CPUs ready to battle.
	/// </summary>
	/// <returns><c>true</c>, if the battle can start, <c>false</c> otherwise.</returns>
	public bool isReadyToStartGame()
	{
		int counter = 0;
		int i;

		for( i = 0; i < numberOfPlayers; i++ )
		{
			if( playerOptions[i].getPlayerType() != PlayerOptions.PlayerType.DISABLED )
			{
				counter++;
			}
		}

		mediator.Publish<DataCommands.ReadyToFight> (
			new DataCommands.ReadyToFight() { isReady = (counter > 1) } );
		return (counter > 1);
	}

	public void initializeMediator()
	{
		mediator = new Mediator();
		mediator.Subscribe<DataCommands.ChangePlayerLevelCommand>( this.onChangePlayerLevel );
		mediator.Subscribe<DataCommands.ChangePlayerMode>( this.onChangePlayerMode );
		mediator.Subscribe<DataCommands.UserChangingOptions>( this.onUserChangingOptions );
	}

	public List<string> getAvailableStages()
	{
		return availableStages;
	}

	public List<CharacterSlot> getAvailableCharacters()
	{
		return availableCharacters;
	}

	public List<PlayerOptions> getPlayerOptions()
	{
		return playerOptions;
	}

	public void onChangePlayerLevel( DataCommands.ChangePlayerLevelCommand cmd )
	{
		if( cmd.playerNum >= 0 && cmd.playerNum < numberOfPlayers )
		{
			playerOptions[ cmd.playerNum ].level = cmd.newLevel;
		}
		else
		{
			Debug.LogError( "Invalid player number : " + cmd.newLevel );
		}
	}

	public void onUserChangingOptions( DataCommands.UserChangingOptions cmd )
	{
		//Debug.Log ("Ai mano " + cmd.isUserChangingOptions  + "  -  " + Time.time );
		if( cmd.isUserChangingOptions )
		{
			mediator.Publish<DataCommands.ReadyToFight> (
				new DataCommands.ReadyToFight() { isReady = false } );
		}
		else
		{
			isReadyToStartGame();
		}

	}
	
	public void onChangePlayerMode( DataCommands.ChangePlayerMode cmd )
	{
		if( cmd.playerNum >= 0 && cmd.playerNum < numberOfPlayers )
		{
			playerOptions[ cmd.playerNum ].setNextType();
		}
		else
		{
			Debug.LogError( "Invalid player number while updating player mode: " + cmd.playerNum );
		}

		isReadyToStartGame();
	}
}
