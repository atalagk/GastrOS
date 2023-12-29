using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/* Author: Hong Yul Yang
 * (c) 2010 The University of Auckland */
namespace GastrOs.Wrapper.Helpers
{
    public static class StringUtils
    {
        public delegate String ToString<T>(T obj);

        public static String ToPrettyString<T>(T obj)
        {
            if (EqualityComparer<T>.Default.Equals(obj, default(T)))
                return "";
            return obj.ToString();
        }
        
        public static string ToPrettyString<T>(this IEnumerable<T> col)
        {
            return ToPrettyString(col, ", ", ToPrettyString);
        }

        public static string ToPrettyString<T>(this IEnumerable<T> col, string delim)
        {
            return ToPrettyString(col, delim, ToPrettyString);
        }
        public static String ToPrettyString<T>(this IEnumerable<T> col, string delim,
            ToString<T> stringFunc)
        {
            return ToPrettyString(col, delim, stringFunc, false, false);
        }
        public static string ToPrettyString<T>(this IEnumerable<T> col, string delim,
            bool brackets)
        {
            return ToPrettyString(col, delim, ToPrettyString, brackets, false);
        }
        public static String ToPrettyString<T>(this IEnumerable<T> col, string delim,
            bool brackets, bool skipEmpty)
        {
            return ToPrettyString(col, delim, ToPrettyString, brackets, skipEmpty);
        }

        public static String ToPrettyString<T>(this IEnumerable<T> col, string delim,
            ToString<T> stringFunc, bool brackets, bool skipEmpty)
        {
            if (col == null)
                return null;
            StringBuilder sb = new StringBuilder();
            if (brackets)
            {
                sb.Append("[");
            }

            int i = 0;
            foreach (T elem in col)
            {
                string str = stringFunc(elem);
                if (skipEmpty && (string.IsNullOrEmpty(str)))
                    continue;
                if (i > 0)
                    sb.Append(delim);
                sb.Append(str);
                i++;
            }

            if (brackets)
            {
                sb.Append("]");
            }

            return sb.ToString();
        }

        public static String ToPrettyString(this IEnumerable col, string delim, ToString<object> stringFunc)
        {
            if (col == null)
                return null;

            StringBuilder sb = new StringBuilder("[");

            int i = 0;
            foreach (object elem in col)
            {
                if (i > 0)
                {
                    sb.Append(delim);
                }
                sb.Append(stringFunc(elem));
                i++;
            }

            sb.Append("]");

            return sb.ToString();
        }

        public static string Indent(this string str, string indenter)
        {
            if (str == null)
                return null;

            StringBuilder sb = new StringBuilder(str);
            sb.Insert(0, indenter);
            sb.Replace("\n", "\n" + indenter);
            return sb.ToString();
        }
    }
}
