using Microsoft.Xrm.Sdk;
using System;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessCustomer.Services
{
    public static class SocialProfileHelper
    {
        public static EntityCollection GetSocialProfileEntityFromPayload(Customer customerInfo, Guid customerId, ITracingService trace)
        {
            if (customerInfo == null) return null;
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Social Profile populate records - start");

            var socialprofile = customerInfo.Social;
            if (socialprofile == null) return null;
            if (socialprofile.Length <= 0) return null;
            EntityCollection entityCollectionsocialprofiles = new EntityCollection();

            trace.Trace("Processing " + socialprofile.Length.ToString() + " Social Profile records - start");
            Entity socialProfileEntity = null;
            for (int profileCount = 0; profileCount < socialprofile.Length; profileCount++)
            {
                socialProfileEntity = PrepareCustomerSocialProfiles(socialprofile[profileCount], customerId, trace);
                if (socialProfileEntity != null)
                    entityCollectionsocialprofiles.Entities.Add(socialProfileEntity);
            }
            trace.Trace("Processing " + socialprofile.Length + " Social Profile records - end");

            trace.Trace("Social Profile populate records - end");
            return entityCollectionsocialprofiles;
        }

        private static Entity PrepareCustomerSocialProfiles(Social socialprofile, Guid customerId, ITracingService trace)
        {            
            if (socialprofile.Value == null) return null;
            trace.Trace("Preparing Social Profile information - Start");
            Entity socialprofileEntity = null;
            if (!string.IsNullOrWhiteSpace(socialprofile.Value))
                socialprofileEntity = new Entity(EntityName.SocialProfile, Attributes.SocialProfile.UniqueProfileId, socialprofile.Value);
            if (!string.IsNullOrWhiteSpace(socialprofile.Value))
                socialprofileEntity[Attributes.SocialProfile.ProfileName] = socialprofile.Value;
            if (!string.IsNullOrWhiteSpace(socialprofile.SocialType))
                socialprofileEntity[Attributes.SocialProfile.SocialChannel] = CommonXrm.GetCommunity(socialprofile.SocialType);
            if (customerId != null)
                socialprofileEntity[Attributes.SocialProfile.Customer] = new EntityReference(EntityName.Contact, customerId);
            trace.Trace("Preparing Social Profile information - End");
            return socialprofileEntity;
        }
    }
}
