using System.Collections.Generic;

namespace BusinessIntegrationClient.Dtos
{
    /// <summary>
    ///     Represents a reference to an entity, such as a <see cref="Restaurant" /> or <see cref="RetailLocation" />.
    /// </summary>
    public class EntityReference
    {
        public EntityReference()
        {
        }

        public EntityReference(string id, string type)
        {
            Id = id;
            EntityType = type;
        }

        /// <summary>
        ///     The Entity Id of the associated entity.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     The type of the entity, such as Supplier, Distributor, etc.. <see cref="BusinessApiExtensions.ListEntityTypes" />
        ///     and <see cref="EntityTypeInfo.EntityTypeName" />
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        ///     A dictionary that allows for additional Data Firewall configuration parameters.
        /// </summary>
        /// <remarks>
        ///     The hierarchy values need to be configured and defined ahead of time
        ///     as they relate to data firewall configuration for specific records.
        /// </remarks>
        public Dictionary<string, string> Hierarchy { get; set; }
    }
}