﻿namespace Adept_AIO.Champions.LeeSin.Ward_Manager
{
    using Aimtec;

    public interface IWardTracker
    {
        bool IsAtWall { get; set; }

        float LastWardCreated { get; set; }

        string WardName { get; }
        Vector3 WardPosition { get; set; }

        bool DidJustWard { get; }
        bool IsWardReady();

        string Ward();
    }
}