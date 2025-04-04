﻿namespace DAL;

public static class FileHelper
{
    public static string BasePath = Environment
                                        .GetFolderPath(Environment.SpecialFolder.UserProfile) 
                                    + Path.DirectorySeparatorChar + "tic-tac-two" + Path.DirectorySeparatorChar;

    public static string ConfigExtension = ".config.json";
    public static string GameExtension = ".game.json";

    public static string UsersJson = "users.json";
}