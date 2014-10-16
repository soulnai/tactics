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

	public enum unitActions{
		idle,
		readyToMove,
		moving,
		meleeAttack,
		rangedAttack,
		magicAttack,
		skillAttack,
		healAttack
	}

	public enum resistTypes{
		strenght,
		dexterity,
		magic
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
