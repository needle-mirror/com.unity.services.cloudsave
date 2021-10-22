using System.Runtime.CompilerServices;
using Unity.Services.Authentication.Internal;

[assembly: InternalsVisibleTo("Unity.Services.CloudSave.Tests")]

namespace Unity.Services.CloudSave
{
    internal interface IAuthentication
    {
        string GetAccessToken();
        string GetPlayerId();
    }

    internal class AuthenticationWrapper : IAuthentication
    {
        readonly IPlayerId m_PlayerId;
        readonly IAccessToken m_AccessToken;

        internal AuthenticationWrapper(IPlayerId playerId, IAccessToken accessToken)
        {
            m_PlayerId = playerId;
            m_AccessToken = accessToken;
        }

        public string GetAccessToken()
        {
            return m_AccessToken?.AccessToken;
        }

        public string GetPlayerId()
        {
            return m_PlayerId?.PlayerId;
        }
    }
}
