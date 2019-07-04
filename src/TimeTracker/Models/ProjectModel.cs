using TimeTracker.Domain;

namespace TimeTracker.Models
{
    /// <summary>
    /// Represents a single project.
    /// </summary>
    public class ProjectModel
    {
        /// <summary>
        /// Gets or sets the project id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the project name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the id of the client that owns this project.
        /// </summary>
        public long ClientId { get; set; }

        /// <summary>
        /// Gets or sets the name of the client that owns this project.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="ProjectModel"/> from the given <see cref="Project"/> instance.
        /// </summary>
        /// <param name="project">A <see cref="Project"/> instance to convert to <see cref="ProjectModel"/>.</param>
        /// <returns>A new instance of <see cref="ProjectModel"/>.</returns>
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
