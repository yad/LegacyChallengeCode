using System;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace LegacyQuestTracker
{
    public class Quest
    {
        [ScriptIgnore]
        public Func<Task<bool>> Condition { get; internal set; }
        public string Objective { get; internal set; }
        public string Title { get; internal set; }
        public bool IsCompleted { get; internal set; }
    }
}
