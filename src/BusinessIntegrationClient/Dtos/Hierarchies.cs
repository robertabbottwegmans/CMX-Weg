using System.Collections.Generic;
using System.Linq;

namespace BusinessIntegrationClient.Dtos
{
    /// <summary>
    ///     Represents hierarchies sued to configure the data firewall for a <see cref="User" />.
    /// </summary>
    public class Hierarchies
    {
        /// <summary>
        ///     Creates an instance with the specified number of <see cref="Hierarchy" /> dictionaries already initialized, makes
        ///     using the <see cref="this" /> indexer easier.
        /// </summary>
        /// <param name="numberOfHierarchy">the # of dictionaries to create</param>
        public Hierarchies(int numberOfHierarchy)
        {
            Hierarchy = Enumerable.Range(1, numberOfHierarchy)
                .Select(i => new Dictionary<string, string>())
                .ToList();
        }

        public Hierarchies()
        {
            Hierarchy = new List<Dictionary<string, string>>();
        }

        /// <summary>
        ///     A list of Dictionaries representing levels in a hierarchy.  Used for Data Firewall configuration. Keys need to be
        ///     configured ahead of time in the system.
        /// </summary>
        public List<Dictionary<string, string>> Hierarchy { get; set; }

        /// <summary>
        ///     An indexer that allows easier access of <see cref="Hierarchy" /> items.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Dictionary<string, string> this[int index]
        {
            get { return Hierarchy[index]; }
            set { Hierarchy[index] = value; }
        }
    }
}