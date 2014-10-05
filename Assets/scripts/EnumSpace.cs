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
		moving,
		meleeAttack,
		rangedAttack,
		magicAttack,
		skillAttack
	}

	public enum damageTypes{
		poison,
		fire,
		water,
		electricity,
		earth,
		darkness,
		light
	}
}
