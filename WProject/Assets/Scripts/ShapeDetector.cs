using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Drawing;
using System;
using Emgu.CV.Util;

public class ShapeDetector : MonoBehaviour {

    // Unity components
    public UnityEngine.UI.RawImage rawImage;
    public Texture2D tex;

    [HideInInspector]
    public string fileName;


    [Range(0, 20)]
    public double dp = 2.0;
    [Range(0, 500)]
    public double minDist = 61.0;

    // 1 / 45
    // 2 / 61

    // Start is called before the first frame update
    void Start() {
        // Retrieving unity webcam screen
        rawImage = GameObject.Find("WebcamScreen").GetComponent<UnityEngine.UI.RawImage>();
    }

    void Update() {
        Mat image = new Mat(@"D:\Cours\Gamagora\WProject\TestImages\" + fileName);
        Mat result = ProcessImage(image);

        int width = (int)rawImage.rectTransform.rect.width;
        int height = (int)rawImage.rectTransform.rect.height;

        // destroy existing texture
        if (tex != null) {
            Destroy(tex);
            tex = null;
        }

        tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // Resize mat to the texture format
        CvInvoke.Resize(result, result, new System.Drawing.Size(width, height));
        // Convert to unity texture format ( RGBA )
        CvInvoke.CvtColor(result, result, ColorConversion.Bgr2Rgba);
        // Flipping because unity texture is inverted.
        CvInvoke.Flip(result, result, FlipType.Vertical);

        // loading texture in texture object
        tex.LoadRawTextureData(result.ToImage<Rgba, byte>().Bytes);
        tex.Apply();

        // assigning texture to gameObject
        rawImage.texture = tex;
    }

    public Mat ProcessImage(Mat img) {
        using (UMat gray = new UMat())
        using (UMat cannyEdges = new UMat())
        using (Mat triangleRectangleImage = new Mat(img.Size, DepthType.Cv8U, 3)) //image to draw triangles and rectangles on
        using (Mat circleImage = new Mat(img.Size, DepthType.Cv8U, 3)) //image to draw circles on
        //using (Mat lineImage = new Mat(img.Size, DepthType.Cv8U, 3)) //image to draw lines on
        {
            //Convert the image to grayscale and filter out the noise
            CvInvoke.CvtColor(img, gray, ColorConversion.Bgr2Gray);

            //Remove noise
            CvInvoke.GaussianBlur(gray, gray, new Size(3, 3), 1);

            #region circle detection
            double cannyThreshold = 180.0;
            double circleAccumulatorThreshold = 120;
            CircleF[] circles = CvInvoke.HoughCircles(gray, HoughModes.Gradient, dp, minDist, cannyThreshold, circleAccumulatorThreshold, 0);
            #endregion

            #region Canny and edge detection
            double cannyThresholdLinking = 120.0;
            CvInvoke.Canny(gray, cannyEdges, cannyThreshold, cannyThresholdLinking);
            LineSegment2D[] lines = CvInvoke.HoughLinesP(
                cannyEdges,
                1, //Distance resolution in pixel-related units
                Math.PI / 45.0, //Angle resolution measured in radians.
                20, //threshold
                30, //min Line width
                10); //gap between lines
            #endregion

            #region Find triangles and rectangles
            List<Triangle2DF> triangleList = new List<Triangle2DF>();
            List<RotatedRect> boxList = new List<RotatedRect>(); //a box is a rotated rectangle
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint()) {
                CvInvoke.FindContours(cannyEdges, contours, null, RetrType.List,
                    ChainApproxMethod.ChainApproxSimple);
                int count = contours.Size;
                for (int i = 0; i < count; i++) {
                    using (VectorOfPoint contour = contours[i])
                    using (VectorOfPoint approxContour = new VectorOfPoint()) {
                        CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05,
                            true);
                        if (CvInvoke.ContourArea(approxContour, false) > 250
                        ) //only consider contours with area greater than 250
                        {
                            if (approxContour.Size == 3) //The contour has 3 vertices, it is a triangle
                            {
                                Point[] pts = approxContour.ToArray();
                                triangleList.Add(new Triangle2DF(
                                    pts[0],
                                    pts[1],
                                    pts[2]
                                ));
                            } else if (approxContour.Size == 4) //The contour has 4 vertices.
                              {
                                #region determine if all the angles in the contour are within [80, 100] degree
                                bool isRectangle = true;
                                Point[] pts = approxContour.ToArray();
                                LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                                for (int j = 0; j < edges.Length; j++) {
                                    double angle = Math.Abs(
                                        edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
                                    if (angle < 80 || angle > 100) {
                                        isRectangle = false;
                                        break;
                                    }
                                }

                                #endregion

                                if (isRectangle) boxList.Add(CvInvoke.MinAreaRect(approxContour));
                            }
                        }
                    }
                }
            }
            #endregion

            #region draw triangles and rectangles
            triangleRectangleImage.SetTo(new MCvScalar(0));
            foreach (Triangle2DF triangle in triangleList) {
                CvInvoke.Polylines(triangleRectangleImage, Array.ConvertAll(triangle.GetVertices(), Point.Round),
                    true, new Bgr(System.Drawing.Color.DarkBlue).MCvScalar, 2);
            }

            foreach (RotatedRect box in boxList) {
                CvInvoke.Polylines(triangleRectangleImage, Array.ConvertAll(box.GetVertices(), Point.Round), true,
                    new Bgr(System.Drawing.Color.DarkOrange).MCvScalar, 2);
            }

            //Drawing a light gray frame around the image
            CvInvoke.Rectangle(triangleRectangleImage,
                new Rectangle(Point.Empty,
                    new Size(triangleRectangleImage.Width - 1, triangleRectangleImage.Height - 1)),
                new MCvScalar(120, 120, 120));
            //Draw the labels
            CvInvoke.PutText(triangleRectangleImage, "Triangles and Rectangles", new Point(20, 20),
                FontFace.HersheyDuplex, 0.5, new MCvScalar(120, 120, 120));
            #endregion

            #region draw circles
            circleImage.SetTo(new MCvScalar(0));
            foreach (CircleF circle in circles)
                CvInvoke.Circle(circleImage, Point.Round(circle.Center), (int)circle.Radius,
                    new Bgr(System.Drawing.Color.Brown).MCvScalar, 2);

            //Drawing a light gray frame around the image
            CvInvoke.Rectangle(circleImage,
                new Rectangle(Point.Empty, new Size(circleImage.Width - 1, circleImage.Height - 1)),
                new MCvScalar(120, 120, 120));
            //Draw the labels
            CvInvoke.PutText(circleImage, "Circles", new Point(20, 20), FontFace.HersheyDuplex, 0.5,
                new MCvScalar(120, 120, 120));
            #endregion

            //#region draw lines
            //lineImage.SetTo(new MCvScalar(0));
            //foreach (LineSegment2D line in lines)
            //    CvInvoke.Line(lineImage, line.P1, line.P2, new Bgr(System.Drawing.Color.Green).MCvScalar, 2);
            ////Drawing a light gray frame around the image
            //CvInvoke.Rectangle(lineImage,
            //    new Rectangle(Point.Empty, new Size(lineImage.Width - 1, lineImage.Height - 1)),
            //    new MCvScalar(120, 120, 120));
            ////Draw the labels
            //CvInvoke.PutText(lineImage, "Lines", new Point(20, 20), FontFace.HersheyDuplex, 0.5,
            //    new MCvScalar(120, 120, 120));
            //#endregion

            Mat result = new Mat();
            //CvInvoke.VConcat(new Mat[] { img, triangleRectangleImage, circleImage, lineImage }, result);
            CvInvoke.VConcat(new Mat[] { img, triangleRectangleImage, circleImage}, result);
            return result;

            // imageDisplay
            //int nbImages_W = 2;
            //int nbImages_H = 2;

            //Image<Bgr, byte> imageImg = img.ToImage<Bgr, byte>();
            //Image<Bgr, byte> imageTriangleRectangleImage = triangleRectangleImage.ToImage<Bgr, byte>();
            //Image<Bgr, byte> imageCircleImage = circleImage.ToImage<Bgr, byte>();
            //Image<Bgr, byte> imageLineImage = lineImage.ToImage<Bgr, byte>();
            //Image<Bgr, byte> imgDisplay = new Image<Bgr, byte>(img.Width * nbImages_W, img.Height * nbImages_H);

            //// Use Mat on filters, copy to Image for pixel access
            //CopyToImage(ref imageImg, ref imgDisplay);
            //CopyToImage(ref imageTriangleRectangleImage, ref imgDisplay, img.Width);
            //CopyToImage(ref imageCircleImage, ref imgDisplay, 0, img.Height);
            //CopyToImage(ref imageLineImage, ref imgDisplay, img.Width, img.Height);

            //return imgDisplay.Mat;

        }
    }

