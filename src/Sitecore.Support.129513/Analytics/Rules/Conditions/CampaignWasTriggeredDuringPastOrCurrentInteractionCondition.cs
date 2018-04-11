namespace Sitecore.Support.Analytics.Rules.Conditions
{
	using Sitecore.Analytics;
	using Sitecore.Analytics.Tracking;
	using Sitecore.Diagnostics;
	using Sitecore.Rules;
	using Sitecore.Support.Analytics.Rules.Conditions;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class CampaignWasTriggeredDuringPastOrCurrentInteractionCondition<T> : HasEventOccurredCondition<T> where T : RuleContext
	{
		private Guid? campaignGuid;

		private bool campaignGuidInitialized;

		public string CampaignId
		{
			get;
			set;
		}

		private Guid? CampaignGuid
		{
			get
			{
				if (this.campaignGuidInitialized)
				{
					return this.campaignGuid;
				}
				try
				{
					this.campaignGuid = new Guid(this.CampaignId);
				}
				catch
				{
					Log.Warn(string.Format("Could not convert value to guid: {0}", this.CampaignId), (object)base.GetType());
				}
				this.campaignGuidInitialized = true;
				return this.campaignGuid;
			}
		}

		public CampaignWasTriggeredDuringPastOrCurrentInteractionCondition()
			: base(false)
		{
		}

		protected CampaignWasTriggeredDuringPastOrCurrentInteractionCondition(bool filterByCustomData)
			: base(filterByCustomData)
		{
		}

		protected override bool Execute(T ruleContext)
		{
			Assert.ArgumentNotNull((object)ruleContext, "ruleContext");
			Assert.IsNotNull((object)Tracker.Current, "Tracker.Current is not initialized");
			Assert.IsNotNull((object)Tracker.Current.Session, "Tracker.Current.Session is not initialized");
			Assert.IsNotNull((object)Tracker.Current.Session.Interaction, "Tracker.Current.Session.Interaction is not initialized");
			if (!this.CampaignGuid.HasValue)
			{
				return false;
			}
			if (this.HasEventOccurredInInteraction(Tracker.Current.Session.Interaction))
			{
				return true;
			}
			Assert.IsNotNull((object)Tracker.Current.Contact, "Tracker.Current.Contact is not initialized");
			KeyBehaviorCache keyBehaviorCache = ContactKeyBehaviorCacheExtension.GetKeyBehaviorCache(Tracker.Current.Contact);
			return Enumerable.Any<KeyBehaviorCacheEntry>(this.FilterKeyBehaviorCacheEntries(keyBehaviorCache), (Func<KeyBehaviorCacheEntry, bool>)delegate (KeyBehaviorCacheEntry entry)
			{
				Guid id = entry.Id;
				Guid? b = this.CampaignGuid;
				return (Guid?)id == b;
			});
		}

		protected override IEnumerable<KeyBehaviorCacheEntry> GetKeyBehaviorCacheEntries(KeyBehaviorCache keyBehaviorCache)
		{
			Assert.ArgumentNotNull((object)keyBehaviorCache, "keyBehaviorCache");
			return keyBehaviorCache.Campaigns;
		}

		protected override bool HasEventOccurredInInteraction(IInteractionData interaction)
		{
			Assert.ArgumentNotNull((object)interaction, "interaction");
			if (interaction.CampaignId.HasValue)
			{
				Guid value = interaction.CampaignId.Value;
				Guid? b = this.CampaignGuid;
				return (Guid?)value == b;
			}
			return false;
		}
	}

}