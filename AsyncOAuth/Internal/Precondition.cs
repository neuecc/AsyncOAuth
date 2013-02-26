using System;

namespace AsyncOAuth
{
    internal static class Precondition
    {
        public static void NotNull(object obj, string parameterName = "", string message = "")
        {
            if (obj == null) throw new ArgumentNullException(parameterName, message);
        }

        public static void NotNullOrEmpty(string value, string parameterName = "", string message = "")
        {
            if (value == null) throw new ArgumentNullException(parameterName, message);
            if (value == "") throw new ArgumentException(message, parameterName);
        }

        public static void Requires(bool condition, string parameterName = "", string message = "")
        {
            if (!condition) throw new ArgumentException(message, parameterName);
        }
    }
}