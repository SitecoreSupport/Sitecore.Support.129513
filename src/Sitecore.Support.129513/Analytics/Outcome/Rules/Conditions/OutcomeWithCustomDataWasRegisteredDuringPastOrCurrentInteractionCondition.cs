using Sitecore.Rules;

namespace Sitecore.Support.Analytics.Outcome.Rules.Conditions
{
    public class OutcomeWithCustomDataWasRegisteredDuringPastOrCurrentInteractionCondition<T> : OutcomeWasRegisteredDuringPastOrCurrentInteractionCondition<T> where T : RuleContext
    {
        public OutcomeWithCustomDataWasRegisteredDuringPastOrCurrentInteractionCondition()
            : base(true)
        {
        }
    }
}