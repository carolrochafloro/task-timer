﻿namespace task_timer
{
    public class AppSecrets
    {
        // database
        string? host;
        int? port;
        string? database;
        string? key;
        string? password;

        // jwt
        string? ValidAudience;
        string? ValidIssuer;
        string? SecretKey;
        string? TokenValidityInMinutes;
        string? RefreshTokenValidityInMinutes;


    }
}
