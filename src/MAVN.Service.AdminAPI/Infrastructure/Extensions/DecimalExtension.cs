using System;
using System.Globalization;

namespace MAVN.Service.AdminAPI.Infrastructure.Extensions
{
    public static class DecimalExtension
    {
        public static string ToKBM(this decimal num)
        {
            var truncated = Math.Truncate(num);

            var thousands = truncated.ToString("#,##0,k", CultureInfo.InvariantCulture);

            if (thousands.Replace(",", "").Length <= 7)
            {
                return thousands;
            }

            var millions = truncated.ToString("#,##0,,m", CultureInfo.InvariantCulture);

            if (millions.Replace(",", "").Length <= 7)
            {
                return millions;
            }

            var billions = truncated.ToString("#,##0,,,b", CultureInfo.InvariantCulture);

            return billions;
        }
    }
}
