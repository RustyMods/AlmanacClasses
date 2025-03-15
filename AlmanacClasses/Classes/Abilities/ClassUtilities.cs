using UnityEngine;

namespace AlmanacClasses.Classes.Abilities
{
    public static class ClassUtilities
    {
        /// <summary>
        /// Returns true with a specified weight, for example, 0.5f would equal to true roughly 50% of the time.
        /// </summary>
        /// <param name="weight">The probability of the method returning true.</param>
        /// <returns></returns>
        public static bool RandomBoolWithWeight(float weight) => Random.value < weight;
    }
}
