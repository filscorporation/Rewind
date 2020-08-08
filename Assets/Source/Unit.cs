using System.Collections.Generic;
using UnityEngine;

namespace Source
{
    public abstract class Unit : MonoBehaviour, IShootTarget, IRewindDataProvider
    {
        [SerializeField] private GameObject bloodPrefab;
        
        protected abstract float MaxHealth { get; }
        protected float health;
        protected ProviderState providerState = ProviderState.Writing;
        
        private void Start()
        {
            health = MaxHealth;
            
            TimeDataManager.Instance.AddToData(this);
        }

        protected virtual void Initialize()
        {
            
        }

        public void TakeDamage(float damage)
        {
            health = Mathf.Max(0, health - damage);
            if (Mathf.Approximately(health, 0))
            {
                Die(true);
            }
        }

        protected virtual void Die(bool blood = false)
        {
            if (blood)
            {
                GameObject go = Instantiate(bloodPrefab, transform.position, Quaternion.identity);
                float angle = Mathf.Atan2(
                    PlayerController.Instance.transform.position.y - go.transform.position.y,
                    PlayerController.Instance.transform.position.x - go.transform.position.x);
                angle *= (180f / Mathf.PI);
                go.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
                Destroy(go, 1f);
            }
        }
        
        public virtual RewindData GetData()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["position"] = transform.position;
            data["rotationZ"] = transform.eulerAngles.z;
            data["health"] = health;
            
            return new RewindData { Data = data };
        }

        public virtual void ApplyData(RewindData data)
        {
            transform.position = (Vector3)data.Data["position"];
            transform.eulerAngles = new Vector3(0, 0, (float)data.Data["rotationZ"]);
            float oldHealth = health;
            health = (float)data.Data["health"];
            if (Mathf.Approximately(oldHealth, 0) && health > 0)
            {
                gameObject.SetActive(true);
            }
        }

        public void SetState(ProviderState state)
        {
            providerState = state;
        }
    }
}