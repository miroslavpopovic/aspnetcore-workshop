using Microsoft.AspNetCore.Mvc;
using TimeTracker.Domain;

namespace TimeTracker.Models
{
    public class ProjectModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long ClientId { get; set; }
        public string ClientName { get; set; }

        public static ProjectModel FromProject(Project project)
        {
            return new ProjectModel
            {
                Id = project.Id,
                Name = project.Name,
                ClientId = project.Client.Id,
                ClientName = project.Client.Name
            };
        }
    }
}
