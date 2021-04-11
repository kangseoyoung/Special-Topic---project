using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class BasicBallAgent : Agent
{
    private Rigidbody ballRb;
    public GameObject[] target;
    private Vector3 beforePos;
    private float beforeDistance = 9999;
    private int targetIdx = -1;
    

    public override void InitializeAgent()
    {
        ballRb = GetComponent<Rigidbody>();
        MakeRandomTarget();
    }

    public override void CollectObservations()
    {
        AddVectorObs(gameObject.transform.position);
        AddVectorObs(target[targetIdx].transform.position);

        AddVectorObs(ballRb.velocity.x);
        AddVectorObs(ballRb.velocity.z);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        Vector3 velocity = Vector3.zero;
        velocity.x = vectorAction[0];
        velocity.z = vectorAction[1];

        ballRb.AddForce(velocity * 10);

        float distanceToTarget = Vector3.Distance(gameObject.transform.position, target[targetIdx].transform.position);

        if (beforeDistance > distanceToTarget)
        {
            AddReward(0.001f);
        }

        beforeDistance = distanceToTarget;

        if (distanceToTarget < 1.5f)
        {
            AddReward(1.0f);
            MakeRandomTarget();
            Done();
        }

        if (gameObject.transform.position.y < 0)
        {
            AddReward(-1f);
            Done();
        }
    }

    public override void AgentReset()
    {
        if (this.transform.position.y < 0)
        {
            this.transform.position = new Vector3(0, 0, 0);
            this.ballRb.angularVelocity = Vector3.zero;
            this.ballRb.velocity = Vector3.zero;
        }
    }

    void MakeRandomTarget()
    {
        beforeDistance = 9999;
        if (targetIdx != -1)
        {
            beforePos = target[targetIdx].transform.position;
            beforePos.y = 0.5f;
            target[targetIdx].transform.position = beforePos;
            target[targetIdx].transform.localScale = new Vector3(1, 1, 1);
        }

        targetIdx = Random.Range(0, 4);
        beforePos = target[targetIdx].transform.position;
        beforePos.y = 1f;

        target[targetIdx].transform.position = beforePos;
        target[targetIdx].transform.localScale = new Vector3(1, 2, 1);
    }

    private void FixedUpdate()
    {
        AddReward(-0.001f);
    }
}
