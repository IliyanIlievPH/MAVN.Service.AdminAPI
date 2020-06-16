namespace MAVN.Service.AdminAPI.StringUtils
{
    public static class StringUtils
    {
        public static string SanitizeName(this string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            return $"{name[0]}***";
        }
    }
}
