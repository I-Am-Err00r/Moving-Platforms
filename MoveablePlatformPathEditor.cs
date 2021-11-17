using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//This script will make the paths visibile in the editor
[CustomEditor(typeof(MoveablePlatform), true)]
public class MoveablePlatformPathEditor : Editor
{
    //Method that updates component values based on changes in the Unity Editor
    protected virtual void OnSceneGUI()
    {
        //Sets up an int variable for whichever path is next in the iteration
        int next = new int();
        //Sets up a color variable so that any Editor tool this script creates can have a visible color
        Handles.color = Color.magenta;
        //Finds the object in the scene that is going receive the Editor tools
        MoveablePlatform platform = target as MoveablePlatform;
        //For loop to count the number of paths based on the MoveablePlatform script
        for (int i = 0; i < platform.numberOfPoints.Count; i++)
        {
            //Sets up a Vector3 variable for whichever point it is within the overall path
            Vector3 position = platform.numberOfPoints[i];
            //Sets up the next variable value to the appropriate value
            next = i + 1;
            //if it is at the end of the iteration from the MoveablePlatform script, it resets next back to 0
            if (next == platform.numberOfPoints.Count)
            {
                next = 0;
            }
            //Creates a visual representation of the point within the path as a Sphere at the position defined by the path
            position = Handles.FreeMoveHandle(position, Quaternion.identity, .5f, new Vector3(.5f, .5f, .5f), Handles.SphereHandleCap);
            //Draws a dotted line in between points to visually represent where the platform will move to based on the current point and the next point
            Handles.DrawDottedLine(platform.numberOfPoints[i], platform.numberOfPoints[next], 5);
            //Adds text to each point to represent what value in the iteration it is within the path; easily allows you to determine which point it is within the path
            Handles.Label(platform.numberOfPoints[i], "Point: " + i.ToString());
            //Checks to see if there aren't any changes within the Editor so it can redraw points and path lines when changes are finished
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "MoveablePlatform");
                platform.numberOfPoints[i] = position;
            }
        }
    }
}