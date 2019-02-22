using Sitecore.Analytics.Tracking;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Analytics;

namespace Sitecore.Support.Analytics.Rules.Conditions
{
  public class ChannelOfPastOrCurrentInteractionCondition<T> : HasEventOccurredCondition<T> where T : RuleContext
  {
    private Guid? _channelGuid;
    private bool _channelGuidInitialized;

    public ChannelOfPastOrCurrentInteractionCondition()
      : base(false)
    {
    }

    protected ChannelOfPastOrCurrentInteractionCondition(bool filterByCustomData)
      : base(filterByCustomData)
    {
    }

    public string ChannelId { get; set; }

    private Guid? ChannelGuid
    {
      get
      {
        if (this._channelGuidInitialized)
          return this._channelGuid;
        try
        {
          this._channelGuid = new Guid?(new Guid(this.ChannelId));
        }
        catch
        {
          Log.Warn(string.Format("Could not convert value to guid: {0}", (object)this.ChannelId), (object)this.GetType());
        }
        this._channelGuidInitialized = true;
        return this._channelGuid;
      }
    }

    protected override bool Execute(T ruleContext)
    {
      Assert.ArgumentNotNull((object)ruleContext, nameof(ruleContext));
      Assert.IsNotNull((object)Tracker.Current, "Tracker.Current is not initialized");
      Assert.IsNotNull((object)Tracker.Current.Session, "Tracker.Current.Session is not initialized");
      Assert.IsNotNull((object)Tracker.Current.Session.Interaction, "Tracker.Current.Session.Interaction is not initialized");
      if (!this.ChannelGuid.HasValue)
        return false;
      if (this.HasEventOccurredInInteraction((IInteractionData)Tracker.Current.Session.Interaction))
        return true;
      Assert.IsNotNull((object)Tracker.Current.Contact, "Tracker.Current.Contact is not initialized");
      return this.FilterKeyBehaviorCacheEntries(Tracker.Current.Contact.KeyBehaviorCache).Any<KeyBehaviorCacheEntry>((Func<KeyBehaviorCacheEntry, bool>)(entry =>
      {
        Guid id = entry.Id;
        Guid? channelGuid = this.ChannelGuid;
        if (!channelGuid.HasValue)
          return false;
        return id == channelGuid.GetValueOrDefault();
      }));
    }

    protected override IEnumerable<KeyBehaviorCacheEntry> GetKeyBehaviorCacheEntries(KeyBehaviorCache keyBehaviorCache)
    {
      Assert.ArgumentNotNull((object)keyBehaviorCache, nameof(keyBehaviorCache));
      return (IEnumerable<KeyBehaviorCacheEntry>)keyBehaviorCache.Channels;
    }

    protected override bool HasEventOccurredInInteraction(IInteractionData interaction)
    {
      Assert.ArgumentNotNull((object)interaction, nameof(interaction));
      Guid channelId = interaction.ChannelId;
      Guid? channelGuid = this.ChannelGuid;
      if (!channelGuid.HasValue)
        return false;
      return channelId == channelGuid.GetValueOrDefault();
    }
  }
}
