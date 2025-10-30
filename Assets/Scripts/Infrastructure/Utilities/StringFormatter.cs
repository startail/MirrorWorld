using UnityEngine;

namespace Infrastructure.Utilities
{
    public static class StringFormatter
    {
        public static string GetTimeDisplayString(float timeSeconds)
        {
            if (timeSeconds < 0)
            {
                timeSeconds = 0;
            }

            string displayText = "";
        
            const float SECONDS_IN_DAY = 24 * 60 * 60;
            const float SECONDS_IN_MINUTE = 60;
            const float SECONDS_IN_HOUR = 60 * 60;
        
            if (timeSeconds >= SECONDS_IN_DAY)
            {
                // 24時間より大きい場合は、後x日と表示
                int days = Mathf.FloorToInt(timeSeconds / SECONDS_IN_DAY);
                displayText = $"{days} Days";
            }
            else if (timeSeconds >= SECONDS_IN_MINUTE)
            {
                // 24時間以下1分より大きい場合は、時間:分:秒と表示
                int hours = Mathf.FloorToInt(timeSeconds / SECONDS_IN_HOUR);
                int remainingSecondsAfterHours = Mathf.FloorToInt(timeSeconds - hours * SECONDS_IN_HOUR);
                int minutes = Mathf.FloorToInt(remainingSecondsAfterHours / SECONDS_IN_MINUTE);
                float seconds = timeSeconds - (hours * SECONDS_IN_HOUR) - (minutes * SECONDS_IN_MINUTE);
            
                // 秒は小数点以下を含まない形式
                displayText = $"{hours:00}:{minutes:00}:{Mathf.FloorToInt(seconds):00}"; 
            }
            else
            {
                // 1分以下場合は、秒を表示
                displayText = $"{timeSeconds:00.00}";
            }
        
            return displayText;
        }
    }
}