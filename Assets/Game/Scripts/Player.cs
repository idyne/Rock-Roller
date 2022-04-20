using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using PathCreation;
using FateGames;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [SerializeField] private float minSpeed = 1;
    [SerializeField] private float maxSpeed = 40;
    private PathCreator pathCreator;
    private Transform _transform;
    private bool isGrounded = false;
    private Vector3 velocity;
    private Vector3 desiredPosition = Vector3.zero;
    private float previousA = 0;

    private Vector3 closestPoint { get => pathCreator.path.GetClosestPointOnPath(_transform.position); }
    private void Awake()
    {
        pathCreator = FindObjectOfType<PathCreator>();
        _transform = transform;
    }

    private void SetIsGrounded()
    {
        bool previousIsGrounded = isGrounded;
        isGrounded = Vector3.Distance(closestPoint, _transform.position) < 3f;
        if (!previousIsGrounded && isGrounded)
            Impact();
    }

    private void Impact()
    {
        print("impact");
        Vector3 dir = _transform.position - closestPoint;
        dir = (dir.y > 0 ? dir : -dir).normalized;
        float impactAngle = Vector3.SignedAngle(dir, -velocity, -Vector3.right);
        if (impactAngle < 0)
        {
            velocity.Normalize();
            velocity *= minSpeed;
            print("zero");
        }
        velocity *= Mathf.Sin(impactAngle * Mathf.Deg2Rad);
    }

    private void Update()
    {
        SetIsGrounded();
        if (Input.GetMouseButton(0))
        {
            if (isGrounded)
                velocity += Vector3.forward * 5 * Time.deltaTime;
            else
            {
                velocity = Quaternion.Euler(180 * Time.deltaTime, 0, 0) * velocity;
                print("velocity angle: " + Vector3.SignedAngle(velocity, Vector3.forward, Vector3.right));
                if (Vector3.SignedAngle(velocity, Vector3.forward, Vector3.right) < -75)
                    velocity = Quaternion.Euler(75, 0, 0) * Vector3.forward * velocity.magnitude;
            }
        }
        if (isGrounded)
        {
            Vector3 dir = _transform.position - closestPoint;
            dir = (dir.y > 0 ? dir : -dir).normalized;
            velocity += Vector3.forward * 3 * Time.deltaTime;
            Vector3 direction = Quaternion.Euler(90, 0, 0) * dir;
            float a = Vector3.SignedAngle(dir, Vector3.forward, Vector3.right);
            if (previousA < a && (a > 130 || a < 60))
                velocity = direction * velocity.magnitude;
            else if (previousA <= a)
                velocity = direction * velocity.magnitude;
            previousA = a;
        }
        else
        {
            velocity += Vector3.down * 5 * Time.deltaTime;
            if (Vector3.SignedAngle(velocity, Vector3.forward, Vector3.right) < -75)
                velocity = Quaternion.Euler(75, 0, 0) * Vector3.forward * velocity.magnitude;
        }
    }

    private void LateUpdate()
    {
        LimitVelocity();
        Vector3 dir = _transform.position - closestPoint;
        dir = (dir.y > 0 ? dir : -dir).normalized;
        Debug.DrawRay(closestPoint, dir);
        Debug.DrawRay(_transform.position, velocity);
        desiredPosition = _transform.position + velocity;

        _transform.position = Vector3.Lerp(_transform.position, desiredPosition, Time.deltaTime * velocity.magnitude);
        if (isGrounded && desiredPosition.y < (pathCreator.path.GetClosestPointOnPath(desiredPosition) + dir * 2.0f).y)
        {
            _transform.position = Vector3.Lerp(_transform.position, closestPoint + dir * 2, Time.deltaTime * velocity.magnitude);
        }
    }

    private void LimitVelocity()
    {
        if (velocity.magnitude > maxSpeed)
            velocity = velocity.normalized * maxSpeed;
        else if (velocity.magnitude < minSpeed)
            velocity = velocity.normalized * minSpeed;
    }
    private float GetSignedAngle()
    {
        Quaternion rotation = pathCreator.path.GetRotationAtDistance(pathCreator.path.GetClosestDistanceAlongPath(_transform.position));
        float a = -(rotation.eulerAngles.x - 360) % 360;
        float b = -(rotation.eulerAngles.x + 360) % 360;
        float angle;
        if (Mathf.Abs(a) < Mathf.Abs(b))
            angle = a;
        else angle = b;
        return angle;
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Road"))
        {
            isGrounded = true;
            impactAngle = Vector3.SignedAngle(pathCreator.path.GetNormalAtDistance(pathCreator.path.GetClosestDistanceAlongPath(_transform.position)), -velocity.normalized, -Vector3.right);
            if (impactAngle < 0)
                velocity = Vector3.zero;
            velocity *= Mathf.Sin(impactAngle * Mathf.Deg2Rad);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Road"))
        {
            isGrounded = false;
        }
    }
    */

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(desiredPosition, 0.1f);
    }
}
