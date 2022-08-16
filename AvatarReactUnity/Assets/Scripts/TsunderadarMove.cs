﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TsunderadarMove : MonoBehaviour
{
    public string message;
    public Vector3 startPoint;
    public Vector3 startAngle;
    public Vector3 DestinationPoint;
    public Vector3 interactionTargetPoint;
    public Vector3 stanbyPoint;

    enum TsundereState {
        Stanby,
        Start,
        FirstStep,
        SecondStep,
        Finish,
    };
    TsundereState state;
    void nextState() {
        if (state == TsundereState.Stanby) state = TsundereState.Start;
        else if (state == TsundereState.Start) state = TsundereState.FirstStep;
        else if (state == TsundereState.FirstStep) state = TsundereState.SecondStep;
        else if (state == TsundereState.SecondStep) state = TsundereState.Finish;
        else if (state == TsundereState.Finish) state = TsundereState.Stanby;
    }
    string keyword = "Next";
    bool isFinish;
    bool isfirstCalFirstInteraction;
    bool isfirstCalSecondInteraction;
    float firstStepTime = 6.0;
    float secondStepTime = 8.0;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        this.gameObject.transform.position = startPoint;
        this.gameObject.transform.rotation = Quaternion.Euler(startAngle.x, startAngle.y, startAngle.z);
        state = TsundereState.Stanby;
        isFinish = true;
        isfirstCalFirstInteraction = true;
        isfirstCalSecondInteraction = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (message == keyword && isFinish)
        {
            nextState();
        }

        if (state == TsundereState.Start) 
        {
            if (goDestination(this.gameObject, DestinationPoint, interactionTargetPoint)) isFinish = false;
            else isFinish = false;
        }
        else if (state == TsundereState.FirstStep)
        {
            if (firstInteraction()) isFinish = false;
            else isFinish = true;
        }
        else if (state == TsundereState.SecondStep)
        {
            if (secondInteraction()) isFinish = false;
            else isFinish = true;
        }
        else if (state == TsundereState.Finish)
        {
            if (goDestination(this.gameObject, startPoint, stanbyPoint)) isFinish = false;
            else 
            {
                isFinish = false;
                nextState();
            }
        }
    }

    private bool goDestination(GameObject target, Vector3 to, Vector3 interactDirection)
    {
        Debug.Log("firstRotateGoDestination : " + firstRotateGoDestination);
        if (firstRotateGoDestination || headDestination(target, to))
        {
            firstRotateGoDestination = true;
            if (firstMoveGoDestination || moveToDestination(target, to))
            {
                firstMoveGoDestination = true;
                if (seconRotateGoDistiantion || headDestination(target, interactDirection))
                {
                    firstRotateGoDestination = false;
                    firstMoveGoDestination = false;
                    seconRotateGoDistiantion = false;
                    animator.SetBool("isAutoWalk", false);
                    return true;
                }
            }
        }
        animator.SetBool("isAutoWalk", true);
        return false;
    }

    private bool headDestination(GameObject target, Vector3 to)
    {
        float angle = Vector3.Angle(target.transform.position - to, target.transform.forward);
        if (Mathf.Abs(previousAngle - angle) < 0.01f)
        {
            previousAngle = 0.0f;
            return true;
        }
        else
        {
            Vector3 targetDir = to - target.transform.position;
            if (targetDir != Vector3.zero)
                target.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(target.transform.forward, targetDir, rotateSpeed, 10.0F));
            else
                target.transform.rotation = Quaternion.identity;
        }
        previousAngle = angle;
        return false;
    }

    private bool moveToDestination(GameObject target, Vector3 to)
    {
        if (Vector3.Distance(target.transform.position, to) < DestinationDistance)
            return true;
        else
        {
            target.transform.position = Vector3.MoveTowards(target.transform.position, to, moveSpeed);
            return false;
        }
    }

    private bool firstInteraction()
    {
        if (isfirstCalFirstInteraction)
        {
            startTime = Time.time;
            isfirstCalFirstInteraction = false;
        }

        if (Time.time - startTime < firstStepTime)
        {
            animator.SetBool("isFirstStep", true);
            return false;
        }
        else
        {
            isfirstCalFirstInteraction = true;
            animator.SetBool("isFirstStep", false);
            return true;
        }
    }
    private bool secondInteraction()
    {
        if (isfirstCalSecondInteraction)
        {
            startTime = Time.time;
            isfirstCalSecondInteraction = false;
        }

        if (Time.time - startTime < secondStepTime)
        {
            animator.SetBool("isSecondStep", true);
            return false;
        }
        else
        {
            isfirstCalSecondInteraction = true;
            animator.SetBool("isSecondStep", false);
            return true;
        }
    }

}