    public void CopyToImage(ref Image<Bgr, byte> input, ref Image<Bgr, byte> dest, int offsetX = 0, int offsetY = 0) {
        for (int i = 0; i < input.Height; i++)
            for (int j = 0; j < input.Width; j++) {
                dest.Data[i + offsetY, j + offsetX, 0] = input.Data[i, j, 0];
                dest.Data[i + offsetY, j + offsetX, 1] = input.Data[i, j, 1];
                dest.Data[i + offsetY, j + offsetX, 2] = input.Data[i, j, 2];
            }
    }

    public void CopyToImage(ref Image<Gray, byte> input, ref Image<Bgr, byte> dest, int offsetX = 0, int offsetY = 0) {
        for (int i = 0; i < input.Height; i++)
            for (int j = 0; j < input.Width; j++) {
                dest.Data[i + offsetY, j + offsetX, 0] = input.Data[i, j, 0];
                dest.Data[i + offsetY, j + offsetX, 1] = input.Data[i, j, 0];
                dest.Data[i + offsetY, j + offsetX, 2] = input.Data[i, j, 0];
            }
    }
    public void CopyToImage(ref Image<Hsv, byte> input, ref Image<Bgr, byte> dest, int offsetX = 0, int offsetY = 0) {
        for (int i = 0; i < input.Height; i++)
            for (int j = 0; j < input.Width; j++) {
                dest.Data[i + offsetY, j + offsetX, 0] = input.Data[i, j, 0];
                dest.Data[i + offsetY, j + offsetX, 1] = input.Data[i, j, 1];
                dest.Data[i + offsetY, j + offsetX, 2] = input.Data[i, j, 2];
            }
    }
    public void GrayCopyChannelToImage(ref Image<Bgr, byte> input, ref Image<Bgr, byte> dest, int channel, int offsetX = 0, int offsetY = 0) {
        for (int i = 0; i < input.Height; i++)
            for (int j = 0; j < input.Width; j++) {
                dest.Data[i + offsetY, j + offsetX, 0] = input.Data[i, j, channel];
                dest.Data[i + offsetY, j + offsetX, 1] = input.Data[i, j, channel];
                dest.Data[i + offsetY, j + offsetX, 2] = input.Data[i, j, channel];
            }
    }
    public void GrayCopyChannelToImage(ref Image<Hsv, byte> input, ref Image<Bgr, byte> dest, int channel, int offsetX = 0, int offsetY = 0) {
        for (int i = 0; i < input.Height; i++)
            for (int j = 0; j < input.Width; j++) {
                dest.Data[i + offsetY, j + offsetX, 0] = input.Data[i, j, channel];
                dest.Data[i + offsetY, j + offsetX, 1] = input.Data[i, j, channel];
                dest.Data[i + offsetY, j + offsetX, 2] = input.Data[i, j, channel];
            }
    }

    public void CopyChannelToImage(ref Image<Bgr, byte> input, ref Image<Bgr, byte> dest, int channel, int offsetX = 0, int offsetY = 0) {
        for (int i = 0; i < input.Height; i++)
            for (int j = 0; j < input.Width; j++) {
                dest.Data[i + offsetY, j + offsetX, channel] = input.Data[i, j, channel];
            }
    }
}
