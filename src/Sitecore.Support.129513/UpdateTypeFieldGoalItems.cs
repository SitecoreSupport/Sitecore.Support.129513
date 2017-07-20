using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;

namespace Sitecore.Support.Hooks
{
    using Sitecore.Configuration;
    using Sitecore.Diagnostics;
    using Sitecore.Events.Hooks;
    using Sitecore.SecurityModel;

    public class UpdateTypeFieldGoalItems : IHook
    {
        public void Initialize()
        {
            using (new SecurityDisabler())
            {
                var databaseName = "master";
                var fieldName = "Type";
                var database = Factory.GetDatabase(databaseName);
                if (database == null)
                {
                    return;
                }

                /* 
                
                
                the code below is suitable when the same field of different items has to be populated with different values (e.g. unique type names)


                */

                // array of item paths
                string[] itemPaths = new string[]
                {
                    "/sitecore/system/Settings/Rules/Definitions/Elements/Visit/Goal was triggered during a past or current interaction",
                    "/sitecore/system/Settings/Rules/Definitions/Elements/Visit/Goal with custom data was triggered during a past or current interaction",
                };

                // array of types
                Type[] types = new Type[]
                {
                    typeof(Sitecore.Support.Analytics.Rules.Conditions.GoalWasTriggeredDuringPastOrCurrentInteractionCondition<>),
                    typeof(Sitecore.Support.Analytics.Rules.Conditions.GoalWithCustomDataWasTriggeredDuringPastOrCurrentInteractionCondition<>)
                };

                string typeName = null;
                string assemblyName = null;
                string fieldValue = null;
                Item item = null;
                bool isAssemblyInstalled = false;

                // main loop: the same field of every item gets its unique value 
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
