using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;
    public float spawnTime = 0.5f;
    public float ballLife = 5;
    bool canSpawn;
    // Start is called before the first frame update
    void Start()
    {
        canSpawn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canSpawn)
            StartCoroutine(spawnball());
    }

    IEnumerator spawnball()
    {
        canSpawn = false;

        GameObject ball = Instantiate(ballPrefab, this.transform) as GameObject;
        ball.GetComponent<Particle3D>().AddForce(new Vector3(Random.Range(-200, 200), 0.0f, 0.0f));
        Destroy(ball, ballLife);
        yield return new WaitForSeconds(spawnTime);

        canSpawn = true;
    }
}
