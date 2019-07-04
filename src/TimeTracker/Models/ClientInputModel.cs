using TimeTracker.Domain;

namespace TimeTracker.Models
{
    /// <summary>
    /// Represents a single client to add or modify.
    /// </summary>
    public class ClientInputModel
    {
        /// <summary>
        /// Gets or sets the client name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Maps the current model into a <see cref="Client"/> instance.
        /// </summary>
        /// <param name="client">A client instance to modify.</param>
        public void MapTo(Client client)
        {
            client.Name = Name;
        }
    }
}
