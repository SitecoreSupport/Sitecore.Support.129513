using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Analytics;
using Sitecore.Analytics.Tracking;
using Sitecore.Diagnostics;
using Sitecore.Rules;

namespace Sitecore.Support.Analytics.Rules.Conditions
{
  public class VenueOfPastOrCurrentInteractionCondition<T> : HasEventOccurredCondition<T> where T : RuleContext
  {
    private Guid? _venueGuid;
    private bool _venueGuidInitialized;

    public VenueOfPastOrCurrentInteractionCondition()
      : base(false)
    {
    }

    protected VenueOfPastOrCurrentInteractionCondition(bool filterByCustomData)
      : base(filterByCustomData)
    {
    }

    public string VenueId { get; set; }

    private Guid? VenueGuid
    {
      get
      {
        if (_venueGuidInitialized)
          return _venueGuid;
        try
        {
          _venueGuid = new Guid(VenueId);
        }
        catch
        {
          Log.Warn(string.Format("Could not convert value to guid: {0}", VenueId), GetType());
        }

        _venueGuidInitialized = true;
        return _venueGuid;
      }
    }

    protected override bool Execute(T ruleContext)
    {
      Assert.ArgumentNotNull(ruleContext, nameof(ruleContext));
      Assert.IsNotNull(Tracker.Current, "Tracker.Current is not initialized");
      Assert.IsNotNull(Tracker.Current.Session, "Tracker.Current.Session is not initialized");
      Assert.IsNotNull(Tracker.Current.Session.Interaction, "Tracker.Current.Session.Interaction is not initialized");
      if (!VenueGuid.HasValue)
        return false;
      if (HasEventOccurredInInteraction(Tracker.Current.Session.Interaction))
        return true;
      Assert.IsNotNull(Tracker.Current.Contact, "Tracker.Current.Contact is not initialized");
      return FilterKeyBehaviorCacheEntries(Tracker.Current.Contact.KeyBehaviorCache).Any(entry =>
      {
        var id = entry.Id;
        var venueGuid = VenueGuid;
        if (!venueGuid.HasValue)
          return false;
        return id == venueGuid.GetValueOrDefault();
      });
    }

    protected override IEnumerable<KeyBehaviorCacheEntry> GetKeyBehaviorCacheEntries(KeyBehaviorCache keyBehaviorCache)
    {
      Assert.ArgumentNotNull(keyBehaviorCache, nameof(keyBehaviorCache));
      return keyBehaviorCache.Venues;
    }

    protected override bool HasEventOccurredInInteraction(IInteractionData interaction)
    {
      Assert.ArgumentNotNull(interaction, nameof(interaction));
      if (!interaction.VenueId.HasValue)
        return false;
      var guid = interaction.VenueId.Value;
      var venueGuid = VenueGuid;
      if (!venueGuid.HasValue)
        return false;
      return guid == venueGuid.GetValueOrDefault();
    }
  }
}