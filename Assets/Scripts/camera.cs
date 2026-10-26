using System.Linq;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class camera : MonoBehaviour
{
    public player target;
    public float easing = 0.15f;
    public float focusRadius = 5f;
    public float moveOffset = 0f;
    public GameObject[] focusPoints;
    private GameObject closestFocusPoint1;
    private Vector2 fociDiff;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CheckFoci();
    }

    // Update is called once per frame
    void Update()
    {
        CheckFoci();
    }

    void FixedUpdate()
    {

        Vector3 move;
        if (closestFocusPoint1 != null && Vector2.Distance(closestFocusPoint1.transform.position, target.transform.position) <= focusRadius)
        {
            move = closestFocusPoint1.transform.position - transform.position;
            
        } else
        {
            move = target.transform.position - transform.position;

            if (target.isMoving())
            {
                float newMoveOffset = moveOffset * Time.deltaTime * 2.5f;
                newMoveOffset *= target.GetComponent<Rigidbody2D>().linearVelocityX * Mathf.Abs(newMoveOffset);
            } else
            {
                move += new Vector3(target.getMoveDirection().x, target.getMoveDirection().y, 0) * moveOffset;
            }
        }

        move *= easing;

        transform.position += move;
    }

    void CheckFoci() {
        foreach (GameObject focusPoint in focusPoints)
        {
            if (focusPoint != null)
            {
                if (closestFocusPoint1 == null)
                    closestFocusPoint1 = focusPoint;
                else if (Vector2.Distance(target.transform.position, focusPoint.transform.position) < Vector2.Distance(target.transform.position, closestFocusPoint1.transform.position))
                    closestFocusPoint1 = focusPoint;
            }
        }
    }
}
