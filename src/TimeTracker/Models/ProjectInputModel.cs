using TimeTracker.Domain;

namespace TimeTracker.Models
{
    /// <summary>
    /// Represents a single project to add or modify.
    /// </summary>
    public class ProjectInputModel
    {
        /// <summary>
        /// Gets or sets the project name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the client id that the project belongs to.
        /// </summary>
        public long ClientId { get; set; }

        /// <summary>
        /// Map the current model into a <see cref="Project"/> instance.
        /// </summary>
        /// <param name="project">A project instance to modify.</param>
        public void MapTo(Project project)
        {
            project.Name = Name;
        }
    }
}
