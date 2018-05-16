using System;

namespace LegacyQuestTracker.Helpers
{
    public static class ReflectionExt
    {
        internal static void Execute(this object obj, string method, params object[] args)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            obj.GetType().GetMethod(method).Invoke(obj, args);
        }

        internal static bool TryExecute(Action method)
        {
            bool success = false;
            try
            {
                method();
                success = true;
            }
            catch
            {
            }

            return success;
        }
    }
}
