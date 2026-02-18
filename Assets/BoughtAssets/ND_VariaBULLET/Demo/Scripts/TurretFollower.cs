#region Script Synopsis
    //Rotates a top-level object, pointing at a target object
#endregion

using System;
using UnityEngine;

namespace ND_VariaBULLET.Demo
{
	public class TurretFollower : MonoBehaviour
	{
		public Transform TargetDirect;
		public string TargetFromTag;

		[Range(1, 50)]
		public int TargetingSpeed = 10;

		private Transform targetLock;

		private void Start()
		{
			if (TargetDirect != null)
				targetLock = TargetDirect;
			else
			{
				if (!String.IsNullOrEmpty(TargetFromTag))
					targetLock = GameObject.FindGameObjectWithTag(TargetFromTag).transform;
			}
		}

		void Update()
		{
			if (targetLock == null)
				return;

			Vector2 direction = targetLock.position - transform.position;

			transform.rotation = CalcObject.VectorToRotationSlerp(
				transform.rotation, direction, TargetingSpeed
			);
		}
	}
}