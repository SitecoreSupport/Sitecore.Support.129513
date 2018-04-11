namespace Sitecore.Support.Analytics.Rules.Conditions
{
	using Sitecore.Rules;
	using Sitecore.Support.Analytics.Rules.Conditions;

	public class VenueWithCustomDataOfPastOrCurrentInteractionCondition<T> : VenueOfPastOrCurrentInteractionCondition<T> where T : RuleContext
	{
		public VenueWithCustomDataOfPastOrCurrentInteractionCondition()
			: base(true)
		{
		}
	}

}