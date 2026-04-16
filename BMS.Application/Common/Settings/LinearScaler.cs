namespace BMS.Application.Common.Settings
{
    public class LinearScaler
    {
        public double Scale { get; set; }
        public double Offset { get; set; }
        public LinearScaler()
        {

        }
        // سازنده با وارد کردن Scale و Offset
        public LinearScaler(double scale, double offset)
        {
            Scale = scale;
            Offset = offset;
        }
        public LinearScaler(double displayMin, double displayMax, double rawMin, double rawMax)
        {
            Scale = (displayMax - displayMin) / (rawMax - rawMin);
            Offset = displayMin - (rawMin * Scale);
        }
        public class LinearScalerResult
        {
            public double Scale { get; set; }
            public double Offset { get; set; }
        }

        public LinearScalerResult CreateScaler(double scale, double offset)
        {
            return new LinearScalerResult
            {
                Scale = scale,
                Offset = offset
            };
        }
        // سازنده با وارد کردن بازه‌ها (محاسبه خودکار Scale و Offset)


        // تبدیل Raw به Display
        public double RawToDisplay(double raw)
        {
            return raw * Scale + Offset;
        }

        // تبدیل Display به Raw
        public double DisplayToRaw(double display)
        {
            return (display - Offset) / Scale;
        }
    }
}