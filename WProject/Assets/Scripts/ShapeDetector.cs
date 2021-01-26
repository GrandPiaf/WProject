using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Drawing;
using System;
using Emgu.CV.Util;
using Emgu.CV.Aruco;

public enum ShowType {
    Plain_Aruco_Image,
    Sub_Aruco_Image,
    Shape_Sub_Aruco_Image
}

public class ShapeDetector : MonoBehaviour {

    // Unity components
    public UnityEngine.UI.RawImage rawImage;
    public Texture2D tex;

    [HideInInspector]
    public string fileName;

    [Range(0, 1)]
    public double approxVar = 0.04; //Best value

    public double areaThreshold = 250;

    public ShowType showType;

    /** Mat to show **/
    private Mat plainArucoImage;
    private Mat subArucoArea;
    private Mat shapeSubArucoArea;

    EShapeCombination combinationFound;


    // Start is called before the first frame update
    void Start() {
        // Retrieving unity webcam screen
        rawImage = GameObject.Find("WebcamScreen").GetComponent<UnityEngine.UI.RawImage>();
    }


    void Update() {
        combinationFound = GetShapeCombination();
        Debug.Log(combinationFound);
        RenderImage();
    }

    void ResetVariables() {
        plainArucoImage = new Mat();
        subArucoArea = new Mat();
        shapeSubArucoArea = new Mat();

        // destroy existing texture
        if (tex != null) {
            Destroy(tex);
            tex = null;
        }
    }

