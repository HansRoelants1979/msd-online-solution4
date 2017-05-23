namespace Tc.Crm.Common.Models
{
    public sealed class Customer : EntityModel
    {
        public CustomerType CustomerType { get; set; }
        public override string EntityName
        {
            get
            {
                return CustomerType == CustomerType.Account ? Constants.EntityName.Account : Constants.EntityName.Contact;
            }
        }
    }
}
