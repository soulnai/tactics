using UnityEngine;
using System.Collections;

namespace EnumSpace {
	public enum unitStates{
		normal,
		stunned,
		poisoned,
		burning,
		freezed,
		dead
	}

	public enum unitClass{
		squire,
		warrior,
		palladin,
		knigth,
		mage,
		archer
	}

	public enum unitActions{
		idle,
		readyToMove,
		moving,
		readyToAttack,
		attacking,
		casting
	}

	public enum unitAttributes{
		//Stats
		HP,
		MP,
		AP,
		HPmax,
		MPmax,
		APmax,

		//Base Attributes
		strenght,
		dexterity,
		magic,
		
	
		//Def
		PhysicalDef,
		magicDef,
		poisonDef,
		fireDef,
		iceDef,

        //Additional parameters
        movementPerActionPoint
	}

	public enum damageTypes{
		poison,
		fire,
		water,
		ice,
		electricity,
		earth,
		darkness,
		light,
		//-------------
		slashing,
		pearcing,
		blunt
	}

	public enum attackTypes{
		melee,
		ranged,
		magic,
		heal
	}

	public enum areaPatterns{
		line,
		circle,
		cross,
		standart
	}

	public enum playerTurnStates{
		start,
		end,
		nextPlayerTurn
	}

	public enum unitTurnStates{
		canInteract,
		blockInteract
	}

	public enum editorStates{
		setType,
		setHeight
	}

	public enum tooltipTypes{
		effect,
		ability,
		unit
	}

	public enum matchStates{
		selectUnits,
		placeUnits,
		battle,
		victory
	}

	public enum playerType{
		player,
		ai,
		spectr
	}
}
