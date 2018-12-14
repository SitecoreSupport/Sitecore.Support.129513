using Sitecore.Analytics;
using Sitecore.Analytics.Core;
using Sitecore.Analytics.Model;
using Sitecore.Analytics.Tracking;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Support.Rules.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.Support.Analytics.Rules.Conditions
{
    public class GoalWasTriggeredDuringPastOrCurrentInteractionCondition<T> : HasEventOccurredCondition<T> where T : RuleContext
    {
        private Guid? _goalGuid;

        private bool _goalGuidInitialized;

        private bool filterByCustomData;

        public GoalWasTriggeredDuringPastOrCurrentInteractionCondition()
                : base(false)
        {
        }

        protected GoalWasTriggeredDuringPastOrCurrentInteractionCondition(bool filterByCustomData)
                : base(filterByCustomData)
        {
            this.filterByCustomData = filterByCustomData;
        }

        [NotNull]
        public string GoalId { get; set; }

        private Guid? GoalGuid
        {
            get
            {
                if (_goalGuidInitialized)
                {
                    return _goalGuid;
                }

                try
                {
                    _goalGuid = new Guid(GoalId);
                }
                catch
                {
                    Log.Warn($"Could not convert value to guid: {GoalId}", GetType());
                }

                _goalGuidInitialized = true;

                return _goalGuid;
            }
        }

        protected override bool Execute([NotNull] T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            Assert.IsNotNull(Tracker.Current, "Tracker.Current is not initialized");
            Assert.IsNotNull(Tracker.Current.Session, "Tracker.Current.Session is not initialized");
            Assert.IsNotNull(Tracker.Current.Session.Interaction, "Tracker.Current.Session.Interaction is not initialized");

            if (GoalGuid == null)
            {
                return false;
            }

            if (HasEventOccurredInInteraction(Tracker.Current.Session.Interaction))
            {
                return true;
            }

            Assert.IsNotNull(Tracker.Current.Contact, "Tracker.Current.Contact is not initialized");

            KeyBehaviorCache keyBehaviorCache = Tracker.Current.Contact.GetKeyBehaviorCache();

            return Enumerable.Any<KeyBehaviorCacheEntry>(this.FilterKeyBehaviorCacheEntries(keyBehaviorCache), (Func<KeyBehaviorCacheEntry, bool>)delegate (KeyBehaviorCacheEntry entry)
            {
                Guid id = entry.Id;
                Guid? b = this.GoalGuid;
                return (Guid?)id == b;
            });
        }

        protected override IEnumerable<KeyBehaviorCacheEntry> GetKeyBehaviorCacheEntries(KeyBehaviorCache keyBehaviorCache)
        {
            Assert.ArgumentNotNull(keyBehaviorCache, "keyBehaviorCache");

            return keyBehaviorCache.Goals;
        }

        protected override bool HasEventOccurredInInteraction(IInteractionData interaction)
        {
            Assert.ArgumentNotNull(interaction, "interaction");
            Assert.IsNotNull(interaction.Pages, "interaction.Pages is not initialized.");
            if (this.filterByCustomData)
            {
                IEnumerable<PageEventData> source = Enumerable.Where<PageEventData>(Enumerable.SelectMany<Page, PageEventData>((IEnumerable<Page>)interaction.Pages, (Func<Page, IEnumerable<PageEventData>>)((Page page) => page.PageEvents)), (Func<PageEventData, bool>)delegate (PageEventData pageEvent)
                {
                    if (!pageEvent.IsGoal)
                    {
                        Guid pageEventDefinitionId3 = pageEvent.PageEventDefinitionId;
                        Guid? b3 = this.GoalGuid;
                        return (Guid?)pageEventDefinitionId3 == b3;
                    }

                    return false;
                });

                if (this.CustomData == null)
                {
                    Log.Warn("CustomData can not be null", (object)base.GetType());
                    return false;
                }

                IEnumerable<PageEventData> source2 = Enumerable.Where<PageEventData>(source, (Func<PageEventData, bool>)delegate (PageEventData entry)
                {
                    if (entry.Data != null)
                    {
                        return ConditionsUtility.CompareStrings(entry.Data, this.CustomData, this.CustomDataOperatorId);
                    }

                    return false;
                });

                return Enumerable.Any<PageEventData>(source2, (Func<PageEventData, bool>)delegate (PageEventData entry)
                {
                    Guid pageEventDefinitionId2 = entry.PageEventDefinitionId;
                    Guid? b2 = this.GoalGuid;
                    return (Guid?)pageEventDefinitionId2 == b2;
                });
            }

            return interaction.Pages
                .SelectMany(page => page.PageEvents)
                .Any(pageEvent => pageEvent.IsGoal && pageEvent.PageEventDefinitionId == GoalGuid);
        }
    }
}

