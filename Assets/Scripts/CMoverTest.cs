using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMoverTest : MonoBehaviour
{
    public CurveMover2D mover;
    public Transform toFollow;

    private void FixedUpdate()
    {
        mover.MoveTo(toFollow.position.x);
    }
}
