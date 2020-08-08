using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Source
{
    public class MineSpawner : MonoBehaviour
    {
        [SerializeField] private List<Transform> positions;
        [SerializeField] private int amount;
        [SerializeField] private GameObject minePrefab;

        private void Start()
        {
            Spawn();
        }

        private void Spawn()
        {
            foreach (Transform position in positions.Shuffle().Take(amount))
            {
                Instantiate(minePrefab, position.position, Quaternion.identity);
            }
        }
    }
}