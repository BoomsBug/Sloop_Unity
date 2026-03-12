using UnityEngine;
using System;

namespace Sloop.Time
{
    public class GameTimeManager : MonoBehaviour
    {
        public static GameTimeManager Instance { get; private set; }

        [Header("Time Scale")]
        [SerializeField] private float realSecondsPerGameDay = 300f;

    
        public int Day { get; private set; } = 1;
        public int Month { get; private set; } = 1;
        public int Year { get; private set; } = 1604;

        public int Hour { get; private set; } = 6;
        public int Minute { get; private set; }

        const int MINUTES_PER_HOUR = 60;
        const int HOURS_PER_DAY = 24;
        const int DAYS_PER_MONTH = 30;
        const int MONTHS_PER_YEAR = 12;

        float secondsPerGameMinute;
        float timer;

        float timeScale = 1f;

        public bool IsPaused { get; private set; }

        public event Action<int, int, int> OnTimeChanged;
        public event Action<int> OnHourChanged;
        public event Action<int> OnDayChanged;
        public event Action<int, int, int> OnDateChanged;

        public event Action<bool> OnDayNightChanged;

        bool isNight;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            secondsPerGameMinute = realSecondsPerGameDay / (24f * 60f);
        }

        void Update()
        {
            if (IsPaused) return;

            timer += UnityEngine.Time.deltaTime * timeScale;

            while (timer >= secondsPerGameMinute)
            {
                timer -= secondsPerGameMinute;
                AdvanceMinute();
            }
        }

        void AdvanceMinute()
        {
            Minute++;

            if (Minute >= MINUTES_PER_HOUR)
            {
                Minute = 0;
                AdvanceHour();
            }

            OnTimeChanged?.Invoke(Day, Hour, Minute);
        }

        void AdvanceHour()
        {
            Hour++;

            if (Hour >= HOURS_PER_DAY)
            {
                Hour = 0;
                AdvanceDay();
            }

            OnHourChanged?.Invoke(Hour);

            bool nightNow = Hour < 6 || Hour >= 20;

            if (nightNow != isNight)
            {
                isNight = nightNow;
                OnDayNightChanged?.Invoke(isNight);
            }
        }

        void AdvanceDay()
        {
            Day++;

            OnDayChanged?.Invoke(Day);

            if (Day > DAYS_PER_MONTH)
            {
                Day = 1;
                Month++;

                if (Month > MONTHS_PER_YEAR)
                {
                    Month = 1;
                    Year++;
                }
            }

            OnDateChanged?.Invoke(Day, Month, Year);
        }

        public void SetTimeScale(float scale)
        {
            timeScale = Mathf.Max(0f, scale);
        }

        public void PauseTime()
        {
            IsPaused = true;
        }

        public void ResumeTime()
        {
            IsPaused = false;
        }

        public bool IsNight()
        {
            return isNight;
        }
    }
}