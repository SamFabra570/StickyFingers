using System.Collections;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    public GameObject smokeEmitter;
    public GameObject cloudPos;
    public GameObject smokePrefab;
    public GameObject smokeCloudPrefab;
    public float cloudWaitTime = 2.5f;

    void Awake()
    {
        Instance = this;
    }

    //Instantiate smoke effects, call blowsmoke
    public void SpawnSmoke()
    {
        GameObject smoke = Instantiate(smokePrefab);
        //GameObject cloud = Instantiate(smokeCloudPrefab);
        Smoke smokeScript = smoke.GetComponent<Smoke>();
        smokeScript.BlowSmoke(smokeEmitter, cloudPos, cloudWaitTime);
    }

    
}
