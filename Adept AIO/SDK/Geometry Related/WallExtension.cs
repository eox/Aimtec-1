namespace Adept_AIO.SDK.Geometry_Related
{
    using System.Collections.Generic;
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Generic;

    class WallExtension
    {
       
        public static bool IsWallAt(Vector3 pos)
        {
            return NavMesh.WorldToCell(pos).Flags.HasFlag(NavCellFlags.Wall) || NavMesh.WorldToCell(pos).Flags.HasFlag(NavCellFlags.Building);
        }

        public static Vector3 GeneratePoint(Vector3 start, Vector3 end)
        {
            for (var i = 0; i < start.Distance(end); i++)
            {
                var newPoint = start.Extend(end, i);

                if (IsWallAt(newPoint))
                {
                  
                    return newPoint;
                }
            }
            return Vector3.Zero;
        }

        public static bool IsWall(Vector3 start, Vector3 end)
        {
            for (var i = 0; i < start.Distance(end); i++)
            {
                var newPoint = start.Extend(end, i);

                if (IsWallAt(newPoint))
                {
                    return true;
                }
            }
            return false;
        }

        public static Vector3 NearestWall(Obj_AI_Base target, float range)
        {
            for (var i = 0; i < 360; i += 40)
            {
                var dir = target.Orientation.To2D();
                var angleRad = Maths.DegreeToRadian(i);
                var rot = (target.ServerPosition.To2D() + range * dir.Rotated((float) angleRad)).To3D();

                if (!IsWallAt(rot))
                {
                    continue;
                }

                return rot;
            }

            return Vector3.Zero;
        }

        public static Vector3 FurthestWall(Obj_AI_Base target, int range) // useful for Camille
        {
            for (var i = range; i >= 360; i -= 15)
            {
                var dir = target.Orientation.To2D();
                var angleRad = Maths.DegreeToRadian(i);
                var rot = (target.ServerPosition.To2D() + range * dir.Rotated((float) angleRad)).To3D();

                if (!IsWallAt(rot))
                {
                    continue;
                }

                return rot;
            }

            return Vector3.Zero;
        }

        public static float GetWallWidth(Vector3 start, Vector3 direction, float maxWallWidth = 275)
        {
            var thickness = 0f;

            for (var i = 0; i < maxWallWidth; i++)
            {
                if (IsWallAt(start.Extend(direction, i)))
                {
                    thickness += i;
                }
                else
                {
                    return thickness;
                }
            }
            return thickness;
        }

        public static Vector3 GetBestWallHopPos(Vector3 start, float range)
        {
            var spots = new Dictionary<Vector3, float>();
            float thickness = 0f;

            for (int i = 0; i < range; i+= 1)
            {
                var end = start.Extend(Game.CursorPos, i);
                if (IsWallAt(end))
                {
                    thickness+= 10;
                    continue;
                }

                if(!spots.ContainsKey(end))
                spots.Add(end, thickness);
            }

            var spot = spots.OrderBy(x => x.Key.Distance(Game.CursorPos)).ThenBy(x => x.Value).FirstOrDefault(x => x.Value > 50 && x.Value < range);
            DebugConsole.WriteLine($"THICK: {spot.Value}", MessageState.Warn);
            return spot.Key;
        }
    }
}