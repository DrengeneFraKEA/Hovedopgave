namespace Hovedopgave.Server.Models
{
    public class CustomGraphFormats
    {
        public enum CustomGraphFormat 
        {
            Days = 30,
            Weeks = 182, // 26 weeks
            Months = 21900,
            Years = 21901
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
