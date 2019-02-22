using System;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events.Hooks;
using Sitecore.SecurityModel;
using Sitecore.Support.Analytics.Rules.Conditions;

namespace Sitecore.Support.Hooks
{
  public class UpdateTypeFieldGoalItems : IHook
  {
    public void Initialize()
    {
      using (new SecurityDisabler())
      {
        var databaseName = "master";
        var fieldName = "Type";
        var database = Factory.GetDatabase(databaseName, false);
        if (database == null)
        {
          return;
        }

        /*                 
        the code below is suitable when the same field of different items has to be populated with different values (e.g. unique type names)
        */

        // array of item paths
        string[] itemPaths =
        {
          "/sitecore/system/Settings/Rules/Definitions/Elements/Visit/Goal was triggered during a past or current interaction",
          "/sitecore/system/Settings/Rules/Definitions/Elements/Visit/Goal with custom data was triggered during a past or current interaction",
          "/sitecore/system/Settings/Rules/Definitions/Elements/Visit/Campaign was triggered during a past or current interaction",
          "/sitecore/system/Settings/Rules/Definitions/Elements/Visit/Campaign with custom data was triggered during a past or current interaction",
          "/sitecore/system/Settings/Rules/Definitions/Elements/Visit/Channel of a past or current interaction",
          "/sitecore/system/Settings/Rules/Definitions/Elements/Visit/Channel with custom data of a past or current interaction",
          "/sitecore/system/Settings/Rules/Definitions/Elements/Visit/Page event was triggered during a past or current interaction",
          "/sitecore/system/Settings/Rules/Definitions/Elements/Visit/Page event with custom data was triggered during a past or current interaction",
          "/sitecore/system/Settings/Rules/Definitions/Elements/Visit/Venue of a past or current interaction",
          "/sitecore/system/Settings/Rules/Definitions/Elements/Visit/Venue with custom data of a past or current interaction"
        };

        // array of types
        Type[] types =
        {
          typeof(GoalWasTriggeredDuringPastOrCurrentInteractionCondition<>),
          typeof(GoalWithCustomDataWasTriggeredDuringPastOrCurrentInteractionCondition<>),
          typeof(CampaignWasTriggeredDuringPastOrCurrentInteractionCondition<>),
          typeof(CampaignWithCustomDataWasTriggeredDuringPastOrCurrentInteractionCondition<>),
          typeof(ChannelOfPastOrCurrentInteractionCondition<>),
          typeof(ChannelWithCustomDataOfPastOrCurrentInteractionCondition<>),
          typeof(PageEventWasTriggeredDuringPastOrCurrentInteractionCondition<>),
          typeof(PageEventWithCustomDataWasTriggeredDuringPastOrCurrentInteractionCondition<>),
          typeof(VenueOfPastOrCurrentInteractionCondition<>),
          typeof(VenueWithCustomDataOfPastOrCurrentInteractionCondition<>)
        };

        string typeName = null;
        string assemblyName = null;
        string fieldValue = null;
        Item item = null;
        var isAssemblyInstalled = false;

        // main loop: the same field of every item gets its unique value 
        for (var i = 0; i < itemPaths.Length; i++)
        {
          // removing '1 in the end of the generic type name
          typeName = types[i].FullName.Remove(types[i].FullName.IndexOf('`'));
          assemblyName = types[i].Assembly.GetName().Name;
          fieldValue = $"{typeName}, {assemblyName}";
          item = database.GetItem(itemPaths[i]);
          if (item != null)
          {
            if (string.Equals(item[fieldName], fieldValue, StringComparison.Ordinal))
            {
              isAssemblyInstalled = true;
              continue;
            }

            item.Editing.BeginEdit();
            item[fieldName] = fieldValue;
            item.Editing.EndEdit();
          }
        }

        if (!isAssemblyInstalled)
        {
          Log.Info($"Installing {assemblyName}", this);
        }
      }
    }
  }
}