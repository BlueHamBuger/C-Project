using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RootMotion.Dynamics
{
    public partial class PuppetMaster : MonoBehaviour
    {
        public UpdateDelegate OnPreIK = null;

        /// <summary>
        ///  加入现有 muscle，以来防止 内存的重复分配
        /// </summary>
        /// <param name="muscle"></param>
        /// <param name="joint"></param>
        /// <param name="target"></param>
        /// <param name="connectTo"></param>
        /// <param name="targetParent"></param>
        /// <param name="muscleProps"></param>
        /// <param name="forceTreeHierarchy"></param>
        /// <param name="forceLayers"></param>
        public void AddMuscle(Muscle muscle, ConfigurableJoint joint, Transform target, Rigidbody connectTo, Transform targetParent, bool forceTreeHierarchy = false, bool forceLayers = true)
        {
            if (!CheckIfInitiated()) return;

            if (!initiated)
            {
                Debug.LogWarning("PuppetMaster has not been initiated.", transform);
                return;
            }

            if (ContainsJoint(joint))
            {
                Debug.LogWarning("Joint " + joint.name + " is already used by a Muscle", transform);
                return;
            }

            if (target == null)
            {
                Debug.LogWarning("AddMuscle was called with a null 'target' reference.", transform);
                return;
            }

            if (connectTo == joint.GetComponent<Rigidbody>())
            {
                Debug.LogWarning("ConnectTo is the joint's own Rigidbody, can not add muscle.", transform);
                return;
            }

            if (activeMode == Mode.Disabled)
            {
                Debug.LogWarning("Adding muscles to disabled PuppetMasters is not currently supported.", transform);
                return;
            }

            muscle.joint = joint;
            muscle.target = target;
            muscle.joint.transform.parent = (hierarchyIsFlat || connectTo == null) && !forceTreeHierarchy ? transform : connectTo.transform;

            var animationBlocker = target.GetComponent<AnimationBlocker>();
            if (animationBlocker != null) Destroy(animationBlocker);

            if (forceLayers)
            {
                joint.gameObject.layer = gameObject.layer; //@todo what if collider is on a child gameobject?
                target.gameObject.layer = targetRoot.gameObject.layer;
            }

            if (connectTo != null)
            {
                muscle.target.parent = targetParent;

                Vector3 relativePosition = GetMuscle(connectTo).transform.InverseTransformPoint(muscle.target.position);
                Quaternion relativeRotation = Quaternion.Inverse(GetMuscle(connectTo).transform.rotation) * muscle.target.rotation;

                joint.transform.position = connectTo.transform.TransformPoint(relativePosition);
                joint.transform.rotation = connectTo.transform.rotation * relativeRotation;

                joint.connectedBody = connectTo;

                joint.xMotion = ConfigurableJointMotion.Locked;
                joint.yMotion = ConfigurableJointMotion.Locked;
                joint.zMotion = ConfigurableJointMotion.Locked;
            }

            muscle.Initiate(muscles);

            if (connectTo != null)
            {
                muscle.rigidbody.velocity = connectTo.velocity;
                muscle.rigidbody.angularVelocity = connectTo.angularVelocity;
            }

            // Ignore internal collisions
            if (!internalCollisions)
            {
                for (int i = 0; i < muscles.Length; i++)
                {
                    muscle.IgnoreCollisions(muscles[i], true);
                }
            }

            Array.Resize(ref muscles, muscles.Length + 1);
            muscles[muscles.Length - 1] = muscle;

            // Update angular limit ignoring
            muscle.IgnoreAngularLimits(!angularLimits);

            if (behaviours.Length > 0)
            {
                muscle.broadcaster = muscle.joint.gameObject.AddComponent<MuscleCollisionBroadcaster>();
                muscle.broadcaster.puppetMaster = this;
                muscle.broadcaster.muscleIndex = muscles.Length - 1;
            }

            muscle.jointBreakBroadcaster = muscle.joint.gameObject.AddComponent<JointBreakBroadcaster>();
            muscle.jointBreakBroadcaster.puppetMaster = this;
            muscle.jointBreakBroadcaster.muscleIndex = muscles.Length - 1;

            UpdateHierarchies();
            CheckMassVariation(100f, true);

            foreach (BehaviourBase b in behaviours) b.OnMuscleAdded(muscle);
        }

        // 将 直接把现有的 muscle 通过调整 添加到 muscles 中
        public void AddMuscle(Muscle muscle, bool forceLayers = true)
        {
            muscle.transform.gameObject.SetActive(true);
            muscle.Initiate(muscles);
            System.Array.Resize(ref muscles, muscles.Length + 1);
            muscles[muscles.Length - 1] = muscle;
            muscle.rigidbody.centerOfMass = Vector3.zero;
        }
        // 将对应的muscle 中 muscles 中移除 并 标记active 为false；
        public void RemoveMuscle(Muscle muscle){
			Muscle[] newMuscles = new Muscle[muscles.Length - (muscle.childIndexes.Length + 1)];
			int index = GetMuscleIndex(muscle.joint);
			int added = 0;
			for (int i = 0; i < muscles.Length; i++) {
				if (i != index && !muscles[index].childFlags[i]) {
					newMuscles[added] = muscles[i]; 
					added ++;
				} else {
                    muscles[i].transform.gameObject.SetActive(false);
				}
			}
            muscles = newMuscles;

            //UpdateHierarchies();
            
        }
    }
}
