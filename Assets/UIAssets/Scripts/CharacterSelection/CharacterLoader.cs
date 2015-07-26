using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; 

public class CharacterLoader : MonoBehaviour {

	public GameObject characterSlotPanel = null;
	public GameObject chrPanelToFill = null;
	public GameObject playerSlotPanel = null;
	public GameObject plyPanelToFill = null;
	private DataManager dataManager = null;

	// Use this for initialization
	void Start () {
		if( characterSlotPanel == null || chrPanelToFill == null || plyPanelToFill == null || playerSlotPanel == null)
		{
			Debug.LogError ("Please fill all gameobjects needed by this component.");
			return;
		}

		GameObject go = GameObject.Find( "DataManager" );
		if( go == null )
		{
			Debug.LogError( "Couldn't find the data manager object." );
			return;
		}
		dataManager = go.GetComponent<DataManager>();
		if( dataManager == null )
		{
			Debug.LogError( "Couldn't find the data manager component in the data manager object." );
			return;
		}

		fillPanel();
		fillPlayerSlots();
	}

	public void selectCharacter( int playerNum, string characterName )
	{
		Debug.Log ( playerNum +  "  " + characterName ) ;
	}

	public void fillPanel()
	{
		int i = 0;
		List<CharacterSlot> characters = dataManager.getAvailableCharacters();
		for( i = 0; i < characters.Count; i++ )
		{
			GameObject go = GameObject.Instantiate( characterSlotPanel ) as GameObject;
			Text text = go.GetComponentInChildren<Text>();
			if( text == null )
			{
				Debug.LogError( "Couldn't find the text component in the character slot.");
				return;
			}
			text.text = characters[i].characterName;
			go.transform.SetParent( chrPanelToFill.transform, false );
		}
	}

	public void fillPlayerSlots()
	{
		int i = 0;
		List<PlayerOptions> playerOptions = dataManager.getPlayerOptions();
		for( i = 0; i < playerOptions.Count; i++ )
		{
			GameObject go = GameObject.Instantiate( playerSlotPanel ) as GameObject;
			PlayerSlotUI psu = go.GetComponent<PlayerSlotUI>();
			if( psu == null )
			{
				Debug.LogError( "The player slot object should have a PlayerSlotUI component.");
				return;
			}
			psu.updateUIMode( playerOptions[i].getPlayerType() );
			psu.setPlayerNumber( i );

			go.transform.SetParent( plyPanelToFill.transform, false );
		}
	}

}
