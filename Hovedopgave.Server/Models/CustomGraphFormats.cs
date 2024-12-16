namespace Hovedopgave.Server.Models
{
    public class CustomGraphFormats
    {
        public enum CustomGraphFormat 
        {
            Days = 14,
            Weeks = 98, // 12 weeks
            Months = 365,
            Years = 366
        }

        public static CustomGraphFormat GetGraphFormat(int daysInBetween) 
        {
            if (daysInBetween > 0 && daysInBetween <= (int)CustomGraphFormat.Days)return CustomGraphFormat.Days;
            else if (daysInBetween > (int)CustomGraphFormat.Days && daysInBetween <= (int)CustomGraphFormat.Weeks) return CustomGraphFormat.Weeks;
            else if (daysInBetween > (int)CustomGraphFormat.Weeks && daysInBetween <= (int)CustomGraphFormat.Months) return CustomGraphFormat.Months;
            else return CustomGraphFormat.Years;
        }
    }
}
