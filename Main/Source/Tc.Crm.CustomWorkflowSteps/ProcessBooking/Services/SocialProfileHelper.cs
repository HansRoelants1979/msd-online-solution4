using Microsoft.Xrm.Sdk;
using System;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;
using System.Collections.Generic;
using Tc.Crm.CustomWorkflowSteps;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services
{
    public static class SocialProfileHelper
    {
        public static EntityCollection GetSocialProfileEntityFromPayload(Booking bookingInfo,Guid customerId, ITracingService trace)
        {
            if (bookingInfo == null) return null;
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Social Profile populate records - start");

            var socialprofile = bookingInfo.Customer.Social;
            if (socialprofile == null) throw new InvalidPluginExecutionException("social profile service is null;");

            EntityCollection entityCollectionsocialprofiles = new EntityCollection();
            if (socialprofile != null && socialprofile.Length > 0)
            {
                trace.Trace("Processing " + socialprofile.Length.ToString() + " Social Profile records - start");
                Entity socialProfileEntity = null;
                for (int i = 0; i < socialprofile.Length; i++)
                {
                    trace.Trace("Processing Social Profile " + i.ToString() + " - start");
                    socialProfileEntity = PrepareCustomerSocialProfiles(socialprofile[i], customerId, trace);
                    entityCollectionsocialprofiles.Entities.Add(socialProfileEntity);
                    trace.Trace("Processing Social Profile " + i + " - end");
                }
                trace.Trace("Processing " + socialprofile.Length + " Social Profile records - end");
            }
            trace.Trace("Accommodation populate records - end");
            return entityCollectionsocialprofiles;
        }

        private static Entity PrepareCustomerSocialProfiles(Social socialprofile, Guid customerId, ITracingService trace)
        {

            if (socialprofile.Value == null) throw new InvalidPluginExecutionException("social profile service is null;");

            trace.Trace("Preparing Social Profile information - Start");

            Entity socialprofileEntity = null;


            if (!string.IsNullOrWhiteSpace(socialprofile.Value))
                socialprofileEntity = new Entity(EntityName.SocialProfile, Attributes.SocialProfile.UniqueProfileId, socialprofile.Value);
            socialprofileEntity[Attributes.SocialProfile.ProfileName] = socialprofile.Value;

            socialprofileEntity[Attributes.SocialProfile.SocialChannel] = CommonXrm.GetCommunity(socialprofile.SocialType);
            if (customerId != null)
                socialprofileEntity[Attributes.SocialProfile.Customer] = new EntityReference(EntityName.Contact, customerId);



            trace.Trace("Preparing Booking Transport information - End");

            return socialprofileEntity;
        }
    }
}
