using TimeTracker.Domain;

namespace TimeTracker.Models
{
    /// <summary>
    /// Represents a single user.
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the user hour rate.
        /// </summary>
        public decimal HourRate { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="UserModel"/> from an instance of <see cref="User"/>.
        /// </summary>
        /// <param name="user">An instance of <see cref="User"/> to convert to <see cref="UserModel"/>.</param>
        /// <returns>A new instance of <see cref="UserModel"/>.</returns>
        public static UserModel FromUser(User user)
        {
            return new UserModel
            {
                Id = user.Id,
                Name = user.Name,
                HourRate = user.HourRate
            };
        }
    }
}
