﻿using UnityEngine;
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
		magic
	}

	public enum areaPatterns{
		line,
		circle,
		cross
	}
}
