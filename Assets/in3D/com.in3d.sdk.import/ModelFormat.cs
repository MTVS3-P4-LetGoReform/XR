namespace com.in3d.sdk.import
{
    /// <summary>
    /// files format and special format of avatars
    /// </summary>
    public enum ModelFormat
    {
        /// <summary>
        /// GLTF (GL Transmission Format) binary file format, textured, rigged
        /// </summary>
        Glb = 0,

        /// <summary>
        /// FBX (Filmbox) file format, textured, rigged
        /// </summary>
        Fbx = 1,
        // /// <summary>
        // /// GLTF (GL Transmission Format) binary file format, in static pose for virtual fitting room, textured
        // /// </summary>
        // Vto = 2,
        // /// <summary>
        // /// Digital Asset Exchange file format, rigged, packed in zip archive with textures and animations
        // /// </summary>
        // Dae,
        //
        // //Torso Obsolete
        //
        // /// <summary>
        // /// Undressed version of avatar in glb file format, textured, rigged.
        // /// Null if scan of user already stripped or scan created without undress config
        // /// </summary>
        // UndressedGlb,
        // /// <summary>
        // /// Undressed version of avatar in in static pose for virtual fitting room, textured.
        // /// Null if scan of user already stripped or scan created without undress config
        // /// </summary>
        // [Obsolete]
        // UndressedVto
    }

    public static class ModelFormatExt
    {
        private static readonly string[] Names = new[] { "glb", "fbx", "vto", "dae", "T_undressed", "vto_undressed" };

        public static string ToBackendString(this ModelFormat format)
        {
            return Names[(int)format];
        }
    }
}