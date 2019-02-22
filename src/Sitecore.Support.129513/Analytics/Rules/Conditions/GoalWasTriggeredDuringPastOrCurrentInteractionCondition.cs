using Sitecore.Analytics.Core;
using Sitecore.Analytics.Model;
using Sitecore.Analytics.Tracking;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Analytics;

namespace Sitecore.Support.Analytics.Rules.Conditions
{
  public class GoalWasTriggeredDuringPastOrCurrentInteractionCondition<T> : HasEventOccurredCondition<T> where T : RuleContext
  {
    private Guid? _goalGuid;
    private bool _goalGuidInitialized;

    public GoalWasTriggeredDuringPastOrCurrentInteractionCondition()
      : base(false)
    {
    }

    protected GoalWasTriggeredDuringPastOrCurrentInteractionCondition(bool filterByCustomData)
      : base(filterByCustomData)
    {
    }

    public string GoalId { get; set; }

    private Guid? GoalGuid
    {
      get
      {
        if (this._goalGuidInitialized)
          return this._goalGuid;
        try
        {
          this._goalGuid = new Guid?(new Guid(this.GoalId));
        }
        catch
        {
          Log.Warn(string.Format("Could not convert value to guid: {0}", (object)this.GoalId), (object)this.GetType());
        }
        this._goalGuidInitialized = true;
        return this._goalGuid;
      }
    }

    protected override bool Execute(T ruleContext)
    {
      Assert.ArgumentNotNull((object)ruleContext, nameof(ruleContext));
      Assert.IsNotNull((object)Tracker.Current, "Tracker.Current is not initialized");
      Assert.IsNotNull((object)Tracker.Current.Session, "Tracker.Current.Session is not initialized");
      Assert.IsNotNull((object)Tracker.Current.Session.Interaction, "Tracker.Current.Session.Interaction is not initialized");
      if (!this.GoalGuid.HasValue)
        return false;
      if (this.HasEventOccurredInInteraction((IInteractionData)Tracker.Current.Session.Interaction))
        return true;
      Assert.IsNotNull((object)Tracker.Current.Contact, "Tracker.Current.Contact is not initialized");
      return this.FilterKeyBehaviorCacheEntries(Tracker.Current.Contact.KeyBehaviorCache).Any<KeyBehaviorCacheEntry>((Func<KeyBehaviorCacheEntry, bool>)(entry =>
      {
        Guid id = entry.Id;
        Guid? goalGuid = this.GoalGuid;
        if (!goalGuid.HasValue)
          return false;
        return id == goalGuid.GetValueOrDefault();
      }));
    }

    protected override IEnumerable<KeyBehaviorCacheEntry> GetKeyBehaviorCacheEntries(KeyBehaviorCache keyBehaviorCache)
    {
      Assert.ArgumentNotNull((object)keyBehaviorCache, nameof(keyBehaviorCache));
      return (IEnumerable<KeyBehaviorCacheEntry>)keyBehaviorCache.Goals;
    }

    protected override bool HasEventOccurredInInteraction(IInteractionData interaction)
    {
      Assert.ArgumentNotNull((object)interaction, nameof(interaction));
      Assert.IsNotNull((object)interaction.Pages, "interaction.Pages is not initialized.");
      return ((IEnumerable<Page>)interaction.Pages).SelectMany<Page, PageEventData>((Func<Page, IEnumerable<PageEventData>>)(page => page.PageEvents)).Any<PageEventData>((Func<PageEventData, bool>)(pageEvent =>
      {
        if (!pageEvent.IsGoal)
          return false;
        Guid eventDefinitionId = pageEvent.PageEventDefinitionId;
        Guid? goalGuid = this.GoalGuid;
        if (!goalGuid.HasValue)
          return false;
        return eventDefinitionId == goalGuid.GetValueOrDefault();
      }));
    }
  }
}
