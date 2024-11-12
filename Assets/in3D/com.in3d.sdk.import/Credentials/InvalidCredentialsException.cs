using System;

namespace com.in3d.sdk.import.Credentials
{
    /// <summary> Exception that is thrown when user can't be logged in due to invalid credentials. </summary>
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() : base("Can't login with given credentials")
        {
        }
    }
}