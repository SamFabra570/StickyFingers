using System.Collections;
using UnityEngine;

public class Smoke : MonoBehaviour
{
    public ParticleSystem blowSmoke;
    public ParticleSystem smokeCloud;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blowSmoke = GetComponent<ParticleSystem>();
        smokeCloud = GetComponent<ParticleSystem>();

        var smokeMain = blowSmoke.main;
        var cloudMain = smokeCloud.main;
        
        smokeMain.stopAction = ParticleSystemStopAction.Destroy;
        cloudMain.stopAction = ParticleSystemStopAction.Destroy;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BlowSmoke(GameObject emitter, GameObject cloudPos, float waitTime)
    {
        blowSmoke.transform.SetPositionAndRotation(emitter.transform.position, emitter.transform.rotation);
        
        blowSmoke.Play();
        Debug.Log("Set cloud pos, NO SPAWN YET");
        smokeCloud.transform.SetPositionAndRotation(cloudPos.transform.position, cloudPos.transform.rotation);
        //StartCoroutine(SpawnCloud(cloudPos, waitTime));
    }
    
    private IEnumerator SpawnCloud(GameObject cloudPos, float time)
    {
        yield return new WaitForSeconds(time);

        Debug.Log("SPAWN DA CLOUD");
        smokeCloud.Play();
    }
}
