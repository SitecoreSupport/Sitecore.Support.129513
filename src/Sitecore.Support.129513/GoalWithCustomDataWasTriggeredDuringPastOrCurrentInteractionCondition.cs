using Sitecore.Rules;

namespace Sitecore.Support.Analytics.Rules.Conditions
{
    public class GoalWithCustomDataWasTriggeredDuringPastOrCurrentInteractionCondition<T> : GoalWasTriggeredDuringPastOrCurrentInteractionCondition<T> where T : RuleContext
    {
        public GoalWithCustomDataWasTriggeredDuringPastOrCurrentInteractionCondition()
          : base(true)
        {
        }
    }
}
