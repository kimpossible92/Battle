using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loadSpawn : MonoBehaviour
{
    [SerializeField] EnemyCharmander enemy;
    [SerializeField] EnemyCharmander[] enemies;
    [SerializeField] Transform[] transforms;
    int[] Vs1 = { 0, 4, 8};
    int num;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        enemies = FindObjectsOfType<EnemyCharmander>();
        if (enemies.Length <= 0)
        {
            num = Random.Range(0, Vs1.Length);
            var posts = Instantiate(enemy, transforms[Vs1[num]].position, Quaternion.identity);
            posts.waypoints[0] = transforms[num];
            posts.waypoints[1] = transforms[num+1];
            posts.waypoints[2] = transforms[num+2];
            posts.waypoints[3] = transforms[num+3];
        }
    }
}
