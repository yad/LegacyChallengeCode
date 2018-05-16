using System;
using System.Threading.Tasks;

namespace LegacyQuestTracker
{
    public class QuestBuilder
    {
        private readonly Quest _quest;

        public QuestBuilder()
        {
            _quest = new Quest();
        }

        public QuestBuilder WithTitle(string title)
        {
            _quest.Title = title;
            return this;
        }

        public QuestBuilder WithObjective(string objective, params object[] args)
        {
            _quest.Objective = string.Format(objective, args);
            return this;
        }

        public QuestBuilder WithCondition(Func<Task<bool>> condition)
        {
            _quest.Condition = condition;
            return this;
        }

        public Quest Build()
        {
            return _quest;
        }
    }
}
