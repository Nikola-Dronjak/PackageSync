namespace PackageSync.Domain
{
    /// <summary>
    /// Represents the User entity. This entity is used for authentication and authorization.
    /// Each user has the following fields:
    /// <ul>
    ///     <li>Username - string</li>
    ///     <li>Password - string</li>
    /// </ul>
    /// </summary>
    public class User
    {
        /// <summary>
        /// Represents the username of the user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Represents the password of the user.
        /// The password must contain at least 6 characters.
        /// The password must contain at least one upper case and one smaller case letter.
        /// The password must contain at least one digit.
        /// The password must contain at least one special character.
        /// </summary>
        public string Password { get; set; }
    }
}
