using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Support.Analytics.Rules.Conditions
{


	// Sitecore.Support.Analytics.Rules.Conditions.GoalWithCustomDataWasTriggeredDuringPastOrCurrentInteractionCondition<T>
	using Sitecore.Rules;
	using Sitecore.Support.Analytics.Rules.Conditions;

	public class GoalWithCustomDataWasTriggeredDuringPastOrCurrentInteractionCondition<T> : GoalWasTriggeredDuringPastOrCurrentInteractionCondition<T> where T : RuleContext
	{
		public GoalWithCustomDataWasTriggeredDuringPastOrCurrentInteractionCondition()
			: base(true)
		{
		}
	}

}