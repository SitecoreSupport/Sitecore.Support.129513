using Sitecore.Analytics.Outcome.Model;
using Sitecore.Analytics.Tracking;
using Sitecore.Common;
using Sitecore.Data;
using Sitecore.Diagnostics;
using System.Collections.Generic;

namespace Sitecore.Support.Analytics.Outcome.Rules.Conditions
{
    public static class TrackerExtensions
    {
        internal static Dictionary<ID, IOutcome> GetContactOutcomes(Session session)
        {
            Dictionary<ID, IOutcome> dictionary;
            if (session.CustomData.ContainsKey("Sitecore.Analytics.Outcome#Outcomes"))
            {
                dictionary = (session.CustomData["Sitecore.Analytics.Outcome#Outcomes"] as Dictionary<ID, IOutcome>);
                string text = string.Format("Outcome data in session for the contact {0} contained the key {1}, but the type of the object was not Dictionary<ID, IOutcome> as expected.", session.Contact.ContactId, "Sitecore.Analytics.Outcome#Outcomes");
                Assert.IsNotNull((object)dictionary, text);
                foreach (IOutcome value in dictionary.Values)
                {
                    value.EntityId=(TypeExtensions.ToID(session.Contact.ContactId));
                }

                return dictionary;                
            }
            
            dictionary = new Dictionary<ID, IOutcome>();
            session.CustomData["Sitecore.Analytics.Outcome#Outcomes"] = dictionary;
            return dictionary;
        }
    }
}