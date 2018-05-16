using Microsoft.Practices.ServiceLocation;
using System.Linq;

namespace LegacyQuestTracker.Helpers
{
    public static class ServiceLocatorExt
    {
        internal static object GetInstance(this IServiceLocator serviceLocator, string @interface)
        {
            return ServiceLocator.Current.GetAllInstances(null).SingleOrDefault(obj => obj.GetType().GetInterface(@interface) != null);
        }
    }
}
