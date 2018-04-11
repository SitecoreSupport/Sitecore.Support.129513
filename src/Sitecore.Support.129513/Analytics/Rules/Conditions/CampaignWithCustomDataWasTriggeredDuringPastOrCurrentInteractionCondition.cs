using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Support.Analytics.Rules.Conditions
{


	// Sitecore.Support.Analytics.Rules.Conditions.CampaignWithCustomDataWasTriggeredDuringPastOrCurrentInteractionCondition<T>
	using Sitecore.Rules;
	using Sitecore.Support.Analytics.Rules.Conditions;

	public class CampaignWithCustomDataWasTriggeredDuringPastOrCurrentInteractionCondition<T> : CampaignWasTriggeredDuringPastOrCurrentInteractionCondition<T> where T : RuleContext
	{
		public CampaignWithCustomDataWasTriggeredDuringPastOrCurrentInteractionCondition()
			: base(true)
		{
		}
	}

}