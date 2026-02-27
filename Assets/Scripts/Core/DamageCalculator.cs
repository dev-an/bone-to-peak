using UnityEngine;

namespace BoneToPeak.Core
{
    public static class DamageCalculator
    {
        public static float Calculate(float attack, float defense)
        {
            return Mathf.Max(1f, attack - defense);
        }
    }
}
