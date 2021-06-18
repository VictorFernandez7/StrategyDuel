using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers {

    // NaxHelper
    public static bool SetTriggerIfExists(this Animator anim, string trigger) {
        if (!anim.isInitialized) {
            return false;
        }
        for (int i = 0; i < anim.parameterCount; i++) {
            if (anim.parameters[i].name == trigger) {
                anim.SetTrigger(trigger);
                return true;
            }
        }
        return false;
    }

    public static bool CompareLists<T>(List<T> aListA, List<T> aListB)
    {
        if (aListA == null || aListB == null || aListA.Count != aListB.Count)
            return false;
        if (aListA.Count == 0)
            return true;
        Dictionary<T, int> lookUp = new Dictionary<T, int>();
        // create index for the first list
        for(int i = 0; i < aListA.Count; i++)
        {
            int count = 0;
            if (!lookUp.TryGetValue(aListA[i], out count))
            {
                lookUp.Add(aListA[i], 1);
                continue;
            }
            lookUp[aListA[i]] = count + 1;
        }
        for (int i = 0; i < aListB.Count; i++)
        {
            int count = 0;
            if (!lookUp.TryGetValue(aListB[i], out count))
            {
                // early exit as the current value in B doesn't exist in the lookUp (and not in ListA)
                return false;
            }
            count--;
            if (count <= 0)
                lookUp.Remove(aListB[i]);
            else
                lookUp[aListB[i]] = count;
        }
    // if there are remaining elements in the lookUp, that means ListA contains elements that do not exist in ListB
    return lookUp.Count == 0;
    }

    public static T[] ShuffleArray<T>(T[] array, int seed) {
		System.Random prng = new System.Random (seed);

		for (int i =0; i < array.Length -1; i ++) {
			int randomIndex = prng.Next(i,array.Length);
			T tempItem = array[randomIndex];
			array[randomIndex] = array[i];
			array[i] = tempItem;
		}

		return array;
	}
}
