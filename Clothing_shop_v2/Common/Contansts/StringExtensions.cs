namespace Clothing_shop_v2.Common.Contansts
{
    public static class StringExtensions
    {
        public static bool TryParseInt(this string value, out int result)
        {
            return int.TryParse(value, out result);
        }
    }
}
