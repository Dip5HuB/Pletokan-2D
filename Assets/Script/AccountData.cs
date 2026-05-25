using System;
using System.Collections.Generic;

[Serializable]
public class UserAccount
{
    public string username;
    public string password;
}

[Serializable]
public class GlobalAccountRegistry
{
    public List<UserAccount> allUsers = new List<UserAccount>();
    public string lastLoggedInUser;
}