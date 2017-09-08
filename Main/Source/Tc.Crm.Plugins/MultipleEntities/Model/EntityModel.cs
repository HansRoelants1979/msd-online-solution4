using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tc.Crm.Plugins.MultipleEntities.Model
{
    [DataContract(Name = "EntityModel")]
    public class EntityModel
    {
        [DataMember(Name = "Fields")]
        public List<Field> Fields { get; set; }
    }

    [DataContract(Name = "Field")]
    public class Field
    {
        [DataMember(Name = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "Type")]
        public FieldType Type { get; set; }

        [DataMember(Name = "Value")]
        public Object Value { get; set; }
    }

    [DataContract(Name = "OptionSet", Namespace = "")]
    [KnownType(typeof(OptionSet))]
    public class OptionSet
    {
        [DataMember(Name = "Value")]
        public int Value { get; set; }

        [DataMember(Name = "Name")]
        public string Name { get; set; }
    }

    [DataContract(Name = "Lookup", Namespace = "")]
    [KnownType(typeof(Lookup))]
    public class Lookup
    {
        [DataMember(Name = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember(Name = "LogicalName")]
        public string LogicalName { get; set; }
    }

    [DataContract(Name = "RecordCollection", Namespace = "")]
    [KnownType(typeof(RecordCollection))]
    public class RecordCollection
    {
        [DataMember(Name = "ActivityParty")]
        public List<EntityRecord> ActivityParty { get; set; }
    }

    [DataContract(Name = "EntityRecord", Namespace = "")]
    [KnownType(typeof(EntityRecord))]
    public class EntityRecord
    {
        [DataMember(Name = "PartyId")]
        public Lookup PartyId { get; set; }
    }

    [DataContract(Name = "FieldType")]
    public enum FieldType
    {
        [EnumMember]
        Boolean = 0,
        [EnumMember]
        DateTime = 1,
        [EnumMember]
        Decimal = 2,
        [EnumMember]
        Double = 3,
        [EnumMember]
        RecordCollection = 4,
        [EnumMember]
        Lookup = 5,
        [EnumMember]
        Guid = 6,
        [EnumMember]
        Int32 = 7,
        [EnumMember]
        OptionSet = 8,
        [EnumMember]
        String = 9,
        [EnumMember]
        Null = 10,
        [EnumMember]
        Amount = 11
    }
}
