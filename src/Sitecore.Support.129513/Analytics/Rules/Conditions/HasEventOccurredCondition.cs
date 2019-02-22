using Sitecore.Analytics.Tracking;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Rules.Conditions;

namespace Sitecore.Support.Analytics.Rules.Conditions
{
  public abstract class HasEventOccurredCondition<T> : WhenCondition<T> where T : RuleContext
  {
    private readonly bool filterByCustomData;

    protected HasEventOccurredCondition(bool filterByCustomData)
    {
      this.filterByCustomData = filterByCustomData;
    }

    public string CustomData { get; set; }

    public string CustomDataOperatorId { get; set; }

    public int NumberOfElapsedDays { get; set; }

    public string NumberOfElapsedDaysOperatorId { get; set; }

    public int NumberOfPastInteractions { get; set; }

    public string NumberOfPastInteractionsOperatorId { get; set; }

    protected virtual IEnumerable<KeyBehaviorCacheEntry> FilterKeyBehaviorCacheEntries(KeyBehaviorCache keyBehaviorCache)
    {
      Assert.ArgumentNotNull((object)keyBehaviorCache, nameof(keyBehaviorCache));
      IEnumerable<KeyBehaviorCacheEntry> behaviorCacheEntries = this.FilterKeyBehaviorCacheEntriesByInteractionConditions(keyBehaviorCache.Campaigns.Concat<KeyBehaviorCacheEntry>((IEnumerable<KeyBehaviorCacheEntry>)keyBehaviorCache.Channels).Concat<KeyBehaviorCacheEntry>((IEnumerable<KeyBehaviorCacheEntry>)keyBehaviorCache.CustomValues).Concat<KeyBehaviorCacheEntry>((IEnumerable<KeyBehaviorCacheEntry>)keyBehaviorCache.Goals).Concat<KeyBehaviorCacheEntry>((IEnumerable<KeyBehaviorCacheEntry>)keyBehaviorCache.Outcomes).Concat<KeyBehaviorCacheEntry>((IEnumerable<KeyBehaviorCacheEntry>)keyBehaviorCache.PageEvents).Concat<KeyBehaviorCacheEntry>((IEnumerable<KeyBehaviorCacheEntry>)keyBehaviorCache.Venues));
      if (this.filterByCustomData)
      {
        if (this.CustomData == null)
        {
          Log.Warn("CustomData can not be null", (object)this.GetType());
          return Enumerable.Empty<KeyBehaviorCacheEntry>();
        }
        behaviorCacheEntries = behaviorCacheEntries.Where<KeyBehaviorCacheEntry>((Func<KeyBehaviorCacheEntry, bool>)(entry =>
        {
          if (entry.Data != null)
            return ConditionsUtility.CompareStrings(entry.Data, this.CustomData, this.CustomDataOperatorId);
          return false;
        }));
      }
      return Assert.ResultNotNull<IEnumerable<KeyBehaviorCacheEntry>>(this.GetKeyBehaviorCacheEntries(keyBehaviorCache).Intersect<KeyBehaviorCacheEntry>(behaviorCacheEntries, (IEqualityComparer<KeyBehaviorCacheEntry>)new KeyBehaviorCacheEntry.KeyBehaviorCacheEntryEqualityComparer()));
    }

    protected virtual IEnumerable<KeyBehaviorCacheEntry> FilterKeyBehaviorCacheEntriesByInteractionConditions([NotNull] IEnumerable<KeyBehaviorCacheEntry> keyBehaviorCacheEntries)
    {
      Assert.ArgumentNotNull(keyBehaviorCacheEntries, "keyBehaviorCacheEntries");

      var numberOfElapsedDaysComparer = ConditionsUtility.GetInt32Comparer(NumberOfElapsedDaysOperatorId);
      if (numberOfElapsedDaysComparer == null)
      {
        return Enumerable.Empty<KeyBehaviorCacheEntry>();
      }

      var numberOfPastInteractionsComparer = ConditionsUtility.GetInt32Comparer(NumberOfPastInteractionsOperatorId);
      if (numberOfPastInteractionsComparer == null)
      {
        return Enumerable.Empty<KeyBehaviorCacheEntry>();
      }

      var filtered = keyBehaviorCacheEntries
        .GroupBy(entry => new
        {
          entry.InteractionId,
          entry.InteractionStartDateTime
        })
        .OrderByDescending(entries => entries.Key.InteractionStartDateTime)
        .Where((entries, i) => numberOfElapsedDaysComparer((DateTime.UtcNow - entries.Key.InteractionStartDateTime).Days, NumberOfElapsedDays) && numberOfPastInteractionsComparer(i + 2, NumberOfPastInteractions))
        .SelectMany(entries => entries);

      return Assert.ResultNotNull(filtered);
    }

    protected abstract IEnumerable<KeyBehaviorCacheEntry> GetKeyBehaviorCacheEntries(KeyBehaviorCache keyBehaviorCache);

    protected abstract bool HasEventOccurredInInteraction(IInteractionData interaction);
  }
}
