using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomFrustumLocalSpace : MonoBehaviour
{
    public Camera finder;
    public float xRatio = 16;
    public float yRatio = 9;
    public float customOffset = 0.1f;
    public Transform capturePoint;
    public PlayerController controller;
    float aspectRatio = 1;
    GameObject leftPrimitivePlane, rightPrimitivePlane, topPrimitivePlane, bottomPrimitivePlane, frustumObject;

    MeshFilter leftPrimitivePlaneMF,
        rightPrimitivePlaneMF,
        topPrimitivePlaneMF,
        bottomPrimitivePlaneMF,
        frustumObjectMF;

    MeshCollider leftPrimitivePlaneMC,
        rightPrimitivePlaneMC,
        topPrimitivePlaneMC,
        bottomPrimitivePlaneMC,
        frustumObjectMC;

    List<GameObject> leftToCut, rightToCut, topToCut, bottomToCut, objectsInFrustum;
    Vector3 leftUpFrustum, rightUpFrustum, leftDownFrustum, rightDownFrustum, cameraPos;
    Plane leftPlane, rightPlane, topPlane, bottomPlane;
    PolaroidFilm activeFilm;
    Vector3 forwardVector;
    bool isTakingPicture;
    GameObject ending;

    void Start()
    {
        leftPrimitivePlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        leftPrimitivePlane.name = "LeftCameraPlane";
        rightPrimitivePlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        rightPrimitivePlane.name = "RightCameraPlane";
        topPrimitivePlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        topPrimitivePlane.name = "TopCameraPlane";
        bottomPrimitivePlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        bottomPrimitivePlane.name = "BottomCameraPlane";
        frustumObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        frustumObject.name = "FrustumObject";

        leftPrimitivePlaneMC = leftPrimitivePlane.GetComponent<MeshCollider>();
        leftPrimitivePlaneMC.convex = true;
        leftPrimitivePlaneMC.isTrigger = true;
        leftPrimitivePlaneMC.enabled = false;

        rightPrimitivePlaneMC = rightPrimitivePlane.GetComponent<MeshCollider>();
        rightPrimitivePlaneMC.convex = true;
        rightPrimitivePlaneMC.isTrigger = true;
        rightPrimitivePlaneMC.enabled = false;

        topPrimitivePlaneMC = topPrimitivePlane.GetComponent<MeshCollider>();
        topPrimitivePlaneMC.convex = true;
        topPrimitivePlaneMC.isTrigger = true;
        topPrimitivePlaneMC.enabled = false;

        bottomPrimitivePlaneMC = bottomPrimitivePlane.GetComponent<MeshCollider>();
        bottomPrimitivePlaneMC.convex = true;
        bottomPrimitivePlaneMC.isTrigger = true;
        bottomPrimitivePlaneMC.enabled = false;

        frustumObjectMC = frustumObject.GetComponent<MeshCollider>();
        frustumObjectMC.convex = true;
        frustumObjectMC.isTrigger = true;
        frustumObjectMC.enabled = false;

        leftPrimitivePlaneMF = leftPrimitivePlane.GetComponent<MeshFilter>();
        rightPrimitivePlaneMF = rightPrimitivePlane.GetComponent<MeshFilter>();
        topPrimitivePlaneMF = topPrimitivePlane.GetComponent<MeshFilter>();
        bottomPrimitivePlaneMF = bottomPrimitivePlane.GetComponent<MeshFilter>();
        frustumObjectMF = frustumObject.GetComponent<MeshFilter>();

        leftPrimitivePlane.GetComponent<MeshRenderer>().enabled = false;
        rightPrimitivePlane.GetComponent<MeshRenderer>().enabled = false;
        topPrimitivePlane.GetComponent<MeshRenderer>().enabled = false;
        bottomPrimitivePlane.GetComponent<MeshRenderer>().enabled = false;
        frustumObjectMF.GetComponent<MeshRenderer>().enabled = false;

        var leftChecker = leftPrimitivePlane.AddComponent<CollisionChecker>();
        leftChecker.frustumLocalSpace = this;
        leftChecker.side = 0;

        var rightChecker = rightPrimitivePlane.AddComponent<CollisionChecker>();
        rightChecker.frustumLocalSpace = this;
        rightChecker.side = 1;

        var topChecker = topPrimitivePlane.AddComponent<CollisionChecker>();
        topChecker.frustumLocalSpace = this;
        topChecker.side = 2;

        var bottomChecker = bottomPrimitivePlane.AddComponent<CollisionChecker>();
        bottomChecker.frustumLocalSpace = this;
        bottomChecker.side = 3;

        var frustumChecker = frustumObject.AddComponent<CollisionChecker>();
        frustumChecker.frustumLocalSpace = this;
        frustumChecker.side = 4;
    }


    public void AddObjectToCut(GameObject toCut, int side)
    {
        switch (side)
        {
            case 0:
                if (!leftToCut.Contains(toCut))
                    leftToCut.Add(toCut);
                break;
            case 1:
                if (!rightToCut.Contains(toCut))
                    rightToCut.Add(toCut);
                break;
            case 2:
                if (!topToCut.Contains(toCut))
                    topToCut.Add(toCut);
                break;
            case 3:
                if (!bottomToCut.Contains(toCut))
                    bottomToCut.Add(toCut);
                break;
            case 4:
                if (!objectsInFrustum.Contains(toCut))
                    objectsInFrustum.Add(toCut);
                break;
        }
    }


    public class PolaroidFilm
    {
        List<GameObject> placeHolders;

        public PolaroidFilm(List<GameObject> obj, Transform parentToFollow)
        {
            placeHolders = new List<GameObject>();
            foreach (var o in obj)
            {
                var placeholder = GameObject.Instantiate(o);
                placeholder.transform.position = o.transform.position;
                placeholder.transform.rotation = o.transform.rotation;
                placeholder.transform.SetParent(parentToFollow);
                placeholder.SetActive(false);
                placeHolders.Add(placeholder);
            }
        }

        public void ActivateFilm()
        {
            for (int i = 0; i < placeHolders.Count; i++)
            {
                placeHolders[i].transform.SetParent(null);
                placeHolders[i].SetActive(true);
            }
        }
    }
}
