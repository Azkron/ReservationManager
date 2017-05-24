

using System;
using System.Text.RegularExpressions;

namespace ReservationManager
{
    public enum Right
    {
        ALL,
        READ,
        NONE
    }

    public enum Table
    {
        SHOW,
        CLIENT,
        RESERVATION,
        CATEGORY,
        PRICE,
        USER,
        SIZE
    }

    public static class Util
    {
        public static string FilterLetters(string str, int length = 4)
        {
            str = Regex.Replace(str, "[^0-9.]", "");

            if (str.Length > length)
                str = str.Substring(0, length);

            return str;
        }

        public static int StrToInt(string str, int length = 4)
        {
            return Convert.ToInt32(FilterLetters(str, length));
        }
    }
}
