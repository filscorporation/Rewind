using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Source
{
    public class Mine : MonoBehaviour, IRewindDataProvider
    {
        [SerializeField] private GameObject mine;
        [SerializeField] private GameObject explosionPrefab;
        private bool exploded = false;
        private ProviderState providerState = ProviderState.Writing;

        private void Start()
        {
            TimeDataManager.Instance.AddToData(this);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            Debug.Log("Collided");
            if (providerState == ProviderState.Reading)
                return;
            
            PlayerController player = other.collider.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                mine.SetActive(true);
                player.Freeze();
                StartCoroutine(Explode());
            }
        }

        private IEnumerator Explode()
        {
            yield return new WaitForSeconds(0.5f);
            exploded = true;
            PlayerController.Instance.Die();
            Destroy(Instantiate(explosionPrefab, transform.position, Quaternion.identity), 2f);
        }

        public RewindData GetData()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["exploded"] = exploded;
            
            return new RewindData { Data = data };
        }

        public void ApplyData(RewindData data)
        {
            bool oldExploded = exploded;
            exploded = (bool)data.Data["exploded"];
            if (oldExploded && !exploded)
            {
                mine.SetActive(false);
                StopAllCoroutines();
                PlayerController.Instance.Unfreeze();
            }
        }

        public void SetState(ProviderState state)
        {
            providerState = state;
        }
    }
}