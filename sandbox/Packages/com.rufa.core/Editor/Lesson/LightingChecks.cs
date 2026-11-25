using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Rufa.Editor
{
    /// <summary>
    /// What the lesson 8 tutorial checks in the lighthouse: the lamps baked, the room static for the light and the things that
    /// move left free, the room baked and up to date, glowing surfaces in the bake, light probes around what moves, a baked
    /// reflection probe as big as the room, the one light that must stay live, the lightmaps within their memory, and the
    /// shelf's lightmap UVs remade by hand. Nothing is cured by switching off, moving or deleting.
    /// </summary>
    public static class LightingChecks
    {
        /// <summary>Every light under the root, but the one named except, is Baked; the root is untouched, so every lamp is on and in its place.</summary>
        public static bool LightsBaked(string root, string except = null)
        {
            var go = GameObject.Find(root);
            var lights = go == null ? new Light[0] : go.GetComponentsInChildren<Light>(true).Where(l => l.name != except).ToArray();
            return lights.Length > 0 && lights.All(l => l.lightmapBakeType == LightmapBakeType.Baked) && GeometryChecks.Untouched(go);
        }

        /// <summary>
        /// Every renderer of the room has Contribute GI and Reflection Probe Static; none of the moving things has Contribute GI or
        /// Batching Static: a lightmap stays where it was baked, and a batched object cannot move. Both roots are untouched.
        /// </summary>
        public static bool StaticForLight(string room, string moving)
        {
            const StaticEditorFlags light = StaticEditorFlags.ContributeGI | StaticEditorFlags.ReflectionProbeStatic;
            const StaticEditorFlags fixedOnly = StaticEditorFlags.ContributeGI | StaticEditorFlags.BatchingStatic;
            var r = GameObject.Find(room);
            var m = GameObject.Find(moving);
            if (r == null || m == null) return false;
            var walls = r.GetComponentsInChildren<Renderer>(true);
            return walls.Length > 0 && walls.All(x => (Flags(x) & light) == light) && m.GetComponentsInChildren<Renderer>(true).All(x => (Flags(x) & fixedOnly) == 0)
                && GeometryChecks.Untouched(r) && GeometryChecks.Untouched(m);
        }

        /// <summary>
        /// The open scene has baked lightmaps, and every renderer that takes part in the bake, gets its light from the lightmaps and
        /// has something to draw has one. A new scene already has lighting data, so that alone proves nothing; an object made static
        /// after the bake has no lightmap until the next one. A renderer whose mesh is missing or empty never gets one.
        /// </summary>
        public static bool Baked()
        {
            int count = LightmapSettings.lightmaps.Length;
            return count > 0 && Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None)
                .Where(r => r.enabled && Flags(r).HasFlag(StaticEditorFlags.ContributeGI) && r.receiveGI == ReceiveGI.Lightmaps && r.scaleInLightmap > 0f
                    && r.TryGetComponent(out MeshFilter f) && f.sharedMesh != null && f.sharedMesh.vertexCount > 0)
                .All(r => r.lightmapIndex >= 0 && r.lightmapIndex < count);
        }

        /// <summary>The object glows into the bake: its material has emission on, not black, with Global Illumination on Baked, and the object takes part in the bake.</summary>
        public static bool EmissionBaked(string name) =>
            GameObject.Find(name) is GameObject go && go.TryGetComponent(out Renderer r) && Flags(r).HasFlag(StaticEditorFlags.ContributeGI)
            && r.sharedMaterials.Any(m => m != null && m.IsKeywordEnabled("_EMISSION") && m.GetColor("_EmissionColor").maxColorComponent > 0.01f
                && m.globalIlluminationFlags.HasFlag(MaterialGlobalIlluminationFlags.BakedEmissive));

        /// <summary>
        /// The scene has light probe groups with at least min probes, baked, and every renderer under the root has at least four
        /// baked probes within 2.5 m: those probes are what lights it once the lamps are baked. The root is untouched.
        /// </summary>
        public static bool ProbesAround(string root, int min)
        {
            var go = GameObject.Find(root);
            var baked = LightmapSettings.lightProbes;
            if (go == null || baked == null || baked.count < min) return false;
            if (Object.FindObjectsByType<LightProbeGroup>(FindObjectsSortMode.None).Sum(g => g.probePositions.Length) < min) return false;
            var positions = baked.positions;
            return go.GetComponentsInChildren<Renderer>(true).All(r => positions.Count(p => (p - r.bounds.center).sqrMagnitude <= 2.5f * 2.5f) >= 4)
                && GeometryChecks.Untouched(go);
        }

        /// <summary>
        /// A reflection probe of the scene is Baked, has its baked cubemap and Box Projection, its centre is inside the room, its
        /// box covers at least three quarters of the room's floor, and everything Lit in the room gets it at full strength: URP
        /// fades a probe into the sky over its Blend Distance inside the box's faces, and a mirror on a wall sits right there.
        /// </summary>
        public static bool ReflectionBaked(string room)
        {
            if (GameObject.Find(room) is not GameObject go) return false;
            var b = LightingTools.BoundsOf(go);
            var shiny = go.GetComponentsInChildren<Renderer>(true)   // Simple Lit takes no reflections: only Lit shows them
                .Where(r => r.sharedMaterials.Any(m => m != null && m.shader.name == "Universal Render Pipeline/Lit")).Select(r => r.bounds.center).ToList();
            return Object.FindObjectsByType<ReflectionProbe>(FindObjectsSortMode.None).Any(p => p.enabled && p.mode == ReflectionProbeMode.Baked
                && p.bakedTexture != null && p.boxProjection && b.Contains(p.transform.position + p.center)
                && Mathf.Min(p.size.x, b.size.x) * Mathf.Min(p.size.z, b.size.z) >= 0.75f * b.size.x * b.size.z
                && shiny.All(new Bounds(p.transform.position + p.center, p.size - 2f * p.blendDistance * Vector3.one).Contains));
        }

        /// <summary>The named light is Realtime, on, and casts no shadows: what moves stays live, and as cheap as it can be.</summary>
        public static bool LiveLight(string name) =>
            GameObject.Find(name) is GameObject go && go.TryGetComponent(out Light l) && l.enabled && l.lightmapBakeType == LightmapBakeType.Realtime && l.shadows == LightShadows.None;

        /// <summary>
        /// The open scene has baked lightmaps, and they take at most the given MB; the root is untouched, since deleting what
        /// takes room in the lightmaps would fit them too.
        /// </summary>
        public static bool LightmapsAtMost(float megabytes, string root) =>
            LightmapSettings.lightmaps.Length > 0 && WithinBudget(LightmapMegabytes(), megabytes) && GameObject.Find(root) is GameObject go && GeometryChecks.Untouched(go);

        /// <summary>
        /// The megabytes are within the budget as the Lighting window prints them, to one decimal: what reads «1.0 MB» under
        /// Generate Lighting is within 1 MB, even if it is 1,048,656 bytes.
        /// </summary>
        public static bool WithinBudget(double megabytes, double budget) => System.Math.Round(megabytes, 1, System.MidpointRounding.AwayFromZero) <= budget;

        /// <summary>The memory of the baked lightmaps (light, direction, shadow mask) in MB, as the Lighting window counts it under Generate Lighting.</summary>
        public static float LightmapMegabytes()
        {
            var storage = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.TextureUtil").GetMethod("GetStorageMemorySizeLong", BindingFlags.Static | BindingFlags.Public);
            return LightmapSettings.lightmaps.SelectMany(l => new Texture[] { l.lightmapColor, l.lightmapDir, l.shadowMask }).Where(t => t != null)
                .Sum(t => (long)storage.Invoke(null, new object[] { t })) / (1024f * 1024f);
        }

        /// <summary>
        /// The model's lightmap UVs are made by hand: the importer does not generate them, and every mesh has a second UV set inside
        /// the 0-1 square whose triangles do not overlap (sampled on a 512 x 512 grid; a few cells may fall on shared edges).
        /// </summary>
        public static bool HandMadeLightmapUvs(string modelPath)
        {
            if (AssetImporter.GetAtPath(modelPath) is not ModelImporter importer || importer.generateSecondaryUV) return false;
            var meshes = AssetDatabase.LoadAllAssetsAtPath(modelPath).OfType<Mesh>().ToArray();
            return meshes.Length > 0 && meshes.All(m => m.HasVertexAttribute(VertexAttribute.TexCoord1) && Overlap(m) <= 0.005f);
        }

        const int Grid = 512;

        // The share of the cells covered by the second UV set that more than one triangle covers; 1 when a UV is out of the square.
        static float Overlap(Mesh mesh)
        {
            var uv = new List<Vector2>();
            mesh.GetUVs(1, uv);
            if (uv.Count == 0 || uv.Any(p => p.x < -0.001f || p.y < -0.001f || p.x > 1.001f || p.y > 1.001f)) return 1f;
            var cover = new byte[Grid * Grid];
            int covered = 0, twice = 0;
            var t = mesh.triangles;
            for (int i = 0; i < t.Length; i += 3)
            {
                Vector2 a = uv[t[i]] * Grid, b = uv[t[i + 1]] * Grid, c = uv[t[i + 2]] * Grid;
                float area = Cross(b - a, c - a);
                if (Mathf.Abs(area) < 1e-6f) continue;
                int x0 = Mathf.Max(0, (int)Mathf.Min(a.x, Mathf.Min(b.x, c.x))), x1 = Mathf.Min(Grid - 1, (int)Mathf.Max(a.x, Mathf.Max(b.x, c.x)));
                int y0 = Mathf.Max(0, (int)Mathf.Min(a.y, Mathf.Min(b.y, c.y))), y1 = Mathf.Min(Grid - 1, (int)Mathf.Max(a.y, Mathf.Max(b.y, c.y)));
                for (int y = y0; y <= y1; y++)
                    for (int x = x0; x <= x1; x++)
                    {
                        var p = new Vector2(x + 0.5f, y + 0.5f);
                        float w0 = Cross(b - a, p - a), w1 = Cross(c - b, p - b), w2 = Cross(a - c, p - c);
                        if (area > 0f ? w0 < 0f || w1 < 0f || w2 < 0f : w0 > 0f || w1 > 0f || w2 > 0f) continue;
                        int n = ++cover[y * Grid + x];
                        if (n == 1) covered++;
                        else if (n == 2) twice++;
                    }
            }
            return covered == 0 ? 1f : twice / (float)covered;
        }

        static float Cross(Vector2 u, Vector2 v) => u.x * v.y - u.y * v.x;

        static StaticEditorFlags Flags(Component c) => GameObjectUtility.GetStaticEditorFlags(c.gameObject);

        public const string MeasuresTemplate =
@"# Le misure della lezione 8

Al posto di ogni segnaposto tra « » scrivi il tuo numero o il tuo testo. Le foto stanno in questa cartella.

## Il faro
Il soggiorno, Batches e luci in tempo reale prima: «da scrivere»
Il soggiorno, Batches e luci in tempo reale dopo: «da scrivere»
Quanto è durato il bake del soggiorno: «da scrivere»
L'officina, Batches prima e dopo, e le lightmap dopo il bake: «da scrivere»
La scala, Batches prima e dopo: «da scrivere»
La lanterna, Batches prima e dopo: «da scrivere»
Il curatore quale foto ha scelto, in ogni stanza, e perché: «da scrivere»

## Il laboratorio
Cosa si vedeva sullo scaffale prima, e cosa dopo: «da scrivere»
Il margine che hai dato alle isole: «da scrivere»

## Il tuo progetto
Dove hai messo la camera, e perché: «da scrivere»
Batches e luci in tempo reale prima: «da scrivere»
Cosa hai cotto, e cosa hai lasciato in tempo reale: «da scrivere»
Batches e luci in tempo reale dopo: «da scrivere»
";
    }
}
