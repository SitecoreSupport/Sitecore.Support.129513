using Sitecore.Rules;

namespace Sitecore.Support.Analytics.Rules.Conditions
{
    public class CampaignWithCustomDataWasTriggeredDuringPastOrCurrentInteractionCondition<T> : CampaignWasTriggeredDuringPastOrCurrentInteractionCondition<T> where T : RuleContext
    {
        public CampaignWithCustomDataWasTriggeredDuringPastOrCurrentInteractionCondition()
            : base(true)
        {
        }
    }
}