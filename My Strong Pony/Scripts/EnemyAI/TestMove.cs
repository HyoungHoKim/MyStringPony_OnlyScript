using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour
{
	private Transform target;
	private float dist;
	private string[] tags = { "BLUE", "BLUEALPHA" };
	GameObject[] enemyObjs = { };

	void Start()
	{
		Debug.Log("start haptic");
	}

	void Update()
	{
		if (target != null)
		{
			Debug.DrawLine(transform.position, target.position, Color.yellow);
		}

		float closestDistSqr = Mathf.Infinity;
		Transform closestEnemy = null;

		foreach (string tag in tags)
		{
			enemyObjs = GameObject.FindGameObjectsWithTag(tag);

			foreach (GameObject EnemyObj in enemyObjs)
			{
				Vector3 objectPos = EnemyObj.transform.position;
				dist = (objectPos - transform.position).sqrMagnitude;

				if (dist < closestDistSqr)
				{
					closestDistSqr = dist;
					closestEnemy = EnemyObj.transform;
				}

			}
			target = closestEnemy;
		}

	}
}
