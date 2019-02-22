using Sitecore.Rules;

namespace Sitecore.Support.Analytics.Rules.Conditions
{
  public class
    ChannelWithCustomDataOfPastOrCurrentInteractionCondition<T> : ChannelOfPastOrCurrentInteractionCondition<T>
    where T : RuleContext
  {
    public ChannelWithCustomDataOfPastOrCurrentInteractionCondition()
      : base(true)
    {
    }
  }
}