using System.Collections;
using UnityEngine;

public class Smoke : MonoBehaviour
{
    public ParticleSystem blowSmoke;
    public ParticleSystem smokeCloud;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BlowSmoke(GameObject emitter, GameObject cloudPos, float waitTime)
    {
        blowSmoke.transform.SetPositionAndRotation(emitter.transform.position, emitter.transform.rotation);
        blowSmoke.Play();
        StartCoroutine(SpawnCloud(cloudPos, waitTime));
    }
    
    private IEnumerator SpawnCloud(GameObject pos, float time)
    {
        smokeCloud.transform.SetPositionAndRotation(pos.transform.position, pos.transform.rotation);
        yield return new WaitForSeconds(time);

        Debug.Log("Spawn cloud");
        smokeCloud.Play();
    }
}
