using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SnowmanControl : MonoBehaviour
{
    private enum ControlMode : byte
    {
        Launching,
        Sledding,
        Flying,
        Sliding
    }


    [SerializeField] private ControlMode controlMode = ControlMode.Launching;
    [SerializeField] private Transform cosmeticsRoot = null;

    // Vector2.zero is a sentinel value for `no collisions this frame`.
    private Vector2 priorAggregateNormal;
    private Vector2 aggregateNormal;

    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
            aggregateNormal += contact.normal;
    }
    private void FixedUpdate()
    {
        if (aggregateNormal != Vector2.zero)
            priorAggregateNormal = aggregateNormal.normalized;
        else
            priorAggregateNormal = Vector2.zero;
        aggregateNormal = Vector2.zero;
    }

    private void Update()
    {
        if (priorAggregateNormal != Vector2.zero)
        {
            aggregateNormal.Normalize();
            switch (controlMode)
            {
                case ControlMode.Sledding: FixedUpdateSledding(); break;
            }
        }
    }
    private void FixedUpdateSledding()
    {
        cosmeticsRoot.up = priorAggregateNormal;
    }


    // Start is called before the first frame update
    private void Awake()
    {
        aggregateNormal = Vector2.zero;
    }
}
