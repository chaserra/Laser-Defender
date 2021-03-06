﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathing : MonoBehaviour {
    //Config Parameters
    WaveConfig waveConfig;
    List<Transform> waypoints;
    int waypointIndex = 0;

    //Cached References

	// Use this for initialization
	void Start () {
        waypoints = waveConfig.GetWaypoints();
        transform.position = waypoints[waypointIndex].transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        Move();
    }

    public void SetWaveConfig(WaveConfig waveConfig) {
        this.waveConfig = waveConfig;
    }

    private void Move() {
        if (waypointIndex <= waypoints.Count - 1) {
            var targetPosition = waypoints[waypointIndex].transform.position;
            var movementThisFrame = waveConfig.GetMoveSpeed() * Time.deltaTime;
            transform.position = Vector2.MoveTowards
                (transform.position, targetPosition, movementThisFrame);

            if (transform.position == targetPosition) {
                waypointIndex++;
                if(gameObject.tag == "Boss") {
                    if(waypointIndex == waypoints.Count) {
                        waypointIndex = 0;
                    }
                }
            }
        } else {
            Destroy(gameObject);
        }
    }
}
