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
using Sitecore.Support.Rules.Conditions;

namespace Sitecore.Support.Analytics.Rules.Conditions
{


	public class GoalWasTriggeredDuringPastOrCurrentInteractionCondition<T> : Sitecore.Support.Analytics.Rules.Conditions.HasEventOccurredCondition<T>
					where T : RuleContext
	{
		/// <summary>
		/// The goal GUID.
		/// </summary>
		private Guid? _goalGuid;

		/// <summary>
		/// The is goal GUID initialized.
		/// </summary>
		private bool _goalGuidInitialized;

	  private bool filterByCustomData;

		/// <summary>
		/// Initializes a new instance of the <see cref="GoalWasTriggeredDuringPastOrCurrentInteractionCondition{T}"/> class.
		/// </summary>
		public GoalWasTriggeredDuringPastOrCurrentInteractionCondition()
				: base(false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GoalWasTriggeredDuringPastOrCurrentInteractionCondition{T}"/> class.
		/// </summary>
		/// <param name="filterByCustomData">
		/// If set to <c>true</c>, the key behavior cache entries will be filtered by custom data.
		/// </param>
		protected GoalWasTriggeredDuringPastOrCurrentInteractionCondition(bool filterByCustomData)
				: base(filterByCustomData)
		{
		  this.filterByCustomData = filterByCustomData;
		}

		/// <summary>
		/// Gets or sets the identifier of the goal.
		/// </summary>
		/// <value>
		/// A <see cref="string"/> representing the ID of the goal.
		/// </value>
		[NotNull]
		public string GoalId { get; set; }

		/// <summary>
		/// Gets the goal GUID.
		/// </summary>
		/// <value>
		/// The goal GUID.
		/// </value>
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

		/// <summary>
		/// Executes the specified rule context.
		/// </summary>
		/// <param name="ruleContext">
		/// The rule context.
		/// </param>
		/// <returns>
		/// <c>True</c>, if the condition succeeds, otherwise <c>false</c>.
		/// </returns>
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

		/// <summary>
		/// Gets key behavior cache entries.
		/// </summary>
		/// <param name="keyBehaviorCache">
		/// The key behavior cache.
		/// </param>
		/// <returns>
		/// The <see cref="IEnumerable{KeyBehaviorCacheEntry}"/>.
		/// </returns>
		protected override IEnumerable<KeyBehaviorCacheEntry> GetKeyBehaviorCacheEntries(KeyBehaviorCache keyBehaviorCache)
		{
			Assert.ArgumentNotNull(keyBehaviorCache, "keyBehaviorCache");

			return keyBehaviorCache.Goals;
		}

		/// <summary>
		/// Checks whether event occurred in interaction.
		/// </summary>
		/// <param name="interaction">
		/// The interaction.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>
		/// <c>true</c> if the event occurred in the interaction; otherwise, <c>false</c>.
		/// </returns>
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
			var result = interaction.Pages.SelectMany(page => page.PageEvents).Any(pageEvent => pageEvent.IsGoal && pageEvent.PageEventDefinitionId == GoalGuid);
		  return result;
		}
	}
}

