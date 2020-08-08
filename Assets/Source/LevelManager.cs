using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Source
{
    public enum LevelState
    {
        BeforeStart,
        Running,
        Ended,
    }
    
    public class LevelManager : MonoBehaviour, IRewindDataProvider
    {
        [SerializeField] private float time;
        [SerializeField] private GameObject restartScreen;
        [SerializeField] private GameObject wonScreen;
        [SerializeField] private Text rescueText;
        [SerializeField] private Text timeText;
        private float timer = 0f;
        protected ProviderState providerState = ProviderState.Writing;
        
        public int NPCs { get; private set; }
        private int npcsMax = 0;
        
        public LevelState LevelState = LevelState.BeforeStart;

        private void Start()
        {
            TimeDataManager.Instance.AddToData(this);
        }
        
        private void Update()
        {
            rescueText.text = $"Rescue: {NPCs}/{npcsMax}";
            timeText.text = $"Time left: {timer:F1}";
            
            switch (LevelState)
            {
                case LevelState.BeforeStart:
                    break;
                case LevelState.Running:
                    if (providerState == ProviderState.Writing)
                    {
                        timer = Mathf.Max(timer - Time.deltaTime, 0);
                        if (Mathf.Approximately(timer, 0))
                            EndLevel();
                    }
                    break;
                case LevelState.Ended:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void EndLevel()
        {
            LevelState = LevelState.Ended;

            StartCoroutine(ShowRestartScreen());
        }

        public void WinLevel()
        {
            LevelState = LevelState.Ended;

            StartCoroutine(ShowWonScreen());
        }

        public void StopEndLevel()
        {
            LevelState = LevelState.Running;
            
            StopAllCoroutines();
        }

        public void NextLevel()
        {
            StopAllCoroutines();
            try
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        private IEnumerator ShowRestartScreen()
        {
            yield return new WaitForSeconds(2f);

            restartScreen.SetActive(true);
        }

        private IEnumerator ShowWonScreen()
        {
            yield return new WaitForSeconds(1f);

            wonScreen.SetActive(true);
        }

        public void RestartLevel()
        {
            StopAllCoroutines();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void StartLevel()
        {
            LevelState = LevelState.Running;
            timer = time;
            
            PlayerController.Instance.Unfreeze();
            
            StartCoroutine(CountNPCs());
        }

        private IEnumerator CountNPCs()
        {
            yield return new WaitForSeconds(0.2f);
            NPCs = FindObjectsOfType<NPC>().Length;
            npcsMax = NPCs;
        }

        public void NPCRescued()
        {
            NPCs--;
            if (NPCs == 0)
            {
                WinLevel();
            }
        }

        public void NPCUnrescued()
        {
            NPCs++;
        }
        
        public virtual RewindData GetData()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["time"] = timer;
            
            return new RewindData { Data = data };
        }

        public virtual void ApplyData(RewindData data)
        {
            timer = (float)data.Data["time"];
        }

        public void SetState(ProviderState state)
        {
            providerState = state;
        }
    }
}