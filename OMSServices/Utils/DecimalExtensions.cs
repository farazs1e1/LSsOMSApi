namespace OMSServices.Utils
{
    public static class DecimalExtensions
    {
        public static bool HasFractionalPart(this decimal number)
        {
            return (number % 1) != 0;
        }
    }
}
