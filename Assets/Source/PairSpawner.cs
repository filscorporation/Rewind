using UnityEngine;
using Random = System.Random;

namespace Source
{
    public class PairSpawner : MonoBehaviour
    {
        [SerializeField] private Transform position1;
        [SerializeField] private Transform position2;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private GameObject npcPrefab;
        
        private void Start()
        {
            Spawn();
        }

        private void Spawn()
        {
            Random rnd = new Random();

            if (rnd.Next(2) == 1)
            {
                Instantiate(enemyPrefab, position1.position, Quaternion.identity);
                Instantiate(npcPrefab, position2.position, Quaternion.identity);
            }
            else
            {
                Instantiate(enemyPrefab, position2.position, Quaternion.identity);
                Instantiate(npcPrefab, position1.position, Quaternion.identity);
            }
        }
    }
}