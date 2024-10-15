using LunarLander.Entities.GameObjects;
using LunarLander.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace LunarLander.Assets.GameObjects
{
    // The terrain that makes up the lunar surface in game.
    public class Terrain
    {
        // Vertices that make up the triangles that fill in terrain with a grey color.
        public VertexPositionColor[] fillVerts { get; set; }

        // Describes rectangles that outline surface of the terrain.
        public DistanceAngle_Pair[] distanceAnglePairs { get; set; }

        // Color to fill the terrain area with.
        private Color fillColor = new Color(114, 126, 143);

        // Color of the terrain outline.
        private Color lineColor = new Color(255, 255, 255);

        // How thick to draw lines.
        private float thickness = 3.0f;

        // Describes the shape of the terrain.  Also utilized in collision detection.
        public List<LineEntry> lineList { get; set; }

        // Indices indicating the triangle list drawing order.
        public int[] triangleIndices { get; set; }

        // Offset values from edges of the screen.
        public const float BOTTOM_OFFSET_PERCENT = 0.05f;
        public const float TOP_OFFSET_PERCENT = 0.35f;
        public const float SIDE_SAFE_ZONE_OFFSET_PERCENT = 0.15f;

        // Length range of more difficult landing zones.
        public int smallSafeZoneLength_MIN = 120;
        public int smallSafeZoneLength_MAX = 140;

        // Length range of easy landing zones.
        public int bigSafeZoneLength_MIN = 220;
        public int bigSafeZoneLength_MAX = 270;

        // This indicates current safe zone's min and max coordinates.
        public int currentSafeZoneMin;
        public int currentSafeZoneMax;

        // Describes max X and Y position to draw terrain in.
        public int boundsMax_X { get; private set; }
        public int boundsMax_Y { get; private set; }

        // How many levels of subdivision to apply while generating the terrain features.
        public const int PROCESSING_LEVELS = 9;

        // The shader that will draw terrain vertices and triangles.
        public BasicEffect terrainShader { get; private set; }

        // Constructor
        public Terrain(Vector2 bounds)
        {
            boundsMax_X = (int)bounds.X;
            boundsMax_Y = (int)bounds.Y;

            lineList = new List<LineEntry>();

            // Initialize terrain shader.
            terrainShader = new BasicEffect(Managers.graphicsManager.graphicsDevice)
            {
                VertexColorEnabled = true,
                View = Matrix.CreateLookAt(new Vector3(0, 0, 1), Vector3.Zero, Vector3.Up),
                Projection = Matrix.CreateOrthographicOffCenter
                (
                    0,
                    Managers.graphicsManager.graphicsDevice.Viewport.Width,
                    Managers.graphicsManager.graphicsDevice.Viewport.Height,
                    0,
                    0.001f,
                    1000.0f
                ),
                World = Matrix.Identity
            };
        }

        // Generates new terrain containing given number of safe zones.
        public void GenerateTerrain(int safeZoneCount)
        {
            // Initialize the randoms.
            Random random = new Random();
            GaussianRandom gaussianRandom = new GaussianRandom();

            // Initialize vertex and triangle lists.
            List<VertexEntry> vertexList = new List<VertexEntry>();
            List<int> triangleList = new List<int>();

            // Compute the min and max elevatations possible for terrain.
            int minElevation = (int)((1.0f - BOTTOM_OFFSET_PERCENT) * boundsMax_Y);
            int maxElevation = (int)(TOP_OFFSET_PERCENT * boundsMax_Y);

            // Get random elevation.
            int elevation = random.Next(maxElevation, minElevation);

            // Define bottom-left corner vertex for base of terrain.
            vertexList.Add(new VertexEntry(new VertexPositionColor(new Vector3(0, boundsMax_Y, 0), Color.White), VertexType.NORMAL));

            // Define left-bounded vertex.  This is where terrain will root from.
            vertexList.Add(new VertexEntry(new VertexPositionColor(new Vector3(0, elevation, 0), Color.White), VertexType.NORMAL));

            // Compute length and starting left position bound of first safe zone vertex.
            int subzoneLength = (int)((boundsMax_X - 2 * SIDE_SAFE_ZONE_OFFSET_PERCENT * boundsMax_X) / safeZoneCount);
            int startingLeft = (int)(SIDE_SAFE_ZONE_OFFSET_PERCENT * boundsMax_X);

            // Step 1 - Generate safe zones.
            for (int i = 0; i < safeZoneCount; i++)
            {
                // Compute left point of the safe zone.
                int leftPointX;

                if (safeZoneCount > 1)
                {
                    leftPointX = startingLeft + random.Next(0, subzoneLength * i);
                }
                else
                {
                    startingLeft = random.Next(0, (int)(0.333f * subzoneLength));
                    leftPointX = startingLeft + random.Next(0, subzoneLength);
                }

                // Compute right point of the safe zone.
                int rightPointX = leftPointX + random.Next(currentSafeZoneMin, currentSafeZoneMax);

                // Compute random elevation
                elevation = random.Next(maxElevation, minElevation);

                // Left point
                vertexList.Add(new VertexEntry(new VertexPositionColor(new Vector3(leftPointX, boundsMax_Y, 0), fillColor), VertexType.NORMAL));
                vertexList.Add(new VertexEntry(new VertexPositionColor(new Vector3(leftPointX, elevation, 0), fillColor), VertexType.SAFE_LEFT));

                // Right point
                vertexList.Add(new VertexEntry(new VertexPositionColor(new Vector3(rightPointX, boundsMax_Y, 0), fillColor), VertexType.NORMAL));
                vertexList.Add(new VertexEntry(new VertexPositionColor(new Vector3(rightPointX, elevation, 0), fillColor), VertexType.SAFE_RIGHT));

                // Compute next safe zone's starting left point bound.
                startingLeft = subzoneLength * (i + 1);
            }

            // Setup last point in far right bound of the terrain.
            elevation = random.Next(maxElevation, minElevation);

            // Add last point of the terrain on right-bound.
            vertexList.Add(new VertexEntry(new VertexPositionColor(new Vector3(boundsMax_X, boundsMax_Y, 0), fillColor), VertexType.NORMAL));

            // Add last point at bottom-right corner.
            vertexList.Add(new VertexEntry(new VertexPositionColor(new Vector3(boundsMax_X, elevation, 0), fillColor), VertexType.NORMAL));

            // Define initial roughness factor.
            float surfaceRoughnessFactor = 1.56f;

            //Step 2 - Generate vertices around safe zones.
            for (int i = 0; i < PROCESSING_LEVELS; i++)
            {
                // Start forming the triangles that make up terrain.
                int k = 0;
                while (k < vertexList.Count - 2)
                {

                    // Get verts that appear on the surface.
                    VertexEntry leftUpperVertex = vertexList[k + 1];
                    VertexEntry rightUpperVertex = vertexList[k + 3];

                    // Skip generation of vertices if on safe zone vertices.
                    if (!(leftUpperVertex.vertexType == VertexType.SAFE_LEFT && rightUpperVertex.vertexType == VertexType.SAFE_RIGHT))
                    {
                        // Generate a random elevation based on Gaussian distribution.
                        double gaussianRandomValue = gaussianRandom.NextGaussianRandom();

                        // Utilize surface roughness factor to generate elevation relative to distance between two terrain vertices.
                        int randomElevationOffset = (int)(surfaceRoughnessFactor * gaussianRandomValue * Math.Abs(rightUpperVertex.vertex.Position.X - leftUpperVertex.vertex.Position.X));

                        // Compute midpoint between current pair of terrain vertices.
                        int midPointX = (int)(0.5f * (leftUpperVertex.vertex.Position.X + rightUpperVertex.vertex.Position.X));
                        int midPointY = (int)(0.5f * (leftUpperVertex.vertex.Position.Y + rightUpperVertex.vertex.Position.Y) + randomElevationOffset);

                        // Ensure the Y-coordinate of midpoint is within acceptable range.
                        while (midPointY < maxElevation || midPointY >= minElevation)
                        {
                            gaussianRandomValue = gaussianRandom.NextGaussianRandom();
                            randomElevationOffset = (int)(surfaceRoughnessFactor * gaussianRandomValue * Math.Abs(rightUpperVertex.vertex.Position.X - leftUpperVertex.vertex.Position.X));
                            midPointY = (int)(0.5f * (leftUpperVertex.vertex.Position.Y + rightUpperVertex.vertex.Position.Y) + randomElevationOffset);
                        }

                        // Create base point of terrain to help form a triangle.
                        VertexEntry newBasePoint = new VertexEntry(new VertexPositionColor(new Vector3(midPointX, boundsMax_Y, 0), fillColor), VertexType.NORMAL);

                        // Create new terrain point based on computed midpoint.
                        VertexEntry newPoint = new VertexEntry(new VertexPositionColor(new Vector3(midPointX, midPointY, 0), fillColor), VertexType.NORMAL);


                        // Insert vertices in appropriate spot on vertex list.
                        vertexList.Insert(k + 2, newBasePoint);
                        vertexList.Insert(k + 3, newPoint);

                        // Update offset in vertex list.
                        k += 4;
                    }
                    else
                    {
                        // Update offset in vertex list.
                        k += 2;
                    }
                }

                // Update surface roughness factor for next subdivision pass.
                surfaceRoughnessFactor += -(float)(gaussianRandom.NextGaussianRandom() / 100f + 0.155f);
            }

            // Step 3 - Connect the dots.
            for (int i = 1; i < vertexList.Count - 2; i += 2)
            {
                // Get pair of vertices to form line with.
                VertexEntry leftVertex = vertexList[i];
                VertexEntry rightVertex = vertexList[i + 2];

                // Check if line is safe zone line or not.  Mark corresponding lines with this informtion.
                if (!(leftVertex.vertexType == VertexType.SAFE_LEFT && rightVertex.vertexType == VertexType.SAFE_RIGHT))
                {
                    // Not a safe zone.
                    LineEntry line = new LineEntry(leftVertex, rightVertex, false);
                    lineList.Add(line);
                }
                else
                {
                    // Is a safe zone.
                    LineEntry line = new LineEntry(leftVertex, rightVertex, true);
                    lineList.Add(line);
                }
            }

            // Step 4 - Construct the triangles.
            for (int i = 0; i < vertexList.Count - 2; i += 2)
            {
                // Left triangle
                triangleList.Add(i); // Base Left
                triangleList.Add(i + 1); // Upper Left
                triangleList.Add(i + 3); // Upper Right

                // Right triangle
                triangleList.Add(i); // Base Left
                triangleList.Add(i + 3); // Upper Right
                triangleList.Add(i + 2); // Base Right
            }

            // Copy vertex data into the fill verts array.
            fillVerts = new VertexPositionColor[vertexList.Count];
            for (int i = 0; i < vertexList.Count; i++)
            {
                fillVerts[i] = vertexList[i].vertex;
            }

            // Convert triangle list to triange array.
            triangleIndices = triangleList.ToArray();

            // Generate the outlines on terrain surface.
            GenerateOutlines();
        }

        // Generates the data needed to draw outlines on terrain surface.
        private void GenerateOutlines()
        {
            // Make a new list of distance-angle pairs.
            distanceAnglePairs = new DistanceAngle_Pair[lineList.Count];

            for (int i = 0; i < lineList.Count; i++)
            {
                // Compute distance between vertices that make up the line.
                float distance = Vector3.Distance(lineList[i].leftVertex.vertex.Position, lineList[i].rightVertex.vertex.Position);

                // Compute the angle of the line.
                float angle = (float)Math.Atan2(lineList[i].rightVertex.vertex.Position.Y - lineList[i].leftVertex.vertex.Position.Y, lineList[i].rightVertex.vertex.Position.X - lineList[i].leftVertex.vertex.Position.X);

                // Create a new distance-angle pair and add to the distance-angle pairs list.
                DistanceAngle_Pair currentPair = new DistanceAngle_Pair(distance, angle);
                distanceAnglePairs[i] = currentPair;
            }
        }

        // Draws the outline on terrain surface.
        public void DrawOutline()
        {
            for (int i = 0; i < distanceAnglePairs.Length; i++)
            {
                Managers.graphicsManager.spriteBatch.Draw
                (
                    Managers.graphicsManager.rectColorTexture,
                    new Vector2(lineList[i].leftVertex.vertex.Position.X, lineList[i].leftVertex.vertex.Position.Y), // Position - the start point of the line
                    null, // Source rectangle (null for entire texture)
                    lineColor, // Color
                    distanceAnglePairs[i].angle, // Rotation
                    new Vector2(0, 0.5f), // Origin inside the texture (for rotation)
                    new Vector2(distanceAnglePairs[i].distance, thickness), // Scale - stretch the pixel between the points and scale its thickness
                    SpriteEffects.None,
                    0
                );
            }
        }

        // Draws the terrain shape and fills it in with given fill color.
        public void DrawFill()
        {
            foreach (EffectPass pass in terrainShader.CurrentTechnique.Passes)
            {
                pass.Apply();

                // Draw the terrain.
                Managers.graphicsManager.graphicsDevice.DrawUserIndexedPrimitives
                (
                    PrimitiveType.TriangleList,
                    fillVerts,
                    0,
                    fillVerts.Length,
                    triangleIndices,
                    0,
                    triangleIndices.Length / 3
                );
            }
        }

        // Clears all previous data about terrain in preparation in generating a new terrain.
        public void ClearData()
        {
            // Clear arrays
            fillVerts = null; // Or new VertexPositionColor[0] if you prefer to avoid nulls
            distanceAnglePairs = null; // Or new DistAngle_Pair[0] for avoiding nulls

            // Clear lists
            if (lineList != null)
            {
                lineList.Clear();
            }

            // Since triangleIndices is an array, you can also set it to null or an empty array
            triangleIndices = null; // Or new int[0] to avoid nulls
        }
    }
}
