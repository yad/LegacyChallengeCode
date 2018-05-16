using Microsoft.Practices.ServiceLocation;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using LegacyQuestTracker.Helpers;
using System.Web;
using System.Threading.Tasks;

namespace LegacyQuestTracker
{
    public class QuestBook
    {
        private readonly List<Quest> _quests;

        public QuestBook()
        {
            _quests = new List<Quest>()
            {
                EnvironmentVersionQuest(),
                MvcVersionQuest(),
                PoolIntegratedModeQuest(),
                LoggerQuest(),
                UseGlobalAuthorizeFilterQuest(),
                RemoveLocalControllerAuthorizeFilterQuest(),
                AllowHttpThreadContextSwapQuest()
            };
        }

        private static Quest EnvironmentVersionQuest()
        {
            var envVersion = Environment.Version;
            return new QuestBuilder()
                .WithTitle("Upgrade .NET Framework")
                .WithObjective("Use latest .NET Framework Version, current version is Major {0}, Minor {1}", envVersion.Major, envVersion.Minor)
                .WithCondition(() => Task.FromResult(envVersion.Major == 4 && envVersion.Minor >= 6))
                .Build();
        }

        private static Quest MvcVersionQuest()
        {
            var mvcVersion = typeof(Controller).Assembly.GetName().Version;
            return new QuestBuilder()
                .WithTitle("Upgrade MVC Framework")
                .WithObjective("Use latest MVC Framework Version, current version is Major {0}, Minor {1}", mvcVersion.Major, mvcVersion.Minor)
                .WithCondition(() => Task.FromResult(mvcVersion.Major == 3 && mvcVersion.Minor >= 2))
                .Build();
        }

        private static Quest PoolIntegratedModeQuest()
        {
            var useIntegratedMode = System.Web.HttpRuntime.UsingIntegratedPipeline;
            return new QuestBuilder()
                .WithTitle("Use IIS Pool Integrated Pipeline Mode")
                .WithObjective("Switch application configuration to Integrated Pipeline mode, current mode is {0}", useIntegratedMode ? "integrated" : "classic")
                .WithCondition(() => Task.FromResult(useIntegratedMode))
                .Build();
        }

        private static Quest LoggerQuest()
        {
            var logger = ServiceLocator.Current.GetInstance("ILogger");
            return new QuestBuilder()
                .WithTitle("Use a working Logger")
                .WithObjective("Use a working Logger, current logguer is {0}", logger == null ? "not defined" : logger.ToString())
                .WithCondition(() => Task.FromResult(ReflectionExt.TryExecute(() => logger.Execute("Error", new object[] { "any" }))))
                .Build();
        }

        private static Quest UseGlobalAuthorizeFilterQuest()
        {
            var filters = GlobalFilters.Filters;
            return new QuestBuilder()
                .WithTitle("Use global AuthorizeAttribute")
                .WithObjective("Use global AuthorizeAttribute, filters are {0}", filters.Count == 0 ? "empty" : string.Join(" - ", filters.Select(f => f.Instance.GetType().Name)))
                .WithCondition(() => Task.FromResult(filters.Any(f => f.Instance is AuthorizeAttribute)))
                .Build();
        }

        private static Quest RemoveLocalControllerAuthorizeFilterQuest()
        {
            var controllerType = typeof(Controller);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => controllerType.IsAssignableFrom(p))
                .Where(p => p.GetCustomAttributes(true).Any(attr => attr is AuthorizeAttribute))
                .ToArray();

            return new QuestBuilder()
                .WithTitle("Remove local AuthorizeAttribute")
                .WithObjective("Remove local AuthorizeAttribute, controllers that use it are {0}", types.Length == 0 ? "empty" : string.Join(" - ", types.Select(c => c.Name)))
                .WithCondition(() => Task.FromResult(!types.Any(c => c.GetCustomAttributes(true).Any(attr => attr is AuthorizeAttribute))))
                .Build();
        }

        private static Quest AllowHttpThreadContextSwapQuest()
        {
            var expectedMessage = "expectedMessage";
            HttpContext.Current.Items.Add(expectedMessage, expectedMessage);
            return new QuestBuilder()
                .WithTitle("Allow http thread context swap")
                .WithObjective("The program just set a value into HttpContext.Current.Items then swap with await Task.Yield() but value disapears, please fix it")
                .WithCondition(async() =>
                {
                    await Task.Yield();
                    return HttpContext.Current != null && HttpContext.Current.Items[expectedMessage] != null && (string)HttpContext.Current.Items[expectedMessage] == expectedMessage;
                })
                .Build();
        }

        public async Task<IEnumerable<Quest>> GetQuests()
        {
            foreach(var quest in _quests)
            {
                var condition = await quest.Condition();
                quest.IsCompleted = condition;
            }

            return _quests;
        }
    }
}
