using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Analytics;
using Sitecore.Analytics.Tracking;
using Sitecore.Diagnostics;
using Sitecore.Rules;

namespace Sitecore.Support.Analytics.Rules.Conditions
{
  public class CampaignWasTriggeredDuringPastOrCurrentInteractionCondition<T> : HasEventOccurredCondition<T>
    where T : RuleContext
  {
    private Guid? _campaignGuid;
    private bool _campaignGuidInitialized;

    public CampaignWasTriggeredDuringPastOrCurrentInteractionCondition()
      : base(false)
    {
    }

    protected CampaignWasTriggeredDuringPastOrCurrentInteractionCondition(bool filterByCustomData)
      : base(filterByCustomData)
    {
    }

    public string CampaignId { get; set; }

    private Guid? CampaignGuid
    {
      get
      {
        if (_campaignGuidInitialized)
          return _campaignGuid;
        try
        {
          _campaignGuid = new Guid(CampaignId);
        }
        catch
        {
          Log.Warn(string.Format("Could not convert value to guid: {0}", CampaignId), GetType());
        }

        _campaignGuidInitialized = true;
        return _campaignGuid;
      }
    }

    protected override bool Execute(T ruleContext)
    {
      Assert.ArgumentNotNull(ruleContext, nameof(ruleContext));
      Assert.IsNotNull(Tracker.Current, "Tracker.Current is not initialized");
      Assert.IsNotNull(Tracker.Current.Session, "Tracker.Current.Session is not initialized");
      Assert.IsNotNull(Tracker.Current.Session.Interaction, "Tracker.Current.Session.Interaction is not initialized");
      if (!CampaignGuid.HasValue)
        return false;
      if (HasEventOccurredInInteraction(Tracker.Current.Session.Interaction))
        return true;
      Assert.IsNotNull(Tracker.Current.Contact, "Tracker.Current.Contact is not initialized");
      return FilterKeyBehaviorCacheEntries(Tracker.Current.Contact.KeyBehaviorCache).Any(entry =>
      {
        var id = entry.Id;
        var campaignGuid = CampaignGuid;
        if (!campaignGuid.HasValue)
          return false;
        return id == campaignGuid.GetValueOrDefault();
      });
    }

    protected override IEnumerable<KeyBehaviorCacheEntry> GetKeyBehaviorCacheEntries(KeyBehaviorCache keyBehaviorCache)
    {
      Assert.ArgumentNotNull(keyBehaviorCache, nameof(keyBehaviorCache));
      return keyBehaviorCache.Campaigns;
    }

    protected override bool HasEventOccurredInInteraction(IInteractionData interaction)
    {
      Assert.ArgumentNotNull(interaction, nameof(interaction));
      if (!interaction.CampaignId.HasValue)
        return false;
      var guid = interaction.CampaignId.Value;
      var campaignGuid = CampaignGuid;
      if (!campaignGuid.HasValue)
        return false;
      return guid == campaignGuid.GetValueOrDefault();
    }
  }
}