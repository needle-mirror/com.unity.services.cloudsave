using System;
using Unity.Services.CloudSave.Internal.Http;
using Unity.Services.CloudSave.Internal.Models;
using UnityEngine;

namespace Unity.Services.CloudSave
{
    internal interface IRateLimiter
    {
        bool RateLimited { get; }
        float RetryAfter { get; }
        bool IsRateLimitException(HttpException<BasicErrorResponse> e);
        void ProcessRateLimit(HttpException<BasicErrorResponse> e);
    }

    internal class RateLimiter : IRateLimiter
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
            return !RateLimited && e.Response is {StatusCode : 429};
        }

        public void ProcessRateLimit(HttpException<BasicErrorResponse> e)
        {
            if (IsRateLimitException(e))
            {
                m_rateLimitUntilUnscaledTime = Time.unscaledTime;

                if (e.Response.Headers.TryGetValue("Retry-After", out string timeInSeconds) &&
                    Single.TryParse(timeInSeconds, out float time))
                {
                    m_rateLimitUntilUnscaledTime += time;
                }
                else if (e.Response.Headers.TryGetValue("X-Retry-After", out string timeInSecondsX) &&
                         Single.TryParse(timeInSecondsX, out float timeX))
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
