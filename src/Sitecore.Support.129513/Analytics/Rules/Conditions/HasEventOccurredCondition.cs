using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Support.Analytics.Rules.Conditions
{


	// Sitecore.Support.Analytics.Rules.Conditions.HasEventOccurredCondition<T>
	using Sitecore.Analytics.Tracking;
	using Sitecore.Diagnostics;
	using Sitecore.Rules;
	using Sitecore.Rules.Conditions;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public abstract class HasEventOccurredCondition<T>:WhenCondition<T> where T : RuleContext
	{
		private readonly bool filterByCustomData;

		public string CustomData
		{
			get;
			set;
		}

		public string CustomDataOperatorId
		{
			get;
			set;
		}

		public int NumberOfElapsedDays
		{
			get;
			set;
		}

		public string NumberOfElapsedDaysOperatorId
		{
			get;
			set;
		}

		public int NumberOfPastInteractions
		{
			get;
			set;
		}

		public string NumberOfPastInteractionsOperatorId
		{
			get;
			set;
		}

		protected HasEventOccurredCondition(bool filterByCustomData)
			: base()
		{
			this.filterByCustomData = filterByCustomData;
		}

		protected virtual IEnumerable<KeyBehaviorCacheEntry> FilterKeyBehaviorCacheEntries(KeyBehaviorCache keyBehaviorCache)
		{
			Assert.ArgumentNotNull((object)keyBehaviorCache, "keyBehaviorCache");
			IEnumerable<KeyBehaviorCacheEntry> keyBehaviorCacheEntries = Enumerable.Concat<KeyBehaviorCacheEntry>(Enumerable.Concat<KeyBehaviorCacheEntry>(Enumerable.Concat<KeyBehaviorCacheEntry>(Enumerable.Concat<KeyBehaviorCacheEntry>(Enumerable.Concat<KeyBehaviorCacheEntry>(Enumerable.Concat<KeyBehaviorCacheEntry>((IEnumerable<KeyBehaviorCacheEntry>)keyBehaviorCache.Campaigns, (IEnumerable<KeyBehaviorCacheEntry>)keyBehaviorCache.Channels), (IEnumerable<KeyBehaviorCacheEntry>)keyBehaviorCache.CustomValues), (IEnumerable<KeyBehaviorCacheEntry>)keyBehaviorCache.Goals), (IEnumerable<KeyBehaviorCacheEntry>)keyBehaviorCache.Outcomes), (IEnumerable<KeyBehaviorCacheEntry>)keyBehaviorCache.PageEvents), (IEnumerable<KeyBehaviorCacheEntry>)keyBehaviorCache.Venues);
			IEnumerable<KeyBehaviorCacheEntry> enumerable = this.FilterKeyBehaviorCacheEntriesByInteractionConditions(keyBehaviorCacheEntries);
			if (this.filterByCustomData)
			{
				if (this.CustomData == null)
				{
					Log.Warn("CustomData can not be null", (object)base.GetType());
					return Enumerable.Empty<KeyBehaviorCacheEntry>();
				}
				enumerable = Enumerable.Where<KeyBehaviorCacheEntry>(enumerable, (Func<KeyBehaviorCacheEntry, bool>)delegate (KeyBehaviorCacheEntry entry)
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0008: Unknown result type (might be due to invalid IL or missing references)
					if (entry.Data != null)
					{
						return ConditionsUtility.CompareStrings(entry.Data, this.CustomData, this.CustomDataOperatorId);
					}
					return false;
				});
			}
			return Assert.ResultNotNull<IEnumerable<KeyBehaviorCacheEntry>>(Enumerable.Intersect<KeyBehaviorCacheEntry>(this.GetKeyBehaviorCacheEntries(keyBehaviorCache), enumerable, (IEqualityComparer<KeyBehaviorCacheEntry>)new Sitecore.Analytics.Model.Entities.KeyBehaviorCacheEntryEqualityComparer()));
		}

		protected virtual IEnumerable<KeyBehaviorCacheEntry> FilterKeyBehaviorCacheEntriesByInteractionConditions(IEnumerable<KeyBehaviorCacheEntry> keyBehaviorCacheEntries)
		{
			Assert.ArgumentNotNull((object)keyBehaviorCacheEntries, "keyBehaviorCacheEntries");
			if (ConditionsUtility.GetInt32Comparer(this.NumberOfElapsedDaysOperatorId) == null)
			{
				return Enumerable.Empty<KeyBehaviorCacheEntry>();
			}
			Func<int, int, bool> numberOfPastInteractionsComparer = ConditionsUtility.GetInt32Comparer(this.NumberOfPastInteractionsOperatorId);
			Func<int, int, bool> numberOfElapsedDaysOperatorsComparer = ConditionsUtility.GetInt32Comparer(this.NumberOfElapsedDaysOperatorId);
			if (numberOfPastInteractionsComparer == null)
			{
				return Enumerable.Empty<KeyBehaviorCacheEntry>();
			}
			return Assert.ResultNotNull<IEnumerable<KeyBehaviorCacheEntry>>(Enumerable.SelectMany(Enumerable.Where(Enumerable.OrderByDescending(Enumerable.GroupBy(keyBehaviorCacheEntries, (KeyBehaviorCacheEntry entry) => new
			{
				InteractionId = entry.InteractionId,
				InteractionStartDateTime = entry.InteractionStartDateTime
			}), entries => entries.Key.InteractionStartDateTime), (entries, i) =>
			{
				if (numberOfElapsedDaysOperatorsComparer((DateTime.UtcNow - entries.Key.InteractionStartDateTime).Days, this.NumberOfElapsedDays))
				{
					return numberOfPastInteractionsComparer(i + 2, this.NumberOfPastInteractions);
				}
				return false;
			}), entries => (IEnumerable<KeyBehaviorCacheEntry>)entries));
		}

		protected abstract IEnumerable<KeyBehaviorCacheEntry> GetKeyBehaviorCacheEntries(KeyBehaviorCache keyBehaviorCache);

		protected abstract bool HasEventOccurredInInteraction(IInteractionData interaction);
	}

}