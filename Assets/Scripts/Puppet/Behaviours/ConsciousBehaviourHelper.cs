using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion;
using RootMotion.Dynamics;

public partial class ConsciousBehaviour : BehaviourBase
{

    /// <summary>
    /// Determines whether this ragdoll is facing up (false) or down (true).
    /// </summary>
    public bool IsProne()
    {
        float dot = Vector3.Dot(puppetMaster.muscles[0].transform.rotation * hipsForward, puppetMaster.targetRoot.up);
        return dot < 0f;
    }

    // Gets the falloff value of muscle 'i' according to it's kinship degree from muscle 'muscleIndex' and the parent and child falloff values.
    private float GetFalloff(int i, int muscleIndex, float falloffParents, float falloffChildren)
    {
        if (i == muscleIndex) return 1f;

        bool isChild = puppetMaster.muscles[muscleIndex].childFlags[i];
        int kinshipDegree = puppetMaster.muscles[muscleIndex].kinshipDegrees[i];

        return Mathf.Pow(isChild ? falloffChildren : falloffParents, kinshipDegree);
    }

    // Gets the falloff value of muscle 'i' according to it's kinship degree from muscle 'muscleIndex' and the parent, child and group falloff values.
    private float GetFalloff(int i, int muscleIndex, float falloffParents, float falloffChildren, float falloffGroup)
    {
        float falloff = GetFalloff(i, muscleIndex, falloffParents, falloffChildren);

        if (falloffGroup > 0f && i != muscleIndex && InGroup(puppetMaster.muscles[i], puppetMaster.muscles[muscleIndex]))
        {
            falloff = Mathf.Max(falloff, falloffGroup);
        }

        return falloff;
    }

    // Returns true is the groups match directly OR in the group overrides.
    private bool InGroup(Muscle muscle1, Muscle muscle2)
    {
        if (muscle1 == muscle2) return true;

        foreach (MuscleBasePropsGroup musclePropsGroup in groupOverrides)
        {
            foreach (Muscle m in musclePropsGroup.muscleGroup.muscles)
            {
                if (m == muscle1)
                {
                    foreach (Muscle m2 in musclePropsGroup.muscleGroup.muscles)
                    {
                        if (m2 == muscle2) return true;
                    }
                }
            }
        }

        return false;
    }

    // Returns the MusclePropsGroup of the specified muscle group.
    private MuscleProps GetProps(Muscle m)
    {
        foreach (MuscleBasePropsGroup g in groupOverrides)
        {
            foreach (Muscle m1 in g.muscleGroup.muscles)
            {
                if (m1 == m) return g.props;
            }
        }
        return defaults;
    }
}

