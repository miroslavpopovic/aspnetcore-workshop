using TimeTracker.Domain;

namespace TimeTracker.Models
{
    /// <summary>
    /// Represents a single user to add or modify.
    /// </summary>
    public class UserInputModel
    {
        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the user hour rate.
        /// </summary>
        public decimal HourRate { get; set; }

        /// <summary>
        /// Maps the current model to the instance of <see cref="User"/>.
        /// </summary>
        /// <param name="user">A <see cref="User"/> instance to modify.</param>
        public void MapTo(User user)
        {
            user.Name = Name;
            user.HourRate = HourRate;
        }
    }
}
