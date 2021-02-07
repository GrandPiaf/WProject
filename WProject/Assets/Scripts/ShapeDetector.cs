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
using TMPro;

public enum ShowType { // To display a different result in order to see the process & make adjustements if needed
    Plain_Aruco_Image,
    Sub_Aruco_Image,
    Sub_Negative_Aruco_Image,
    Sub_Negative_Aruco_Image_Gray,
    Sub_Negative_Aruco_Image_Gray_Blur,
    Sub_Negative_Aruco_Image_Gray_Binary,
    Shape_Sub_Aruco_Image
}

// Detected simple shape enumeration
enum Shape {
    None,
    Rectangle,
    Triangle,
    Circle
}

public class ShapeDetector : MonoBehaviour {


    #region UnityComponents
    public UnityEngine.UI.RawImage rawImage;
    public Texture2D tex;
    public TMP_Text speelText;
    #endregion


    #region EmguCV fields
    //Webcam & webcamFrame
    private Emgu.CV.VideoCapture webcam;
    public Mat webcamFrame;

    private Mat plainArucoImage;
    private Mat subArucoArea;
    private Mat subArucoAreaNegative;
    private Mat subArucoAreaNegativeGray;
    private Mat subArucoAreaNegativeGrayBlur;
    private Mat subArucoAreaNegativeGrayBinary;
    private Mat shapeSubArucoArea;
    #endregion


    #region EmguCV parameters
    [Range(0, 1)]
    public double approxVar = 0.04; //Best value
    public double areaThreshold = 250;
    #endregion

    // What image should be diaplayed on screen
    // Used as debug purpose
    // Could be used to adjust use settings at each launch of the game
    public ShowType showType;
    
    // Shape detected. If no shape detect, it is equal to NONE
    EShapeCombination combinationFound;

    #region Camera framerate
    public float grabDelay = 0.1f;
    private float timer;
    #endregion

    // Start is called before the first frame update
    void Start() {

        // Capture the webcam
        webcam = new Emgu.CV.VideoCapture(0, VideoCapture.API.DShow);
        webcamFrame = new Mat();

        // Add event handler to the webcam to retrieve the frame if available
        webcam.ImageGrabbed += HandleWebcamQueryFrame;

        // Turning on camera
        webcam.Start();

        // Retrieving unity display screen
        rawImage = GameObject.Find("WebcamScreen").GetComponent<UnityEngine.UI.RawImage>();
    }

    // Taking off event handler & closing the camera 
    void OnDestroy() {
        if (webcam != null) {
            webcam.ImageGrabbed -= HandleWebcamQueryFrame;

            lock (webcam) {
                //waiting for thread to finish before disposing the camera...(took a while to figure out)
                System.Threading.Thread.Sleep(1000);
                // close camera
                webcam.Stop();
                webcam.Dispose();
            }
        }

    }


    void Update() {

        timer -= Time.deltaTime;

        if (timer <= 0 && webcam.IsOpened) {
            timer = grabDelay;

            bool grabbed = webcam.Grab();

            if (!grabbed) {
                //Debug.Log("no more grab");
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                        Application.Quit();
#endif
                return;
            }
        }
    }

    
    private void HandleWebcamQueryFrame(object sender, System.EventArgs e) {

        // Retrieve the next camera frame if available
        if (webcam == null) return;
        if (webcam.IsOpened) {
            webcam.Retrieve(webcamFrame);
        }
        if (webcamFrame == null) return;
        if (webcamFrame.IsEmpty) return;

        
        lock (webcamFrame) {
            // Try to detect the shape combination on the frame
            combinationFound = GetShapeCombination(webcamFrame);

            // Convert combination to spell
            Spell spell = SpellBuilder.ConvertShapeToSpell(combinationFound);

            speelText.text = spell.type.ToString();

            // Play spell on the ennemi
            Player player = FindObjectOfType<Player>();
            if (player != null) {
                bool success = player.PlayCard(spell, GameManager.Instance.enemies[0]);
            }
            
            // Display frame on screen
            RenderImage();
        }
    }

    // Reset image & texture
    // Avoid errors in case we do not retrieve the frame but apply our process on them
    void ResetVariables() {
        plainArucoImage = new Mat();
        subArucoArea = new Mat();
        subArucoAreaNegative = new Mat();
        subArucoAreaNegativeGray = new Mat();
        subArucoAreaNegativeGrayBinary = new Mat();
        subArucoAreaNegativeGrayBlur = new Mat();
        shapeSubArucoArea = new Mat();

        // destroy existing texture
        if (tex != null) {
            Destroy(tex);
            tex = null;
        }
    }

