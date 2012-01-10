using System;
using System.Xml.Linq;
using Microsoft.Phone.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace WordPressStarterKit.Extensions
{
    public static class Extensions
    {
        public static void CheckNotNullThrowException(this object obj)
        {
            if (obj == null)
                throw new Exception(obj.GetType().ToString() + " cannot be null");
        }
        public static void IfNullDoThis(this object obj, Action act)
        {
            if (obj == null)
                act.Invoke();
        }
        public static void IfNotNullDoThis(this object obj, Action act)
        {
            if (obj != null)
                act.Invoke();
        }

        public static void IfNotNullInvoke(this Action act)
        {
            if (act != null)
                act.Invoke();
        }

        public static string IfNullEmptyString(this XElement item, string key)
        {
            return (item.Element(key) == null) ? string.Empty : item.Element(key).Value;
        }
    }
}
