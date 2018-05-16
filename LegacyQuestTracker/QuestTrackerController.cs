using System.Collections.Generic;
using System.Web.Mvc;

namespace LegacyQuestTracker
{
    public class QuestTrackerController : AsyncController
    {
        [HttpGet]
        public void GetAsync()
        {
            AsyncManager.OutstandingOperations.Increment();
            new QuestBook().GetQuests().ContinueWith((result) =>
            {
                AsyncManager.Parameters["quests"] = result.Result;
                AsyncManager.OutstandingOperations.Decrement();
            });
        }

        public ActionResult GetCompleted(IEnumerable<Quest> quests)
        {
            return Json(quests, JsonRequestBehavior.AllowGet);
        }
    }
}
