/* 
 *  Copyright © yiroth, all right reserved 
 *  Creation date: 2025-11-23
 *  Purpose: Helper class for time-related utilities
*/

using UnityEngine;

namespace LibYiroth.Helper
{
    public enum ETimeRate
    {
        [InspectorName("FPS: 1")]
        TimeRateFPS1,
        [InspectorName("FPS: 2")]
        TimeRateFPS2,
        [InspectorName("FPS: 5")]
        TimeRateFPS5,
        [InspectorName("FPS: 10")]
        TimeRateFPS10,
        [InspectorName("FPS: 12")]
        TimeRateFPS12,
        [InspectorName("FPS: 15")]
        TimeRateFPS15,
        [InspectorName("FPS: 20")]
        TimeRateFPS20,
        [InspectorName("FPS: 24")]
        TimeRateFPS24,
        [InspectorName("FPS: 25")]
        TimeRateFPS25,
        [InspectorName("FPS: 30")]
        TimeRateFPS30,
        [InspectorName("FPS: 48")]
        TimeRateFPS48,
        [InspectorName("FPS: 60")]
        TimeRateFPS60,
        [InspectorName("FPS: 72")]
        TimeRateFPS72,
        [InspectorName("FPS: 96")]
        TimeRateFPS96,
        [InspectorName("FPS: 100")]
        TimeRateFPS100,
        [InspectorName("FPS: 120")]
        TimeRateFPS120,
        [InspectorName("FPS: 144")]
        TimeRateFPS144,
        [InspectorName("FPS: 240")]
        TimeRateFPS240,
        [InspectorName("FPS: 360")]
        TimeRateFPS360
    }
    
    public class TimeRate
    {
        public const float ConstTimeRateFPS1 = 1.0f;
        public const float ConstTimeRateFPS2 = 0.5f;
        public const float ConstTimeRateFPS5 = 0.2f;
        public const float ConstTimeRateFPS10 = 0.1f;
        public const float ConstTimeRateFPS12 = 0.08333333f;
        public const float ConstTimeRateFPS15 = 0.06666667f;
        public const float ConstTimeRateFPS20 = 0.05f;
        public const float ConstTimeRateFPS24 = 0.04166667f;
        public const float ConstTimeRateFPS25 = 0.04f;
        public const float ConstTimeRateFPS30 = 0.03333333f;
        public const float ConstTimeRateFPS48 = 0.02083333f;
        public const float ConstTimeRateFPS60 = 0.01666667f;
        public const float ConstTimeRateFPS72 = 0.01388889f;
        public const float ConstTimeRateFPS96 = 0.01041667f;
        public const float ConstTimeRateFPS100 = 0.01f;
        public const float ConstTimeRateFPS120 = 0.008333333f;
        public const float ConstTimeRateFPS144 = 0.006944444f;
        public const float ConstTimeRateFPS240 = 0.004166667f;
        public const float ConstTimeRateFPS360 = 0.002777778f;

        public static float GetTimeRateValue(ETimeRate timeRate)
        {
            return timeRate switch
            {
                ETimeRate.TimeRateFPS1 => ConstTimeRateFPS1,
                ETimeRate.TimeRateFPS2 => ConstTimeRateFPS2,
                ETimeRate.TimeRateFPS5 => ConstTimeRateFPS5,
                ETimeRate.TimeRateFPS10 => ConstTimeRateFPS10,
                ETimeRate.TimeRateFPS12 => ConstTimeRateFPS12,
                ETimeRate.TimeRateFPS15 => ConstTimeRateFPS15,
                ETimeRate.TimeRateFPS20 => ConstTimeRateFPS20,
                ETimeRate.TimeRateFPS24 => ConstTimeRateFPS24,
                ETimeRate.TimeRateFPS25 => ConstTimeRateFPS25,
                ETimeRate.TimeRateFPS30 => ConstTimeRateFPS30,
                ETimeRate.TimeRateFPS48 => ConstTimeRateFPS48,
                ETimeRate.TimeRateFPS60 => ConstTimeRateFPS60,
                ETimeRate.TimeRateFPS72 => ConstTimeRateFPS72,
                ETimeRate.TimeRateFPS96 => ConstTimeRateFPS96,
                ETimeRate.TimeRateFPS100 => ConstTimeRateFPS100,
                ETimeRate.TimeRateFPS120 => ConstTimeRateFPS120,
                ETimeRate.TimeRateFPS144 => ConstTimeRateFPS144,
                ETimeRate.TimeRateFPS240 => ConstTimeRateFPS240,
                ETimeRate.TimeRateFPS360 => ConstTimeRateFPS360,
                _ => ConstTimeRateFPS30
            };
        }

        public static LibYiroth.Data.Time GetTimeFromSeconds(int totalSeconds)
        {
            totalSeconds = Mathf.Clamp(totalSeconds, 0, 86400);

            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;

            return new LibYiroth.Data.Time(hours, minutes, seconds);
        }            
    }
}
