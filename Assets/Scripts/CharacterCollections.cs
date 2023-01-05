using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCollections : MonoBehaviour
{
    public GameObject[] Friendly;
    public GameObject[] Enemies;
    public GameObject[] Obstacles;

    public GameObject GetRandomFriendly()
    {
        return Friendly[Random.Range(0, Friendly.Length)];
    }

    public GameObject GetRandomEnemy()
    {
        return Enemies[Random.Range(0, Enemies.Length)];
    }

    public GameObject GetRandomObstacle()
    {
        return Obstacles[Random.Range(0, Obstacles.Length)];
    }
}
