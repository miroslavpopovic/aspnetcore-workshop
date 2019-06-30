using TimeTracker.Domain;

namespace TimeTracker.Models
{
    public class ClientInputModel
    {
        public string Name { get; set; }

        public void MapTo(Client client)
        {
            client.Name = Name;
        }
    }
}
