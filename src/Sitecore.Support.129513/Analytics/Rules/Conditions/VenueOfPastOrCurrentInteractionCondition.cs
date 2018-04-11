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

	public class VenueOfPastOrCurrentInteractionCondition<T> : HasEventOccurredCondition<T> where T : RuleContext
	{
		private Guid? venueGuid;

		private bool venueGuidInitialized;

		public string VenueId
		{
			get;
			set;
		}

		private Guid? VenueGuid
		{
			get
			{
				if (this.venueGuidInitialized)
				{
					return this.venueGuid;
				}
				try
				{
					this.venueGuid = new Guid(this.VenueId);
				}
				catch
				{
					Log.Warn(string.Format("Could not convert value to guid: {0}", this.VenueId), (object)base.GetType());
				}
				this.venueGuidInitialized = true;
				return this.venueGuid;
			}
		}

		public VenueOfPastOrCurrentInteractionCondition()
			: base(false)
		{
		}

		protected VenueOfPastOrCurrentInteractionCondition(bool filterByCustomData)
			: base(filterByCustomData)
		{
		}

		protected override bool Execute(T ruleContext)
		{
			Assert.ArgumentNotNull((object)ruleContext, "ruleContext");
			Assert.IsNotNull((object)Tracker.Current, "Tracker.Current is not initialized");
			Assert.IsNotNull((object)Tracker.Current.Session, "Tracker.Current.Session is not initialized");
			Assert.IsNotNull((object)Tracker.Current.Session.Interaction, "Tracker.Current.Session.Interaction is not initialized");
			if (!this.VenueGuid.HasValue)
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
				Guid? b = this.VenueGuid;
				return (Guid?)id == b;
			});
		}

		protected override IEnumerable<KeyBehaviorCacheEntry> GetKeyBehaviorCacheEntries(KeyBehaviorCache keyBehaviorCache)
		{
			Assert.ArgumentNotNull((object)keyBehaviorCache, "keyBehaviorCache");
			return keyBehaviorCache.Venues;
		}

		protected override bool HasEventOccurredInInteraction(IInteractionData interaction)
		{
			Assert.ArgumentNotNull((object)interaction, "interaction");
			if (interaction.VenueId.HasValue)
			{
				Guid value = interaction.VenueId.Value;
				Guid? b = this.VenueGuid;
				return (Guid?)value == b;
			}
			return false;
		}
	}

}