using System;
using UnityEngine;

namespace Source
{
    public class Enemy : Unit
    {
        [SerializeField] protected float maxHealth = 10f;
        [SerializeField] private float reactionTime = 1f;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private Transform gun;
        [SerializeField] private GameObject reactionPrefab;
        [SerializeField] private GameObject shotPrefab;
        private float reactionTimer = 100f;
        private Reaction uiReaction;
        private bool didShoot = false;

        protected override float MaxHealth => maxHealth;

        protected override void Initialize()
        {
            base.Initialize();

            reactionTimer = reactionTime;
        }

        private void Update()
        {
            if (didShoot)
                return;
            
            if (providerState == ProviderState.Writing)
            {
                if (CanSeePlayer())
                {
                    RotateToPlayer();
                    reactionTimer = Mathf.Max(reactionTimer - Time.deltaTime, 0);
                    if (Mathf.Approximately(reactionTimer, 0))
                    {
                        Shoot();
                        Destroy(uiReaction.gameObject);
                        uiReaction = null;
                        return;
                    }
                }
                else
                {
                    reactionTimer = Mathf.Min(reactionTimer + Time.deltaTime, reactionTime);
                }
            }

            if (!Mathf.Approximately(reactionTime, reactionTimer))
            {
                if (uiReaction == null)
                {
                    uiReaction = Instantiate(reactionPrefab, Vector3.zero, Quaternion.identity, gun).GetComponent<Reaction>();
                    uiReaction.Initialize();
                }
                uiReaction.SetReaction(reactionTimer / reactionTime);
            }
            else
            {
                if (uiReaction != null)
                {
                    Destroy(uiReaction.gameObject);
                    uiReaction = null;
                }
            }
        }

        private bool CanSeePlayer()
        {
            RaycastHit2D hit = Physics2D.Linecast(
                transform.position, 
                PlayerController.Instance.transform.position,
                layerMask
                );
            return hit.collider == null;
        }
        
        private void RotateToPlayer()
        {
            Vector3 target = PlayerController.Instance.transform.position;
            float angle = Mathf.Atan2(target.y - transform.position.y, target.x - transform.position.x);
            angle *= (180f / Mathf.PI);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private void Shoot()
        {
            didShoot = true;
            GameObject shot = Instantiate(shotPrefab, gun.position, gun.rotation);
            shot.layer = gameObject.layer;
            Destroy(shot, 2f);
            PlayerController.Instance.Die(transform.position);
        }

        protected override void Die(bool blood = false)
        {
            base.Die(blood);
            
            gameObject.SetActive(false);
        }

        public override RewindData GetData()
        {
            RewindData data = base.GetData();

            data.Data["reaction"] = reactionTimer;
            data.Data["didshoot"] = didShoot;
            
            return data;
        }

        public override void ApplyData(RewindData data)
        {
            base.ApplyData(data);
            
            reactionTimer = (float)data.Data["reaction"];
            didShoot = (bool)data.Data["didshoot"];
        }
    }
}