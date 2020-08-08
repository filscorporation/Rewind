using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Source
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public LevelManager CurrentLevel;
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SetMasks();
            
            CurrentLevel = FindObjectOfType<LevelManager>();

            if (SceneManager.GetActiveScene().name == "Level1Scene")
            {
                PlayerController.Instance.Freeze();
                return;
            }
            
            CurrentLevel.StartLevel();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        private void SetMasks()
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("BehindMask"))
            {
                go.layer = LayerMask.NameToLayer("BehindMask");
            }
        }
    }
}
