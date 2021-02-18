using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocker : Kinematic
{
    public bool avoidObstacles = false;
    public GameObject myCohereTarget;
    BlendedSteering mySteering;
    GameObject[] kBirds;

    void Start()
    {
        // Separate from other birds
        Separation separate = new Separation();
        separate.character = this;
        GameObject[] goBirds = GameObject.FindGameObjectsWithTag("bird");
        kBirds = new GameObject[goBirds.Length-1];
        int j = 0;
        for (int i=0; i<goBirds.Length-1; i++)
        {
            if (goBirds[i] == this)
            {
                continue;
            }
            kBirds[j++] = goBirds[i];
        }
        separate.targets = kBirds;

        // Cohere to center of mass - ez mode
        Seek cohere = new Seek();
        cohere.character = this;
        cohere.target = myCohereTarget;

        Pursue pursue = new Pursue();
        pursue.character = this;
        pursue.target = myTarget;

        // look where center of mass is going - ez mode
        LookWhereGoing myRotateType = new LookWhereGoing();
        myRotateType.character = this;

        mySteering = new BlendedSteering();
        mySteering.behaviors = new BehaviorAndWeight[4];
        mySteering.behaviors[0] = new BehaviorAndWeight();
        mySteering.behaviors[0].behavior = separate;
        mySteering.behaviors[0].weight = .2f;
        mySteering.behaviors[1] = new BehaviorAndWeight();
        mySteering.behaviors[1].behavior = cohere;
        mySteering.behaviors[1].weight = .3f;
        mySteering.behaviors[2] = new BehaviorAndWeight();
        mySteering.behaviors[2].behavior = pursue;
        mySteering.behaviors[2].weight = .5f;
        mySteering.behaviors[3] = new BehaviorAndWeight();
        mySteering.behaviors[3].behavior = myRotateType;
        mySteering.behaviors[3].weight = 1f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        Vector3 centerOfMass = Vector3.zero;
        Vector3 flockVelocity = Vector3.zero;
        foreach (GameObject bird in kBirds)
        {
            centerOfMass += bird.transform.position;
            flockVelocity += bird.GetComponent<Kinematic>().linearVelocity;
        }
        centerOfMass /= kBirds.Length;
        flockVelocity /= kBirds.Length;
        myCohereTarget.transform.position = centerOfMass;
        myCohereTarget.GetComponent<Kinematic>().linearVelocity = flockVelocity;

        steeringUpdate = new SteeringOutput();
        steeringUpdate = mySteering.getSteering();
        base.Update();
    }
}
