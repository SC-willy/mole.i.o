
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Supercent.MoleIO.InGame;

public class GooglePlayManager : IStartable
{
    public void StartSetup()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        SignIn();
    }

    public void SignIn()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // string name = PlayGamesPlatform.Instance.GetUserDisplayName();
            // string id = PlayGamesPlatform.Instance.GetUserId();
            // string ImgUrl = PlayGamesPlatform.Instance.GetUserImageUrl();
        }
    }


}
