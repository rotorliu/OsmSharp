﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Osm.Renderer.Gdi.Layers;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Tools.Math.Geo;
using Tools.Math;

namespace Osm.Renderer.Gdi.Layers
{
    public abstract class HeatLayer : GdiCustomLayer
    {
        private byte _intensity;

        private int _radius;

        protected HeatLayer(int radius, byte intensity)
        {
            _intensity = intensity;
            _radius = radius;
        }

        public override void Render(Osm.Renderer.Gdi.IGdiTarget target, Osm.Renderer.View view)
        {
            // Create new graphics surface from memory bitmap
            Graphics DrawSurface = target.Graphics;

            // get the list of heat points.
            List<GeoCoordinate> points = this.GetPoint(view.Box);

            // Traverse heat point data and draw masks for each heat point
            foreach (GeoCoordinate point in points)
            {
               PointF2D target_point = view.ConvertToTargetCoordinates(target,point);

               if (target_point[0] > 0 && target_point[0] <= target.XRes &&
                   target_point[1] > 0 && target_point[1] <= target.YRes)
               {
                   HeatPoint DataPoint = new HeatPoint((float)target_point[0], (float)target_point[1], _intensity);

                   // Render current heat point on draw surface
                   DrawHeatPoint(DrawSurface, DataPoint, _radius);
               }
            }
        }

        protected abstract List<GeoCoordinate> GetPoint(GeoCoordinateBox geoCoordinateBox);

        private void DrawHeatPoint(Graphics Canvas, HeatPoint HeatPoint, int Radius)
        {
            // Create points generic list of points to hold circumference points
            List<Point> CircumferencePointsList = new List<Point>();

            // Create an empty point to predefine the point struct used in the circumference loop
            Point CircumferencePoint;

            // Create an empty array that will be populated with points from the generic list
            Point[] CircumferencePointsArray;

            // Calculate ratio to scale byte intensity range from 0-255 to 0-1
            float fRatio = 1F / Byte.MaxValue;
            // Precalulate half of byte max value
            byte bHalf = Byte.MaxValue / 2;
            // Flip intensity on it's center value from low-high to high-low
            int iIntensity = (byte)(HeatPoint.Intensity - ((HeatPoint.Intensity - bHalf) * 2));
            // Store scaled and flipped intensity value for use with gradient center location
            float fIntensity = iIntensity * fRatio;

            // Loop through all angles of a circle
            // Define loop variable as a double to prevent casting in each iteration
            // Iterate through loop on 10 degree deltas, this can change to improve performance
            for (double i = 0; i <= 360; i += 10)
            {
                // Replace last iteration point with new empty point struct
                CircumferencePoint = new Point();

                // Plot new point on the circumference of a circle of the defined radius
                // Using the point coordinates, radius, and angle
                // Calculate the position of this iterations point on the circle
                CircumferencePoint.X = Convert.ToInt32(HeatPoint.X + Radius * Math.Cos(ConvertDegreesToRadians(i)));
                CircumferencePoint.Y = Convert.ToInt32(HeatPoint.Y + Radius * Math.Sin(ConvertDegreesToRadians(i)));

                // Add newly plotted circumference point to generic point list
                CircumferencePointsList.Add(CircumferencePoint);
            }

            // Populate empty points system array from generic points array list
            // Do this to satisfy the datatype of the PathGradientBrush and FillPolygon methods
            CircumferencePointsArray = CircumferencePointsList.ToArray();

            // Create new PathGradientBrush to create a radial gradient using the circumference points
            PathGradientBrush GradientShaper = new PathGradientBrush(CircumferencePointsArray);

            // Create new color blend to tell the PathGradientBrush what colors to use and where to put them
            ColorBlend GradientSpecifications = new ColorBlend(3);

            // Define positions of gradient colors, use intesity to adjust the middle color to
            // show more mask or less mask
            GradientSpecifications.Positions = new float[3] { 0, fIntensity, 1 };
            // Define gradient colors and their alpha values, adjust alpha of gradient colors to match intensity
            GradientSpecifications.Colors = new Color[3]
            {
                Color.FromArgb(0,  Color.White),
                Color.FromArgb(HeatPoint.Intensity, Color.Black),
                Color.FromArgb(HeatPoint.Intensity, Color.Black)
            };

            // Pass off color blend to PathGradientBrush to instruct it how to generate the gradient
            GradientShaper.InterpolationColors = GradientSpecifications;

            // Draw polygon (circle) using our point array and gradient brush
            Canvas.FillPolygon(GradientShaper, CircumferencePointsArray);
        }

        private double ConvertDegreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }

        private struct HeatPoint
        {
            public float X;
            public float Y;
            public byte Intensity;
            public HeatPoint(float iX, float iY, byte bIntensity)
            {
                X = iX;
                Y = iY;
                Intensity = bIntensity;
            }
        }
    }
}
