using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgent : Agent
{
    Rigidbody rBody;
    public Transform target;
    public float forceMultiplier = 10;
    public float cutOffDistance = 1.42f; //diagonal of size 1 square
    public override void CollectObservations(VectorSensor sensor)
    {
        //base.CollectObservations(sensor);
        //player position (3 floats)
        //target position (3 floats)
        //player velocity.x, velocity.z) (2 floats)  => 8 floats

        sensor.AddObservation(this.transform.localPosition); //3
        sensor.AddObservation(target.localPosition); //+3=6
        sensor.AddObservation(rBody.velocity.x); //+1=7
        sensor.AddObservation(rBody.velocity.z); //+1=8
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //Act
        Vector3 controlForce = Vector3.zero;
        controlForce.x=actions.ContinuousActions[0];  //[-1,1]
        controlForce.z = actions.ContinuousActions[1];//[-1,1]
        rBody.AddForce(controlForce * forceMultiplier);

        //Deal with Rewards
        if (Vector3.Distance(this.transform.localPosition, target.localPosition) < cutOffDistance)
        {
            //episode ends successfully
            SetReward(1.0f);
            EndEpisode();
        }

        if (this.transform.localPosition.y < 0)
        {
            //fell off platform => end episode
            EndEpisode();
        }


    }

    public override void OnEpisodeBegin()
    {
        //reset player
        if (this.transform.position.y < 0)
        {
            //fell off the cliff
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        //reset the target
        target.localPosition = new Vector3(-4 + 8 * Random.value, 0.5f, -4 + 8 * Random.value);
    }

    // Start is called before the first frame update
    void Start()
    {
        rBody = this.GetComponent<Rigidbody>();
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //base.Heuristic(actionsOut);
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");

    }
}
