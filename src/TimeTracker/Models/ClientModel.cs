using TimeTracker.Domain;

namespace TimeTracker.Models
{
    public class ClientModel
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public static ClientModel FromClient(Client client)
        {
            return new ClientModel
            {
                Id = client.Id,
                Name = client.Name
            };
        }
    }
}
