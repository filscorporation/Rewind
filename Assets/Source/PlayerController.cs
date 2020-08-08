using System.Collections.Generic;
using UnityEngine;

namespace Source
{
    public class PlayerController : MonoBehaviour, IRewindDataProvider
    {
        public static PlayerController Instance;
        
        [SerializeField] [Range(0, 10f)] private float speed = 1f;
        [SerializeField] private GameObject shotPrefab;
        [SerializeField] private GameObject bloodPrefab;

        private Weapon weapon;
        private float shootCooldown = 0;

        private bool canControl = true;
        private bool isDead = false;
        private ProviderState providerState = ProviderState.Writing;

        private void Start()
        {
            weapon = GetComponentInChildren<Weapon>();
            Instance = this;
            
            TimeDataManager.Instance.AddToData(this);
        }

        private void Update()
        {
            if (canControl && !isDead && providerState == ProviderState.Writing)
            {
                Control();
                Rotate();
            }
            ControlTime();

            shootCooldown = Mathf.Max(0, shootCooldown - Time.deltaTime);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            NPC npc = other.gameObject.GetComponent<NPC>();
            if (npc != null)
            {
                npc.Rescue();
            }
        }

        private void Control()
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += Vector3.up * speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.position += Vector3.left * speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.position += Vector3.down * speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += Vector3.right * speed * Time.deltaTime;
            }

            if (Input.GetMouseButtonDown(0) && Mathf.Approximately(shootCooldown, 0))
            {
                GameObject shot = Instantiate(shotPrefab, weapon.transform.position, weapon.transform.rotation);
                Destroy(shot, 2f);
                
                IShootTarget hit = weapon.Shoot();
                if (hit != null)
                {
                    hit.TakeDamage(weapon.Damage);
                }
                shootCooldown = weapon.ShootDelay;
            }
        }

        private void ControlTime()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                TimeDataManager.Instance.State = State.ReadData;
                GameManager.Instance.CurrentLevel.StopEndLevel();
            }

            if (Input.GetKeyUp(KeyCode.Q))
            {
                TimeDataManager.Instance.State = State.WriteData;
            }
        }

        private void Rotate()
        {
            Vector3 target = Camera.main.ScreenToWorldPoint((Vector2)Input.mousePosition);
            float angle = Mathf.Atan2(target.y - transform.position.y, target.x - transform.position.x);
            angle *= (180f / Mathf.PI);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        public void Die(Vector2? source = null)
        {
            if (source.HasValue)
            {
                GameObject go = Instantiate(bloodPrefab, transform.position, Quaternion.identity);
                float angle = Mathf.Atan2(source.Value.y - go.transform.position.y, source.Value.x - go.transform.position.x);
                angle *= (180f / Mathf.PI);
                go.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
                Destroy(go, 1f);
            }
            isDead = true;
            GameManager.Instance.CurrentLevel.EndLevel();
        }

        public void Freeze()
        {
            canControl = false;
        }

        public void Unfreeze()
        {
            canControl = true;
        }
        
        public RewindData GetData()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["position"] = transform.position;
            data["rotationZ"] = transform.eulerAngles.z;
            data["shootCooldown"] = shootCooldown;
            data["isdead"] = isDead;
            
            return new RewindData { Data = data };
        }

        public void ApplyData(RewindData data)
        {
            transform.position = (Vector3)data.Data["position"];
            transform.eulerAngles = new Vector3(0, 0, (float)data.Data["rotationZ"]);
            shootCooldown = (float)data.Data["shootCooldown"];
            bool oldIsDead = isDead;
            isDead = (bool)data.Data["isdead"];
            if (oldIsDead && !isDead)
            {
                GameManager.Instance.CurrentLevel.StopEndLevel();
            }
        }

        public void SetState(ProviderState state)
        {
            providerState = state;
        }
    }
}