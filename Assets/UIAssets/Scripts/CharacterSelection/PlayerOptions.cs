using UnityEngine;
using System.Collections;

public class PlayerOptions {

	public enum PlayerType
	{
		DISABLED,
		CPU,
		PLAYER
	};

	public PlayerType type = PlayerType.DISABLED;
	public int level = 3;
	public int playerNumber = 0;
	public CharacterSlot selectedSlot = null;
	public int skinColor = 0;

	public void setNumber( int i )
	{
		playerNumber = i;
	}

	public PlayerType getPlayerType ()
	{
		return type;
	}

	public static PlayerType getNextType( PlayerType pt )
	{
		if( pt == PlayerType.PLAYER )
		{
			pt = PlayerType.DISABLED;
		}
		else if( pt == PlayerType.CPU )
		{
			pt = PlayerType.PLAYER;
		}
		else if( pt == PlayerType.DISABLED )
		{
			pt = PlayerType.CPU;
		}
		else
		{
			Debug.LogError("Invalid player type");
		}
		
		return pt;
	}

	public void setPlayerType( PlayerType pt )
	{
		type = pt;
	}

	public void setNextType()
	{
		type = PlayerOptions.getNextType( type );
	}
}
