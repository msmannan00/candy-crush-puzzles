public class userSessionManager : GenericSingletonClass<userSessionManager>
{
    public string profileUsername;
    public string profileID;

    public void initialize(string profileUsername, string profileID)
    {
        this.profileUsername = profileUsername;
        this.profileID = profileID;
    }

    public void resetSession()
    {
        this.profileUsername = null;
        this.profileID = null;
    }

}

