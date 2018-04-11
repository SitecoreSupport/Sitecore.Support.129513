namespace Sitecore.Support.Analytics.Outcome.Rules.Conditions
{
	using Sitecore.Rules;
	using Sitecore.Support.Analytics.Outcome.Rules.Conditions;

	public class OutcomeWithCustomDataWasRegisteredDuringPastOrCurrentInteractionCondition<T> : OutcomeWasRegisteredDuringPastOrCurrentInteractionCondition<T> where T : RuleContext
	{
		public OutcomeWithCustomDataWasRegisteredDuringPastOrCurrentInteractionCondition()
			: base(true)
		{
		}
	}

}