    // The main method : retrieve the shape combination from the frame
    public EShapeCombination GetShapeCombination(Mat frame) {
        ResetVariables();

        #region Aruco
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

        // detect markers
        ArucoInvoke.DetectMarkers(frame, dictMarkers, markersCorner, markersID, parameters, rejectedCandidates);

        // Create debug image, copy & draw markers if any
        plainArucoImage = new Mat(frame.Width, frame.Height, DepthType.Cv8U, 3);
        frame.CopyTo(plainArucoImage);

        if (markersID.Size > 0) {
            ArucoInvoke.DrawDetectedMarkers(plainArucoImage, markersCorner, markersID, new MCvScalar(0, 255, 0));
        }

        //Guard : if no markers, stoppping here & returning NONE
        if (markersCorner.Size != 4) {
            return EShapeCombination.NONE;
        }

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

        // Extraction of the sub image from the bounding box
        subArucoArea = new Mat(frame, boundingBox);
        #endregion

        // Invert the image in order for teh shape detection to not detect the image border as a rectangle
        CvInvoke.BitwiseNot(subArucoArea, subArucoAreaNegative);

        // Convert image to gray scale
        Image<Bgr, byte> img = subArucoAreaNegative.ToImage<Bgr, byte>();
        CvInvoke.CvtColor(subArucoAreaNegative, subArucoAreaNegativeGray, ColorConversion.Bgr2Gray);

        // Thresholding the image to augment contrast between white and black shapes using Otzu method
        CvInvoke.Threshold(subArucoAreaNegativeGray, subArucoAreaNegativeGrayBinary, 0, 255, ThresholdType.Otsu);

        // Detecting contours on the previous image
        VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
        CvInvoke.FindContours(subArucoAreaNegativeGrayBinary, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);

        //Filter small areas as we can still have some either from noise or from aruco markers
        VectorOfVectorOfPoint filteredContours = new VectorOfVectorOfPoint();
        for (int i = 0; i < contours.Size; i++) {
            double area = CvInvoke.ContourArea(contours[i], false);
            if (area > areaThreshold) {
                filteredContours.Push(contours[i]);
            }
        }

        // If no countours or more than 2 contours found, return NONE.
        if (filteredContours.Size < 1 || filteredContours.Size > 2) {
            return EShapeCombination.NONE;
        }


        // Form polygons from contours using their perimeters
        double outisdeArea = 0;
        Shape outsideShape = Shape.None;
        Shape insideShape = Shape.None;

        for (int i = 0; i < filteredContours.Size; i++) {

            double perimeter = CvInvoke.ArcLength(filteredContours[i], true);
            VectorOfPoint approx = new VectorOfPoint();
            CvInvoke.ApproxPolyDP(filteredContours[i], approx, approxVar * perimeter, true);

            CvInvoke.DrawContours(img, filteredContours, i, new MCvScalar(0, 0, 0), 1);
            img.Draw(CvInvoke.MinAreaRect(approx), new Bgr(System.Drawing.Color.Orange), 2);
            var moments = CvInvoke.Moments(filteredContours[i]);
            int x = (int)(moments.M10 / moments.M00) - 35;
            int y = (int)(moments.M01 / moments.M00);

            double area = CvInvoke.ContourArea(filteredContours[i], false);
            Shape currentShape = Shape.None;

            // From the polygon number of sides, we retrieve the current shape
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

            // If there is multiple polygons detected, this source code
            if (area > outisdeArea) {
                outisdeArea = area;
                insideShape = outsideShape;
                outsideShape = currentShape;
            }
            else {
                insideShape = currentShape;
            }

            // Draw the shape and it's bounding box on 
            RotatedRect approxMinRect = CvInvoke.MinAreaRect(approx);
            RotatedRect minRect = CvInvoke.MinAreaRect(filteredContours[i]);
            img.Draw(minRect, new Bgr(System.Drawing.Color.Red), 2);

        }

        shapeSubArucoArea = img.Mat;

        // Return the detected shape combination depending on the outer and inner shape detected previously
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

    // Simple render method to display corret image on screen
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
            case ShowType.Sub_Negative_Aruco_Image:
                toShow = subArucoAreaNegative;
                break;
            case ShowType.Sub_Negative_Aruco_Image_Gray:
                toShow = subArucoAreaNegativeGray;
                break;
            case ShowType.Sub_Negative_Aruco_Image_Gray_Blur:
                toShow = subArucoAreaNegativeGrayBlur;
                break;
            case ShowType.Sub_Negative_Aruco_Image_Gray_Binary:
                toShow = subArucoAreaNegativeGrayBinary;
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

}
