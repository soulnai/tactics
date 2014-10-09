using UnityEngine;
using System.Collections;

namespace EnumSpace {
	public enum unitStates{
		normal,
		stunned,
		poisoned,
		burning,
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
		cross
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

}
