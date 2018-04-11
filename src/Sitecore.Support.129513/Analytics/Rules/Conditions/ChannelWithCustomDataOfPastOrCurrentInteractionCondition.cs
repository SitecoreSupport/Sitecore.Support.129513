using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Support.Analytics.Rules.Conditions
{


	// Sitecore.Support.Analytics.Rules.Conditions.ChannelWithCustomDataOfPastOrCurrentInteractionCondition<T>
	using Sitecore.Rules;
	using Sitecore.Support.Analytics.Rules.Conditions;

	public class ChannelWithCustomDataOfPastOrCurrentInteractionCondition<T> : ChannelOfPastOrCurrentInteractionCondition<T> where T : RuleContext
	{
		public ChannelWithCustomDataOfPastOrCurrentInteractionCondition()
			: base(true)
		{
		}
	}

}