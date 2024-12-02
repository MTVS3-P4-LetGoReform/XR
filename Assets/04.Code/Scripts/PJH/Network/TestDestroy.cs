using System;
using Fusion;
using UnityEngine;

public class TestDestroy : NetworkBehaviour
{
    private void Start()
    {
        if (!HasStateAuthority)
        {
            Destroy(gameObject);
        }
    }
}
