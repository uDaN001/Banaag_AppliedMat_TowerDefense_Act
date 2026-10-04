using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static List<Enemy> ActiveEnemies = new List<Enemy>();

    void Awake() => ActiveEnemies.Clear();

    public static void Register(Enemy enemy) => ActiveEnemies.Add(enemy);
    public static void Unregister(Enemy enemy) => ActiveEnemies.Remove(enemy);
}