    Mat GetFrame() {
        return new Mat(@"..\TestImages\" + fileName); ;
    }

    EShapeCombination GetShapeCombination() {

        ResetVariables();
        Mat frame = GetFrame();
        
        
        /** ARUCO MARKERS **/

        // Markers ID
        VectorOfInt markersID = new VectorOfInt();

        // marker corners & rejected candidates
        VectorOfVectorOfPointF markersCorner = new VectorOfVectorOfPointF();
        VectorOfVectorOfPointF rejectedCandidates = new VectorOfVectorOfPointF();

        // Detector parameters for tuning the algorithm
        DetectorParameters parameters = new DetectorParameters();
        parameters = DetectorParameters.GetDefault();

        // dictionary of aruco's markers
        Dictionary dictMarkers = new Dictionary(Dictionary.PredefinedDictionaryName.Dict4X4_50);

        //// convert image
        //Mat grayFrame = new Mat(frame.Width, frame.Height, DepthType.Cv8U, 1);
        //CvInvoke.CvtColor(frame, grayFrame, ColorConversion.Bgr2Gray);

        // detect markers
        ArucoInvoke.DetectMarkers(frame, dictMarkers, markersCorner, markersID, parameters, rejectedCandidates);

        plainArucoImage = new Mat(frame.Width, frame.Height, DepthType.Cv8U, 3);
        frame.CopyTo(plainArucoImage);

        if (markersID.Size > 0) {
            ArucoInvoke.DrawDetectedMarkers(plainArucoImage, markersCorner, markersID, new MCvScalar(0, 255, 0));
        }

        //Guard
        if (markersCorner.Size != 4) {
            return EShapeCombination.NONE;
        }


        /** CARD EXTRACTION **/

        // Compute Bounding Box from aruco markers
        PointF[] pointList = new PointF[4];

        for (int i = 0; i < markersCorner.Size; i++) {

            switch (markersID[i]) {
                case 0:
                    pointList[0] = markersCorner[i][2];
                    break;
                case 1:
                    pointList[1] = markersCorner[i][3];
                    break;
                case 2:
                    pointList[2] = markersCorner[i][1];
                    break;
                case 3:
                    pointList[3] = markersCorner[i][0];
                    break;
                default:
                    throw new Exception("Wrong Aruco ID !");
            }

        }

        VectorOfPointF boxList = new VectorOfPointF(pointList);
        System.Drawing.Rectangle boundingBox = CvInvoke.BoundingRectangle(boxList);

        // Extraction
        subArucoArea = new Mat(frame, boundingBox);


        /** SHAPE DETECTION **/

        //Take negative
        Mat subArucoAreaNegative = new Mat();
        CvInvoke.BitwiseNot(subArucoArea, subArucoAreaNegative);

        Image<Bgr, byte> img = subArucoAreaNegative.ToImage<Bgr, byte>();

        Mat image = new Mat();
        CvInvoke.CvtColor(subArucoAreaNegative, image, ColorConversion.Bgr2Gray);

        VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
        CvInvoke.FindContours(image, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);

        //Filter small areas
        VectorOfVectorOfPoint filteredContours = new VectorOfVectorOfPoint();
        for (int i = 0; i < contours.Size; i++) {

            double area = CvInvoke.ContourArea(contours[i], false);
            if (area > areaThreshold) {
                filteredContours.Push(contours[i]);
            }
        }

        //Guard
        if (filteredContours.Size < 1 || filteredContours.Size > 2) {
            return EShapeCombination.NONE;
        }

        double outisdeArea = 0;
        Shape outsideShape = Shape.None;
        Shape insideShape = Shape.None;

        for (int i = 0; i < filteredContours.Size; i++) {

            double perimeter = CvInvoke.ArcLength(filteredContours[i], true);
            VectorOfPoint approx = new VectorOfPoint();
            CvInvoke.ApproxPolyDP(filteredContours[i], approx, approxVar * perimeter, true);

            CvInvoke.DrawContours(img, filteredContours, i, new MCvScalar(0, 0, 255), 1);
            img.Draw(CvInvoke.MinAreaRect(approx), new Bgr(System.Drawing.Color.Orange), 2);
            var moments = CvInvoke.Moments(filteredContours[i]);
            int x = (int)(moments.M10 / moments.M00) - 35;
            int y = (int)(moments.M01 / moments.M00);

            double area = CvInvoke.ContourArea(filteredContours[i], false);
            Shape currentShape = Shape.None;

            if (approx.Size == 3) {
                CvInvoke.PutText(img, "Triangle", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                currentShape = Shape.Triangle;
            }
            if (approx.Size == 4) {
                CvInvoke.PutText(img, "Rectangle", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                currentShape = Shape.Rectangle;
            }
            if (approx.Size > 4) {
                CvInvoke.PutText(img, "Circle", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                currentShape = Shape.Circle;
            }

            if (area > outisdeArea) {
                outisdeArea = area;
                insideShape = outsideShape;
                outsideShape = currentShape;
            } else {
                insideShape = currentShape;
            }

            RotatedRect approxMinRect = CvInvoke.MinAreaRect(approx);
            RotatedRect minRect = CvInvoke.MinAreaRect(filteredContours[i]);
            img.Draw(minRect, new Bgr(System.Drawing.Color.Silver), 2);

        }

        shapeSubArucoArea = img.Mat;

        switch (outsideShape) {
            case Shape.None:
                return EShapeCombination.NONE;

            case Shape.Rectangle:
                switch (insideShape) {
                    case Shape.None:
                        return EShapeCombination.SIMPLE_RECTANGLE;
                    case Shape.Rectangle:
                        return EShapeCombination.DOUBLE_RECTANGLE;
                    case Shape.Triangle:
                        return EShapeCombination.RECTANGLE_TRIANGLE;
                    case Shape.Circle:
                        return EShapeCombination.RECTANGLE_CIRCLE;
                }
                break;

            case Shape.Triangle:
                switch (insideShape) {
                    case Shape.None:
                        return EShapeCombination.SIMPLE_TRIANGLE;
                    case Shape.Rectangle:
                        return EShapeCombination.TRIANGLE_RECTANGLE;
                    case Shape.Triangle:
                        return EShapeCombination.DOUBLE_TRIANGLE;
                    case Shape.Circle:
                        return EShapeCombination.TRIANGLE_CIRCLE;
                }
                break;

            case Shape.Circle:
                switch (insideShape) {
                    case Shape.None:
                        return EShapeCombination.SIMPLE_CIRCLE;
                    case Shape.Rectangle:
                        return EShapeCombination.CIRCLE_RECTANGLE;
                    case Shape.Triangle:
                        return EShapeCombination.CIRCLE_TRIANGLE;
                    case Shape.Circle:
                        return EShapeCombination.DOUBLE_CIRCLE;
                }
                break;
        }

        return EShapeCombination.NONE;

    }

    void RenderImage() {

        int width = (int)rawImage.rectTransform.rect.width;
        int height = (int)rawImage.rectTransform.rect.height;

        tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

        Mat toShow = new Mat();
        switch (showType) {
            case ShowType.Plain_Aruco_Image:
                toShow = plainArucoImage;
                break;
            case ShowType.Sub_Aruco_Image:
                toShow = subArucoArea;
                break;
            case ShowType.Shape_Sub_Aruco_Image:
                toShow = shapeSubArucoArea;
                break;
        }

        // Resize mat to the texture format
        CvInvoke.Resize(toShow, toShow, new System.Drawing.Size(width, height));
        // Convert to unity texture format ( RGBA )
        CvInvoke.CvtColor(toShow, toShow, ColorConversion.Bgr2Rgba);
        // Flipping because unity texture is inverted.
        CvInvoke.Flip(toShow, toShow, FlipType.Vertical);

        // loading texture in texture object
        tex.LoadRawTextureData(toShow.ToImage<Rgba, byte>().Bytes);
        tex.Apply();

        // assigning texture to gameObject
        rawImage.texture = tex;
    }

    enum Shape {
        None,
        Rectangle,
        Triangle,
        Circle
    }

}
