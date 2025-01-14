/*
Copyright (c) 2009-2010 Mikko Mononen memon@inside.org
recast4j copyright (c) 2015-2019 Piotr Piastucki piotr@jtilia.org
DotRecast Copyright (c) 2023-2024 Choi Ikpil ikpil@naver.com

This software is provided 'as-is', without any express or implied
warranty.  In no event will the authors be held liable for any damages
arising from the use of this software.
Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions:
1. The origin of this software must not be misrepresented; you must not
 claim that you wrote the original software. If you use this software
 in a product, an acknowledgment in the product documentation would be
 appreciated but is not required.
2. Altered source versions must be plainly marked as such, and must not be
 misrepresented as being the original software.
3. This notice may not be removed or altered from any source distribution.
*/


using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotRecast.Recast.Toolset.Builder
{
    public class RcBuiltInAreas
    {
        public const int POLYAREA_NOT_WALKABLE = RcRecast.RC_NULL_AREA;
        public const int POLYAREA_WALKABLE = RcRecast.RC_WALKABLE_AREA;
        public const int POLYAREA_JUMP = 2;

        public const int POLYFLAGS_NOT_WALKABLE = 1 << POLYAREA_NOT_WALKABLE; // Disabled polygon
        public const int POLYFLAGS_WALKABLE = 1 << POLYAREA_WALKABLE; // Ability to walk (ground, grass, road)
        public const int POLYFLAGS_JUMP = 1 << POLYAREA_JUMP; // Ability to jump.
        public const int POLYFLAGS_ALL = ushort.MaxValue; // All abilities.
    }

    public class RcAreaConfig
    {
        public RcArea[] Areas = new RcArea[RcRecast.RC_MAX_AREAS];

        public RcAreaConfig()
        {
            for (int i = 0; i < Areas.Length; i++)
            {
                Areas[i] = new();
            }

            Validate();
        }

        void Validate()
        {
            Areas[RcBuiltInAreas.POLYAREA_NOT_WALKABLE].Name = "Not Walkable";
            Areas[RcBuiltInAreas.POLYAREA_WALKABLE].Name = "Walkable";
            Areas[RcBuiltInAreas.POLYAREA_JUMP].Name = "Jump";
        }

        public static RcAreaConfig Load(string path)
        {
            RcAreaConfig config = null;

            try
            {
                var json = File.ReadAllText(path);
                config = JsonSerializer.Deserialize(json, MyJsonContext.Default.RcAreaConfig);
            }
            catch (FileNotFoundException)
            {
                config = new RcAreaConfig();
            }

            config.Validate();

            return config;
        }

        public static void Save(RcAreaConfig config, string path)
        {
            config.Validate();
            var json = JsonSerializer.Serialize<RcAreaConfig>(config, MyJsonContext.Default.RcAreaConfig);
            //Console.WriteLine($"save {json}");
            File.WriteAllText(path, json);
        }
    }

    public partial class RcArea
    {
        public string Name = string.Empty;
        public float Cost = 1.0f;

        [JsonIgnore]
        public bool Set => !string.IsNullOrEmpty(Name);
    }

    [JsonSourceGenerationOptions(WriteIndented = true, IncludeFields = true)]
    [JsonSerializable(typeof(RcArea))]
    [JsonSerializable(typeof(RcAreaConfig))]
    internal partial class MyJsonContext : JsonSerializerContext
    { }
}