using Sitecore.Analytics;
using Sitecore.Analytics.Tracking;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.Support.Analytics.Rules.Conditions
{
    public class ChannelOfPastOrCurrentInteractionCondition<T> : HasEventOccurredCondition<T> where T : RuleContext
    {
        private Guid? channelGuid;

        private bool channelGuidInitialized;

        private bool filterByCustomData;

        public string ChannelId { get; set; }

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
            this.filterByCustomData = filterByCustomData;
        }

        protected override bool Execute(T ruleContext)
        {
            Assert.ArgumentNotNull((object)ruleContext, "ruleContext");
            Assert.IsNotNull((object)Tracker.Current, "Tracker.Current is not initialized");
            Assert.IsNotNull((object)Tracker.Current.Session, "Tracker.Current.Session is not initialized");
            Assert.IsNotNull((object)Tracker.Current.Session.Interaction, "Tracker.Current.Session.Interaction is not initialized");
            if (!this.ChannelGuid.HasValue)
            {
                return false;
            }

            if (!this.HasEventOccurredInInteraction(Tracker.Current.Session.Interaction))
            {
                return false;
            }

            Assert.IsNotNull((object)Tracker.Current.Contact, "Tracker.Current.Contact is not initialized");
            KeyBehaviorCache keyBehaviorCache = ContactKeyBehaviorCacheExtension.GetKeyBehaviorCache(Tracker.Current.Contact);

            return Enumerable.Any<KeyBehaviorCacheEntry>(this.FilterKeyBehaviorCacheEntries(keyBehaviorCache), (Func<KeyBehaviorCacheEntry, bool>)delegate (KeyBehaviorCacheEntry entry)
            {
                Guid id = entry.Id;
                Guid? b = this.ChannelGuid;
                return (Guid?)id == b;
            });
        }

        protected override IEnumerable<KeyBehaviorCacheEntry> GetKeyBehaviorCacheEntries(KeyBehaviorCache keyBehaviorCache)
        {
            Assert.ArgumentNotNull((object)keyBehaviorCache, "keyBehaviorCache");
            return keyBehaviorCache.Channels;
        }

        protected override bool HasEventOccurredInInteraction(IInteractionData interaction)
        {
            Assert.ArgumentNotNull((object)interaction, "interaction");
            Guid channelId = interaction.ChannelId;
            Guid? b = this.ChannelGuid;
            return (Guid?)channelId == b;
        }
    }
}