using System.Runtime.CompilerServices;
using Unity.Services.CloudSave.Internal.Http;
using Unity.Services.CloudSave.Internal.Models;
using UnityEngine;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Unity.Services.CloudSave.Internal
{
    interface IRateLimiter
    {
        bool RateLimited { get; }
        float RetryAfter { get; }
        bool IsRateLimitException(HttpException<BasicErrorResponse> e);
        void ProcessRateLimit(HttpException<BasicErrorResponse> e);
    }

    class RateLimiter : IRateLimiter
    {
        const float k_fallbackBackoffTime = 10.0f;
        float m_rateLimitUntilUnscaledTime;

        internal RateLimiter()
        {
            // Ensure we don't *start* rate limited!
            m_rateLimitUntilUnscaledTime = 0.0f;
        }

        public bool RateLimited => Time.unscaledTime < m_rateLimitUntilUnscaledTime;

        public float RetryAfter => (Time.unscaledTime < m_rateLimitUntilUnscaledTime)
            ? m_rateLimitUntilUnscaledTime - Time.unscaledTime : 0;

        public bool IsRateLimitException(HttpException<BasicErrorResponse> e)
        {
            return !RateLimited && e.Response is { StatusCode : 429 };
        }

        public void ProcessRateLimit(HttpException<BasicErrorResponse> e)
        {
            if (IsRateLimitException(e))
            {
                m_rateLimitUntilUnscaledTime = Time.unscaledTime;

                if (e.Response.Headers.TryGetValue("Retry-After", out var timeInSeconds) &&
                    float.TryParse(timeInSeconds, out var time))
                {
                    m_rateLimitUntilUnscaledTime += time;
                }
                else if (e.Response.Headers.TryGetValue("X-Retry-After", out var timeInSecondsX) &&
                         float.TryParse(timeInSecondsX, out var timeX))
                {
                    m_rateLimitUntilUnscaledTime += timeX;
                }
                else
                {
                    m_rateLimitUntilUnscaledTime += k_fallbackBackoffTime;
                }
            }
        }
    }
}
