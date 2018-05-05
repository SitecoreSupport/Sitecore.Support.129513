using Sitecore.Support.Rules.Conditions;

namespace Sitecore.Support.Analytics.Rules.Conditions
{
	using Sitecore.Analytics;
	using Sitecore.Analytics.Core;
	using Sitecore.Analytics.Model;
	using Sitecore.Analytics.Tracking;
	using Sitecore.Diagnostics;
	using Sitecore.Rules;
	using Sitecore.Support.Analytics.Rules.Conditions;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class PageEventWasTriggeredDuringPastOrCurrentInteractionCondition<T> : HasEventOccurredCondition<T> where T : RuleContext
	{
		private Guid? pageEventGuid;

		private bool pageEventGuidInitialized;
	  private bool filterByCustomData;
		public string PageEventId
		{
			get;
			set;
		}

		private Guid? PageEventGuid
		{
			get
			{
				if (this.pageEventGuidInitialized)
				{
					return this.pageEventGuid;
				}
				try
				{
					this.pageEventGuid = new Guid(this.PageEventId);
				}
				catch
				{
					Log.Warn(string.Format("Could not convert value to guid: {0}", this.PageEventId), (object)base.GetType());
				}
				this.pageEventGuidInitialized = true;
				return this.pageEventGuid;
			}
		}

		public PageEventWasTriggeredDuringPastOrCurrentInteractionCondition()
			: base(false)
		{
		}

		protected PageEventWasTriggeredDuringPastOrCurrentInteractionCondition(bool filterByCustomData)
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
			if (!this.PageEventGuid.HasValue)
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
				Guid? b = this.PageEventGuid;
				return (Guid?)id == b;
			});
		}

		protected override IEnumerable<KeyBehaviorCacheEntry> GetKeyBehaviorCacheEntries(KeyBehaviorCache keyBehaviorCache)
		{
			Assert.ArgumentNotNull((object)keyBehaviorCache, "keyBehaviorCache");
			return keyBehaviorCache.PageEvents;
		}

		protected override bool HasEventOccurredInInteraction(IInteractionData interaction)
		{
			Assert.ArgumentNotNull((object)interaction, "interaction");
			Assert.IsNotNull((object)interaction.Pages, "interaction.Pages is not initialized.");
		  if (this.filterByCustomData)
		  {
		    IEnumerable<PageEventData> source = Enumerable.Where<PageEventData>(Enumerable.SelectMany<Page, PageEventData>((IEnumerable<Page>)interaction.Pages, (Func<Page, IEnumerable<PageEventData>>)((Page page) => page.PageEvents)), (Func<PageEventData, bool>)delegate (PageEventData pageEvent)
		    {

		      if (!pageEvent.IsGoal)
		      {
		        Guid pageEventDefinitionId3 = pageEvent.PageEventDefinitionId;
		        Guid? b3 = this.PageEventGuid;
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
		      Guid? b2 = this.PageEventGuid;
		      return (Guid?)pageEventDefinitionId2 == b2;
		    });
		  }
			return Enumerable.Any<PageEventData>(Enumerable.SelectMany<Page, PageEventData>((IEnumerable<Page>)interaction.Pages, (Func<Page, IEnumerable<PageEventData>>)((Page page) => page.PageEvents)), (Func<PageEventData, bool>)delegate (PageEventData pageEvent)
			{
				if (!pageEvent.IsGoal)
				{
					Guid pageEventDefinitionId = pageEvent.PageEventDefinitionId;
					Guid? b = this.PageEventGuid;
					return (Guid?)pageEventDefinitionId == b;
				}
				return false;
			});
		}
	}

}