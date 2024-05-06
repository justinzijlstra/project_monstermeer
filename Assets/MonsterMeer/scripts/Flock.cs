using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Sockets;
using UnityEditor;
using UnityEngine;

public class Flock : MonoBehaviour
{

    public Vector2 speedRange;
    float Speed;
    float RotationSpeed = 4.0f;
    float NeighbourDistance = 2.0f;
    bool Turning = false;

    void Start()
    {
        Speed = UnityEngine.Random.Range(speedRange.x, speedRange.y);
    }

    void Update()
    {

        Vector3 goalPos = GlobalFlock.GoalPos;

        if (Vector3.Distance(transform.position, goalPos) >= GlobalFlock.tankSize)
        {
            Turning = true;
        }
        else 
        {
            Turning = false;
        }
        if (Turning) 
        {
            Vector3 direction = goalPos - transform.position; ;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), RotationSpeed * Time.deltaTime);


            Speed = UnityEngine.Random.Range(speedRange.x,speedRange.y);
        }
        else 
        {
            if (UnityEngine.Random.Range(0, 5) < 1)
            {
                ApplyRules();
            }
        }

        transform.Translate(0, 0, Time.deltaTime * Speed);
    }

    private void ApplyRules()
    {

        Vector3 goalPos = GlobalFlock.GoalPos;

        GameObject[] fishes; // gos  = fishes 
        fishes = GlobalFlock.AllFish;

        Vector3 groupCentre = goalPos; //group center
        Vector3 groupAvoid = Vector3.zero; // avoid group
        float groupSpeed = 0.1f;

        float dist;

        int groupSize = 0;

        foreach (GameObject fish in fishes) // fish in fishes
        {
            if (fish != gameObject)
            {
                dist = Vector3.Distance(fish.transform.position, transform.position);
                if (dist <= NeighbourDistance)
                {
                    groupCentre += fish.transform.position;
                    groupSize++;
                    if (dist < 1.0f) 
                    {
                        groupAvoid = groupAvoid + (transform.position - fish.transform.position);
                    }

                    Flock anotherFlock = fish.GetComponent<Flock>();
                    groupSpeed = groupSpeed + anotherFlock.Speed;
                }
            }
        }

        if (groupSize > 0) 
        {
            groupCentre = groupCentre / groupSize + (goalPos - transform.position);
            Speed = groupSpeed / groupSize;

            Vector3 direction = (groupCentre + groupAvoid) - transform.position;
            if (direction != goalPos) 
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), RotationSpeed * Time.deltaTime);

            }
        }

    }
}
