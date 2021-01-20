using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Drawing;
using System;
using Emgu.CV.Util;

public class Test : MonoBehaviour
{
    // Webcam
    private Emgu.CV.VideoCapture webcam;
    private Mat webcamFrame;

    // Unity components
    public UnityEngine.UI.RawImage rawImage;
    public Texture2D tex;


    public float grabDelay = 0.1f;
    private float timer;

    void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 5000;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("starting webcam");
        // Launch webcam capture
        // Manière Sans webcam (video)
        //webcam = new Emgu.CV.VideoCapture("D:\\Programmes\\UnityWorspace\\video.mp4");
        // Manière Avec Webcam (flux de la webcam)
        webcam = new Emgu.CV.VideoCapture(0, VideoCapture.API.DShow);
        webcamFrame = new Mat();


        // Add event handler to the webcam
        webcam.ImageGrabbed += HandleWebcamQueryFrame;
        // Demarage de la webcam
        webcam.Start();

        // Retrieving unity webcam screen
        rawImage = GameObject.Find("WebcamScreen").GetComponent<UnityEngine.UI.RawImage>();
    }


    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0 && webcam.IsOpened)
        {
            timer = grabDelay;

            bool grabbed = webcam.Grab();

            if (!grabbed)
            {
                Debug.Log("no more grab");
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                        Application.Quit();
#endif
                return;
            }
        }
    }

    private void DisplayFrameOnPlane()
    {
        if (webcamFrame == null) return;
        if (webcamFrame.IsEmpty) return;

        int width = (int)rawImage.rectTransform.rect.width;
        int height = (int)rawImage.rectTransform.rect.height;

        // destroy existing texture
        if (tex != null)
        {
            Destroy(tex);
            tex = null;
        }

        // creating new texture to hold our frame
        tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // Resize mat to the texture format
        CvInvoke.Resize(webcamFrame, webcamFrame, new System.Drawing.Size(width, height));
        // Convert to unity texture format ( RGBA )
        CvInvoke.CvtColor(webcamFrame, webcamFrame, ColorConversion.Bgr2Rgba);
        // Flipping because unity texture is inverted.
        CvInvoke.Flip(webcamFrame, webcamFrame, FlipType.Vertical);

        // loading texture in texture object
        tex.LoadRawTextureData(webcamFrame.ToImage<Rgba, byte>().Bytes);
        tex.Apply();

        // assigning texture to gameObject
        rawImage.texture = tex;
    }

    private void HandleWebcamQueryFrame(object sender, System.EventArgs e)
    {

        if (webcam == null) return;
        if (webcam.IsOpened)
        {
            webcam.Retrieve(webcamFrame);
        }
        if (webcamFrame == null) return;
        if (webcamFrame.IsEmpty) return;

        Debug.Log(webcamFrame.Rows + " " + webcamFrame.Height);

        //// we access data, to not cause double access, use locks !
        lock (webcamFrame)
        {
            DisplayFrameOnPlane();

            // webcamFrame = ProcessImage(webcamFrame);
        }

        //System.Threading.Thread.Sleep(5);
    }

    void OnDestroy()
    {
        Debug.Log("entering destroy");

        if (webcam != null)
        {
            webcam.ImageGrabbed -= HandleWebcamQueryFrame;

            lock (webcam)
            {
                Debug.Log("sleeping");
                //waiting for thread to finish before disposing the camera...(took a while to figure out)
                System.Threading.Thread.Sleep(1000);
                // close camera
                webcam.Stop();
                webcam.Dispose();
            }
        }

        Debug.Log("Destroying webcam");
    }

    public Mat ProcessImage(Mat img)
    {
        using (UMat gray = new UMat())
        using (UMat cannyEdges = new UMat())
        using (Mat triangleRectangleImage = new Mat(img.Size, DepthType.Cv8U, 3)) //image to draw triangles and rectangles on
        using (Mat circleImage = new Mat(img.Size, DepthType.Cv8U, 3)) //image to draw circles on
        using (Mat lineImage = new Mat(img.Size, DepthType.Cv8U, 3)) //image to drtaw lines on
        {
            //Convert the image to grayscale and filter out the noise
            CvInvoke.CvtColor(img, gray, ColorConversion.Bgr2Gray);

            //Remove noise
            CvInvoke.GaussianBlur(gray, gray, new Size(3, 3), 1);

            #region circle detection
            double cannyThreshold = 180.0;
            double circleAccumulatorThreshold = 120;
            CircleF[] circles = CvInvoke.HoughCircles(gray, HoughModes.Gradient, 2.0, 20.0, cannyThreshold,
                circleAccumulatorThreshold, 5);
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
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(cannyEdges, contours, null, RetrType.List,
                    ChainApproxMethod.ChainApproxSimple);
                int count = contours.Size;
                for (int i = 0; i < count; i++)
                {
                    using (VectorOfPoint contour = contours[i])
                    using (VectorOfPoint approxContour = new VectorOfPoint())
                    {
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
                            }
                            else if (approxContour.Size == 4) //The contour has 4 vertices.
                            {
                                #region determine if all the angles in the contour are within [80, 100] degree
                                bool isRectangle = true;
                                Point[] pts = approxContour.ToArray();
                                LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                                for (int j = 0; j < edges.Length; j++)
                                {
                                    double angle = Math.Abs(
                                        edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
                                    if (angle < 80 || angle > 100)
                                    {
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
            foreach (Triangle2DF triangle in triangleList)
            {
                CvInvoke.Polylines(triangleRectangleImage, Array.ConvertAll(triangle.GetVertices(), Point.Round),
                    true, new Bgr(System.Drawing.Color.DarkBlue).MCvScalar, 2);
            }

            foreach (RotatedRect box in boxList)
            {
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

            #region draw lines
            lineImage.SetTo(new MCvScalar(0));
            foreach (LineSegment2D line in lines)
                CvInvoke.Line(lineImage, line.P1, line.P2, new Bgr(System.Drawing.Color.Green).MCvScalar, 2);
            //Drawing a light gray frame around the image
            CvInvoke.Rectangle(lineImage,
                new Rectangle(Point.Empty, new Size(lineImage.Width - 1, lineImage.Height - 1)),
                new MCvScalar(120, 120, 120));
            //Draw the labels
            CvInvoke.PutText(lineImage, "Lines", new Point(20, 20), FontFace.HersheyDuplex, 0.5,
                new MCvScalar(120, 120, 120));
            #endregion

            Mat result = new Mat();
            CvInvoke.VConcat(new Mat[] { img, triangleRectangleImage, circleImage, lineImage }, result);
            return result;
        }
    }
}
