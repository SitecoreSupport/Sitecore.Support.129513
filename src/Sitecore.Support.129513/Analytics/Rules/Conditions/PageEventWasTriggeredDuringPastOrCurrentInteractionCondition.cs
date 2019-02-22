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
  public class PageEventWasTriggeredDuringPastOrCurrentInteractionCondition<T> : HasEventOccurredCondition<T> where T : RuleContext
  {
    private Guid? _pageEventGuid;
    private bool _pageEventGuidInitialized;

    public PageEventWasTriggeredDuringPastOrCurrentInteractionCondition()
      : base(false)
    {
    }

    protected PageEventWasTriggeredDuringPastOrCurrentInteractionCondition(bool filterByCustomData)
      : base(filterByCustomData)
    {
    }

    public string PageEventId { get; set; }

    private Guid? PageEventGuid
    {
      get
      {
        if (this._pageEventGuidInitialized)
          return this._pageEventGuid;
        try
        {
          this._pageEventGuid = new Guid?(new Guid(this.PageEventId));
        }
        catch
        {
          Log.Warn(string.Format("Could not convert value to guid: {0}", (object)this.PageEventId), (object)this.GetType());
        }
        this._pageEventGuidInitialized = true;
        return this._pageEventGuid;
      }
    }

    protected override bool Execute(T ruleContext)
    {
      Assert.ArgumentNotNull((object)ruleContext, nameof(ruleContext));
      Assert.IsNotNull((object)Tracker.Current, "Tracker.Current is not initialized");
      Assert.IsNotNull((object)Tracker.Current.Session, "Tracker.Current.Session is not initialized");
      Assert.IsNotNull((object)Tracker.Current.Session.Interaction, "Tracker.Current.Session.Interaction is not initialized");
      if (!this.PageEventGuid.HasValue)
        return false;
      if (this.HasEventOccurredInInteraction((IInteractionData)Tracker.Current.Session.Interaction))
        return true;
      Assert.IsNotNull((object)Tracker.Current.Contact, "Tracker.Current.Contact is not initialized");
      return this.FilterKeyBehaviorCacheEntries(Tracker.Current.Contact.KeyBehaviorCache).Any<KeyBehaviorCacheEntry>((Func<KeyBehaviorCacheEntry, bool>)(entry =>
      {
        Guid id = entry.Id;
        Guid? pageEventGuid = this.PageEventGuid;
        if (!pageEventGuid.HasValue)
          return false;
        return id == pageEventGuid.GetValueOrDefault();
      }));
    }

    protected override IEnumerable<KeyBehaviorCacheEntry> GetKeyBehaviorCacheEntries(KeyBehaviorCache keyBehaviorCache)
    {
      Assert.ArgumentNotNull((object)keyBehaviorCache, nameof(keyBehaviorCache));
      return (IEnumerable<KeyBehaviorCacheEntry>)keyBehaviorCache.PageEvents;
    }

    protected override bool HasEventOccurredInInteraction(IInteractionData interaction)
    {
      Assert.ArgumentNotNull((object)interaction, nameof(interaction));
      Assert.IsNotNull((object)interaction.Pages, "interaction.Pages is not initialized.");
      return ((IEnumerable<Page>)interaction.Pages).SelectMany<Page, PageEventData>((Func<Page, IEnumerable<PageEventData>>)(page => page.PageEvents)).Any<PageEventData>((Func<PageEventData, bool>)(pageEvent =>
      {
        if (pageEvent.IsGoal)
          return false;
        Guid eventDefinitionId = pageEvent.PageEventDefinitionId;
        Guid? pageEventGuid = this.PageEventGuid;
        if (!pageEventGuid.HasValue)
          return false;
        return eventDefinitionId == pageEventGuid.GetValueOrDefault();
      }));
    }
  }
}
