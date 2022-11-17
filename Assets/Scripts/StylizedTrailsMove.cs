using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StylizedTrailsMove : MonoBehaviour
{

    [SerializeField]
    [Range(0f, 10f)]
    private float speed = 1;
    [SerializeField]
    [Range(0f, 10f)]
    private float radius = 1;

    [SerializeField]
    private float offset = 0f;

    [Range(-180f, 180f)]
    private float runningTime = 0;
    private Vector3 newPos = new Vector3();

    void Update()
    {
        runningTime = Time.time * speed;
        float x = radius * Mathf.Cos(runningTime + offset);
        float y = transform.position.y;
        float z = radius * Mathf.Sin(runningTime + offset);

        newPos = new Vector3(x, y, z);
        this.transform.position = newPos + this.transform.parent.position;
    }
}
