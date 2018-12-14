using Sitecore.Analytics;
using Sitecore.Analytics.Outcome.Model;
using Sitecore.Analytics.Tracking;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Support.Analytics.Rules.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.Support.Analytics.Outcome.Rules.Conditions
{
    public class OutcomeWasRegisteredDuringPastOrCurrentInteractionCondition<T> : HasEventOccurredCondition<T> where T : RuleContext
    {
        private Guid? outcomeGuid;

        private bool outcomeGuidInitialized;

        public string OutcomeId { get; set; }

        private Guid? OutcomeGuid
        {
            get
            {
                if (this.outcomeGuidInitialized)
                {
                    return this.outcomeGuid;
                }

                try
                {
                    this.outcomeGuid = new Guid(this.OutcomeId);
                }
                catch
                {
                    Log.Warn(string.Format("Could not convert value to guid: {0}", this.OutcomeId), (object)base.GetType());
                }

                this.outcomeGuidInitialized = true;
                return this.outcomeGuid;
            }
        }

        public OutcomeWasRegisteredDuringPastOrCurrentInteractionCondition()
            : base(false)
        {
        }

        protected OutcomeWasRegisteredDuringPastOrCurrentInteractionCondition(bool filterByCustomData)
            : base(filterByCustomData)
        {
        }

        protected override bool Execute(T ruleContext)
        {
            Assert.ArgumentNotNull((object)ruleContext, "ruleContext");
            Assert.IsNotNull((object)Tracker.Current, "Tracker.Current is not initialized");
            Assert.IsNotNull((object)Tracker.Current.Session, "Tracker.Current.Session is not initialized");
            Assert.IsNotNull((object)Tracker.Current.Session.Interaction, "Tracker.Current.Session.Interaction is not initialized");
            if (!this.OutcomeGuid.HasValue)
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
                Guid? b = this.OutcomeGuid;
                return (Guid?)id == b;
            });
        }

        protected override IEnumerable<KeyBehaviorCacheEntry> GetKeyBehaviorCacheEntries(KeyBehaviorCache keyBehaviorCache)
        {
            Assert.ArgumentNotNull((object)keyBehaviorCache, "keyBehaviorCache");
            return keyBehaviorCache.Outcomes;
        }

        protected override bool HasEventOccurredInInteraction(IInteractionData interaction)
        {
            Assert.ArgumentNotNull((object)interaction, "interaction");
            Assert.IsNotNull((object)Tracker.Current, "Tracker.Current is not initialized");
            Assert.IsNotNull((object)Tracker.Current.Contact, "Tracker.Current.Contact is not initialized");

            return Enumerable.Any<IOutcome>(Enumerable.Select<KeyValuePair<ID, IOutcome>, IOutcome>((IEnumerable<KeyValuePair<ID, IOutcome>>)TrackerExtensions.GetContactOutcomes(Tracker.Current.Session), (Func<KeyValuePair<ID, IOutcome>, IOutcome>)((KeyValuePair<ID, IOutcome> keyValuePair) => keyValuePair.Value)), (Func<IOutcome, bool>)delegate (IOutcome outcome)
            {
                if (!outcome.InteractionId.IsNull && outcome.InteractionId.Guid == interaction.InteractionId)
                {
                    Guid guid = outcome.DefinitionId.Guid;
                    Guid? b = this.OutcomeGuid;
                    return (Guid?)guid == b;
                }

                return false;
            });
        }
    }
}