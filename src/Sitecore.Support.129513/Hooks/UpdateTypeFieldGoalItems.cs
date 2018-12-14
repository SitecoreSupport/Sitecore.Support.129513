using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events.Hooks;
using Sitecore.SecurityModel;
using System;

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

                string[] itemPaths = new string[]
                {
                    "/sitecore/system/Settings/Rules/Definitions/Elements/Visit/Goal was triggered during a past or current interaction",
                    "/sitecore/system/Settings/Rules/Definitions/Elements/Visit/Goal with custom data was triggered during a past or current interaction",
                    "/sitecore/system/Settings/Rules/Definitions/Elements/Visit/Campaign was triggered during a past or current interaction",
                    "/sitecore/system/Settings/Rules/Definitions/Elements/Visit/Campaign with custom data was triggered during a past or current interaction",
                    "/sitecore/system/Settings/Rules/Definitions/Elements/Visit/Channel of a past or current interaction",
                    "/sitecore/system/Settings/Rules/Definitions/Elements/Visit/Page event was triggered during a past or current interaction",
                    "/sitecore/system/Settings/Rules/Definitions/Elements/Visit/Page event with custom data was triggered during a past or current interaction",
                    "/sitecore/system/Settings/Rules/Definitions/Elements/Visit/Venue of a past or current interaction",
                    "/sitecore/system/Settings/Rules/Definitions/Elements/Visit/Venue with custom data of a past or current interaction"
                };

                Type[] types = new Type[]
                {
                    typeof(Analytics.Rules.Conditions.GoalWasTriggeredDuringPastOrCurrentInteractionCondition<>),
                    typeof(Analytics.Rules.Conditions.GoalWithCustomDataWasTriggeredDuringPastOrCurrentInteractionCondition<>),
                    typeof(Analytics.Rules.Conditions.CampaignWasTriggeredDuringPastOrCurrentInteractionCondition<>),
                    typeof(Analytics.Rules.Conditions.CampaignWithCustomDataWasTriggeredDuringPastOrCurrentInteractionCondition<>),
                    typeof(Analytics.Rules.Conditions.ChannelOfPastOrCurrentInteractionCondition<>),
                    typeof(Analytics.Rules.Conditions.PageEventWasTriggeredDuringPastOrCurrentInteractionCondition<>),
                    typeof(Analytics.Rules.Conditions.PageEventWithCustomDataWasTriggeredDuringPastOrCurrentInteractionCondition<>),
                    typeof(Analytics.Rules.Conditions.VenueOfPastOrCurrentInteractionCondition<>),
                    typeof(Analytics.Rules.Conditions.VenueWithCustomDataOfPastOrCurrentInteractionCondition<>),
                };

                string typeName = null;
                string assemblyName = null;
                string fieldValue = null;
                Item item = null;
                bool isAssemblyInstalled = false;

                for (int i = 0; i < itemPaths.Length; i++)
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