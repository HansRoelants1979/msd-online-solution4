using Microsoft.Xrm.Sdk;

namespace Tc.Crm.Plugins.Merge.BusinessLogic
{
    public class EntityMergeCustomer : EntityMerge
    {
        public EntityMergeCustomer(IOrganizationService service) : base(service)
        {
        }

        protected override string EntityName  => Entities.Contact;
        protected override string SourceSystemIdName => Attributes.Contact.SourceSystemId;
        protected override string DuplicateSourceSystemIdName => Attributes.Contact.DuplicateSourceSystemId;
        protected override string RecordId => Attributes.Contact.ContactId;
    }
}