using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShapeDetector))]
public class ShapeDetectorEditor : Editor
{
    string[] choices = new[] {
        "Rectangle_Simple.png",
        "Triangle_Simple.png",
        "Circle_Simple.png",
        "Rectangle_Double.png",
        "Triangle_Double.png",
        "Circle_Double.png",
        "Rectangle_Triangle.png",
        "Rectangle_Circle.png",
        "Triangle_Rectangle.png",
        "Triangle_Circle.png",
        "Circle_Rectangle.png",
        "Circle_Triangle.png"
    };
    int choiceIndex = 0;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        //ShapeDetector shapeDetect = (ShapeDetector)target;

        //choiceIndex = EditorGUILayout.Popup("Player", choiceIndex, choices);
        //shapeDetect.fileName = choices[choiceIndex];
    }
}
