using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Source
{
    public enum State
    {
        WriteData,
        ReadData,
    }
    
    public class TimeDataManager : MonoBehaviour
    {
        public static TimeDataManager Instance;
        
        public const float MaxRewindLength = 5f;
        private const float timeout = 1f;
        private bool isOnTimeout = false;
        [SerializeField] private GameObject rewindIcon;
        
        public int MaxRewindFrames => Mathf.FloorToInt(MaxRewindLength / Time.fixedDeltaTime);

        public State State = State.WriteData;

        private readonly Dictionary<IRewindDataProvider, LimitedStack<RewindData>> data = new Dictionary<IRewindDataProvider, LimitedStack<RewindData>>();

        private void Awake()
        {
            Instance = this;
        }

        private void FixedUpdate()
        {
            rewindIcon.SetActive(State == State.ReadData);
            
            if (isOnTimeout)
            {
                WriteData();
                return;
            }
            
            switch (State)
            {
                case State.WriteData:
                    WriteData();
                    break;
                case State.ReadData:
                    ReadData();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void WriteData()
        {
            foreach (KeyValuePair<IRewindDataProvider, LimitedStack<RewindData>> pair in data)
            {
                pair.Value.Push(pair.Key.GetData());
                pair.Key.SetState(ProviderState.Writing);
            }
        }

        private void ReadData()
        {
            foreach (KeyValuePair<IRewindDataProvider, LimitedStack<RewindData>> pair in data)
            {
                if (pair.Value.Count == 0)
                {
                    if (!isOnTimeout)
                    {
                        StartCoroutine(Timeout());
                    }
                    continue;
                }
                pair.Key.ApplyData(pair.Value.Pop());
                pair.Key.SetState(ProviderState.Reading);
            }
        }

        private IEnumerator Timeout()
        {
            isOnTimeout = true;
            State = State.WriteData;
            
            yield return new WaitForSeconds(timeout);

            isOnTimeout = false;
        }

        public void AddToData(IRewindDataProvider provider)
        {
            data[provider] = new LimitedStack<RewindData>(MaxRewindFrames);
        }
    }
}