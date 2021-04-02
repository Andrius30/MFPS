using UnityEngine;

namespace MFPS.ServerTimers
{
    public class Timer
    {
        float startTime;
        bool isDone;

        public Timer(float startTime, bool isDone)
        {
            this.startTime = startTime;
            this.isDone = isDone;
        }

        public void StartTimer()
        {
            if (startTime > 0)
            {
                startTime -= Time.deltaTime;
                isDone = false;
            }
            if(startTime <= 0)
            {
                startTime = 0;
                isDone = true;
            }
        }

        public void SetTimer(float time,bool done)
        {
            startTime = time;
            isDone = done;
        }
        public bool IsDone() => isDone;
    }
}