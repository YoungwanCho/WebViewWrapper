using System;
using UnityEngine;

namespace MWV
{
	public interface IInputManagerHandler
	{
		void OnInputClick(GameObject target, Vector2 position);

		void OnInputMove(GameObject target, Vector2 direction, float distence, InputPhases phase);
	}
}