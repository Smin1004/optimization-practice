using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 target;
    [SerializeField] private float moveSpeed;

    private void Update()
    {
        if (transform.position == target) { DieDestroy(); return; }
        transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * moveSpeed);
    }

    private void DieDestroy()
    {
        ObjectPool.ReturnObject(this);
    }
}
