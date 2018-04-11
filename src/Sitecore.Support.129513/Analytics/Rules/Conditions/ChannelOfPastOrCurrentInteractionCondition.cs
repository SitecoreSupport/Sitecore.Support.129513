using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Support.Analytics.Rules.Conditions
{


	// Sitecore.Support.Analytics.Rules.Conditions.ChannelOfPastOrCurrentInteractionCondition<T>
	using Sitecore.Analytics;
	using Sitecore.Analytics.Tracking;
	using Sitecore.Diagnostics;
	using Sitecore.Rules;
	using Sitecore.Support.Analytics.Rules.Conditions;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class ChannelOfPastOrCurrentInteractionCondition<T> : HasEventOccurredCondition<T> where T : RuleContext
	{
		private Guid? channelGuid;

		private bool channelGuidInitialized;

		public string ChannelId
		{
			get;
			set;
		}

		private Guid? ChannelGuid
		{
			get
			{
				if (this.channelGuidInitialized)
				{
					return this.channelGuid;
				}
				try
				{
					this.channelGuid = new Guid(this.ChannelId);
				}
				catch
				{
					Log.Warn(string.Format("Could not convert value to guid: {0}", this.ChannelId), (object)base.GetType());
				}
				this.channelGuidInitialized = true;
				return this.channelGuid;
			}
		}

		public ChannelOfPastOrCurrentInteractionCondition()
			: base(false)
		{
		}

		protected ChannelOfPastOrCurrentInteractionCondition(bool filterByCustomData)
			: base(filterByCustomData)
		{
		}

		protected override bool Execute(T ruleContext)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected O, but got Unknown
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Expected O, but got Unknown
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			Assert.ArgumentNotNull((object)ruleContext, "ruleContext");
			Assert.IsNotNull((object)Tracker.Current, "Tracker.Current is not initialized");
			Assert.IsNotNull((object)Tracker.Current.Session, "Tracker.Current.Session is not initialized");
			Assert.IsNotNull((object)Tracker.Current.Session.Interaction, "Tracker.Current.Session.Interaction is not initialized");
			if (!this.ChannelGuid.HasValue)
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
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				Guid id = entry.Id;
				Guid? b = this.ChannelGuid;
				return (Guid?)id == b;
			});
		}

		protected override IEnumerable<KeyBehaviorCacheEntry> GetKeyBehaviorCacheEntries(KeyBehaviorCache keyBehaviorCache)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			Assert.ArgumentNotNull((object)keyBehaviorCache, "keyBehaviorCache");
			return keyBehaviorCache.Channels;
		}

		protected override bool HasEventOccurredInInteraction(IInteractionData interaction)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			Assert.ArgumentNotNull((object)interaction, "interaction");
			Guid channelId = interaction.ChannelId;
			Guid? b = this.ChannelGuid;
			return (Guid?)channelId == b;
		}
	}

}