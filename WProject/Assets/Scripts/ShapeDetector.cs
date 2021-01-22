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
    string oldFileName;

    [Range(0, 1)]
    public double approxVar = 0.04; //Best value


    // Start is called before the first frame update
    void Start() {
        // Retrieving unity webcam screen
        rawImage = GameObject.Find("WebcamScreen").GetComponent<UnityEngine.UI.RawImage>();
    }

    void Update() {
        //ProcessImage();
        if (oldFileName != fileName) {
            ProcessImage();
            oldFileName = fileName;
        }
    }

    void ProcessImage() {
        Mat image = new Mat(@"..\TestImages\" + fileName);
        Mat negative = new Mat();
        CvInvoke.BitwiseNot(image, negative);
        Mat result = GetRectangles(negative);

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

    public Mat GetRectangles(Mat src) {

        Image<Bgr, byte> img = src.ToImage<Bgr, byte>();

        Mat image = new Mat();
        CvInvoke.CvtColor(src, image, ColorConversion.Bgr2Gray);

        VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();

        CvInvoke.FindContours(image, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);

        for (int i = 0; i < contours.Size; i++) {
            double perimeter = CvInvoke.ArcLength(contours[i], true);
            VectorOfPoint approx = new VectorOfPoint();
            CvInvoke.ApproxPolyDP(contours[i], approx, approxVar * perimeter, true);

            CvInvoke.DrawContours(img, contours, i, new MCvScalar(0, 0, 255), 1);
            img.Draw(CvInvoke.MinAreaRect(approx), new Bgr(System.Drawing.Color.Orange), 2);
            var moments = CvInvoke.Moments(contours[i]);
            int x = (int)(moments.M10 / moments.M00) - 35;
            int y = (int)(moments.M01 / moments.M00);

            //double area = CvInvoke.ContourArea(contours[i], false);
            //Debug.Log(area);

            if (approx.Size == 3) {
                CvInvoke.PutText(img, "Triangle", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
            }
            if (approx.Size == 4) {
                CvInvoke.PutText(img, "Rectangle", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
            }
            //if (approx.Size == 6) {
            //    CvInvoke.PutText(img, "Hexagon", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
            //}
            if (approx.Size > 4) {
                CvInvoke.PutText(img, "Circle", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
            }

            RotatedRect approxMinRect = CvInvoke.MinAreaRect(approx);
            RotatedRect minRect = CvInvoke.MinAreaRect(contours[i]);
            img.Draw(minRect, new Bgr(System.Drawing.Color.Silver), 2);
        }
        return img.Mat;
    }

}
