

using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;

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

        public static void SelectText(TextBox t)
        {
            t.Focus();
            t.Select(0, t.Text.Length);
        }

        public static int StrToInt(string str, int length = 4)
        {
            int res = -1;

            Int32.TryParse(FilterLetters(str, length), out res);

            return res;
        }
    }
}
