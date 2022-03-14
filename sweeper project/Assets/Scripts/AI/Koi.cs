using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Koi : MonoBehaviour
{
    [Header("Assignables")]
    public KoiManager manager;
    public Transform head;
    public Transform[] pieces;

    [Header("Fish Settings")]
    public float swimSpeed = 1f;
    public float rotationSpeed = 2f;
    public float animationSpeed = 5f;
    public float fastMultiplier = 5f;
    public float multiplier = 5;
    public float stepModifier = 0.2f;
    public float maxRange = 2f;
    private float sine = 0;
    private float frequency = 0;
    private float step;
    private float currentSwimSpeed = 1f;

    private bool mouseEntered = false;

    public Transform destination;

    private void Start()
    {
        destination = manager.GetPosition();
    }

    private void Update()
    {
        frequency += Time.deltaTime * currentSwimSpeed * animationSpeed;
        sine = multiplier * Mathf.Sin(frequency);

        foreach(Transform piece in pieces)
        {
            piece.transform.localEulerAngles = Vector3.up * sine;
        }

        // calculate swim speed
        if (mouseEntered)
        {
            currentSwimSpeed = swimSpeed * fastMultiplier;
        }
        else
        {
            if (currentSwimSpeed > swimSpeed)
            {
                currentSwimSpeed -= Time.deltaTime;
            }
        }

        // Rotate towards destination
        Vector3 targetDir = destination.position - transform.position;
        step = stepModifier * Time.deltaTime * currentSwimSpeed * rotationSpeed;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);

        // Move forward
        transform.position += transform.forward * currentSwimSpeed * Time.deltaTime;

        // Update destination
        if (Vector3.Distance(transform.position, destination.position) < maxRange)
        {
            destination = manager.GetPosition();
        }
    }

    private void OnMouseEnter()
    {
        mouseEntered = true;
    }

    private void OnMouseExit()
    {
        mouseEntered = false;
    }
}
