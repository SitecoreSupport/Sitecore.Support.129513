using Sitecore.Rules;

namespace Sitecore.Support.Analytics.Rules.Conditions
{
  public class
    PageEventWithCustomDataWasTriggeredDuringPastOrCurrentInteractionCondition<T> :
      PageEventWasTriggeredDuringPastOrCurrentInteractionCondition<T> where T : RuleContext
  {
    public PageEventWithCustomDataWasTriggeredDuringPastOrCurrentInteractionCondition()
      : base(true)
    {
    }
  }
}