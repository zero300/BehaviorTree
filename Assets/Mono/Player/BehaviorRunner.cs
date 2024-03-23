using System.Collections;
using System.Collections.Generic;
using Behavior;
using Unity.VisualScripting;
using UnityEngine;

public class BehaviorRunner : MonoBehaviour
{
    public BehaviorTree tree;
    public float MainTainTime = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        tree = tree.Clone();
        tree.Bind(); // 根據教學 這裡有綁定AI的東西
    }

    // Update is called once per frame
    void Update()
    {
        tree.Update();
    }
}
