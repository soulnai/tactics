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
		attacking
	}

	public enum unitAttributes{
		//Stats
		HP,
		MP,
		AP,
		HPmax,
		MPmax,
		APmax,

		//Attributes
		strenght,
		dexterity,
		magic,
	
		//Def
		PhysicalDef,
		magicDef,
		poisonDef,
		fireDef,
		iceDef
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
		backstab,
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
		start,
		end,
		nextUnitTurn
	}

	public enum editorStates{
		setType,
		setHeight
	}
}
