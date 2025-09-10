using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrustumCutHandler : MonoBehaviour
{
    public Camera CameraBinocular;
    public float xRatio = 16;
    public float yRatio = 9;
    public float customOffset = -0.1f;
    public Transform CapturePoint;
    public PlayerController controller;

    //
    private GameObject LeftPlaneObj, RightPlaneObj, TopPlaneObj, BottomPlaneObj, FrustumObjectObj;
    //Object Property Mesh Data
    private MeshFilter leftPlaneMF, rightPlaneMF, topPlaneMF, bottomPlaneMF, frustumObjectMF;
    //Object Property Mesh Collider
    private MeshCollider leftPlaneMC, rightPlaneMC, topPlaneMC, bottomPlaneMC, frustumObjectMC;
    //
    private List<GameObject> ObjectsInLeft, ObjectsInRight, ObjectsInTop, ObjectsInBottom, ObjectsInFrustum;
    //Coordinates
    private Vector3 LeftUpFrustumPos, RightUpFrustumPos, LeftDownFrustumPos, RightDownFrustumPos, CameraPos;
    //Plane to get normal vector for cutting 
    private Plane leftPlane, rightPlane, topPlane, bottomPlane;
    
    private PolaroidFilm activeFilm;

    private GameObject ending;
    private void Awake()
    {
        ObjectsInLeft = new List<GameObject>();
        ObjectsInRight = new List<GameObject>();
        ObjectsInTop = new List<GameObject>();
        ObjectsInBottom = new List<GameObject>();
        ObjectsInFrustum = new List<GameObject>();
    }
    void Start()
    {
    //Setup Collision Detector via Plane Mesh
        //Set Name to read
        LeftPlaneObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
        LeftPlaneObj.name = "LeftCameraPlaneObj";
        RightPlaneObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
        RightPlaneObj.name = "RightCameraPlaneObj";
        TopPlaneObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
        TopPlaneObj.name = "TopCameraPlaneObj";
        BottomPlaneObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
        BottomPlaneObj.name = "BottomCameraPlaneObj";
        FrustumObjectObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
        FrustumObjectObj.name = "FrustumObjectObj";

        LeftPlaneObj.layer = LayerMask.NameToLayer("Uncuttable");
        RightPlaneObj.layer = LayerMask.NameToLayer("Uncuttable");
        TopPlaneObj.layer = LayerMask.NameToLayer("Uncuttable");
        BottomPlaneObj.layer = LayerMask.NameToLayer("Uncuttable");
        FrustumObjectObj.layer = LayerMask.NameToLayer("Uncuttable");
        
        leftPlaneMC = LeftPlaneObj.GetComponent<MeshCollider>();
        leftPlaneMC.convex = true;
        leftPlaneMC.isTrigger = true;
        leftPlaneMC.enabled = false;

        rightPlaneMC = RightPlaneObj.GetComponent<MeshCollider>();
        rightPlaneMC.convex = true;
        rightPlaneMC.isTrigger = true;
        rightPlaneMC.enabled = false;

        topPlaneMC = TopPlaneObj.GetComponent<MeshCollider>();
        topPlaneMC.convex = true;
        topPlaneMC.isTrigger = true;
        topPlaneMC.enabled = false;

        bottomPlaneMC = BottomPlaneObj.GetComponent<MeshCollider>();
        bottomPlaneMC.convex = true;
        bottomPlaneMC.isTrigger = true;
        bottomPlaneMC.enabled = false;

        frustumObjectMC = FrustumObjectObj.GetComponent<MeshCollider>();
        frustumObjectMC.convex = true;
        frustumObjectMC.isTrigger = true;
        frustumObjectMC.enabled = false;

        leftPlaneMF = LeftPlaneObj.GetComponent<MeshFilter>();
        rightPlaneMF = RightPlaneObj.GetComponent<MeshFilter>();
        topPlaneMF = TopPlaneObj.GetComponent<MeshFilter>();
        bottomPlaneMF = BottomPlaneObj.GetComponent<MeshFilter>();
        frustumObjectMF = FrustumObjectObj.GetComponent<MeshFilter>();

        leftPlaneMF.GetComponent<MeshRenderer>().enabled = false;
        rightPlaneMF.GetComponent<MeshRenderer>().enabled = false;
        topPlaneMF.GetComponent<MeshRenderer>().enabled = false;
        bottomPlaneMF.GetComponent<MeshRenderer>().enabled = false;
        frustumObjectMF.GetComponent<MeshRenderer>().enabled = false;

        var leftChecker = LeftPlaneObj.AddComponent<CollisionChecker>();
        leftChecker.frustumCutHandler = this;
        leftChecker.side = 0;

        var rightChecker = RightPlaneObj.AddComponent<CollisionChecker>();
        rightChecker.frustumCutHandler = this;
        rightChecker.side = 1;

        var topChecker = TopPlaneObj.AddComponent<CollisionChecker>();
        topChecker.frustumCutHandler = this;
        topChecker.side = 2;

        var bottomChecker = BottomPlaneObj.AddComponent<CollisionChecker>();
        bottomChecker.frustumCutHandler = this;
        bottomChecker.side = 3;

        var frustumChecker = FrustumObjectObj.AddComponent<CollisionChecker>();
        frustumChecker.frustumCutHandler = this;
        frustumChecker.side = 4;
    }

    public void Cut(bool bTakingPic)
    {
        Debug.Log("Cut occur with bTakingPic:" + bTakingPic);
        CameraPos = CapturePoint.transform.position;
        
        float DistanceToFarPlane = CameraBinocular.farClipPlane;
        float FarPlaneHeight = 2 * DistanceToFarPlane * Mathf.Tan(CameraBinocular.fieldOfView/2 * Mathf.Deg2Rad);
        float FarPlaneWidth = FarPlaneHeight * CameraBinocular.aspect;

        LeftUpFrustumPos = new Vector3(-FarPlaneWidth / 2, FarPlaneHeight / 2, DistanceToFarPlane);
        LeftDownFrustumPos = new Vector3(-FarPlaneWidth/2,-FarPlaneHeight/2, DistanceToFarPlane);
        RightUpFrustumPos = new Vector3(FarPlaneWidth/2,FarPlaneHeight/2, DistanceToFarPlane);
        RightDownFrustumPos = new Vector3(FarPlaneWidth/2,-FarPlaneHeight/2, DistanceToFarPlane);
        //Debug.Log("LeftUp: " + LeftUpFrustumPos +"; RightUp: " +  RightUpFrustumPos + "; LeftDown: "  + LeftDownFrustumPos + "; RightDown: " + RightDownFrustumPos);
        LeftUpFrustumPos = CapturePoint.TransformPoint(LeftUpFrustumPos);
        LeftDownFrustumPos = CapturePoint.TransformPoint(LeftDownFrustumPos);
        RightUpFrustumPos = CapturePoint.TransformPoint(RightUpFrustumPos);
        RightDownFrustumPos = CapturePoint.TransformPoint(RightDownFrustumPos);
        
        //Plane To get Normal vector to define cut plane in next process
        leftPlane = new Plane(CameraPos, LeftUpFrustumPos, LeftDownFrustumPos);
        rightPlane = new Plane(CameraPos, RightDownFrustumPos, RightUpFrustumPos);
        bottomPlane = new Plane(CameraPos, LeftDownFrustumPos, RightDownFrustumPos);
        topPlane = new  Plane(CameraPos, RightUpFrustumPos, LeftUpFrustumPos);
        
        var leftOffset = leftPlane.normal * customOffset;
        leftPlaneMF.mesh = CreateTriangleMesh(CameraPos, LeftUpFrustumPos, LeftDownFrustumPos,
            CameraPos + leftOffset, LeftUpFrustumPos + leftOffset, LeftDownFrustumPos + leftOffset);
        leftPlaneMC.sharedMesh = leftPlaneMF.mesh;

        var rightOffset = rightPlane.normal * customOffset;
        rightPlaneMF.sharedMesh = CreateTriangleMesh(CameraPos, RightDownFrustumPos, RightUpFrustumPos,
            CameraPos + rightOffset, RightDownFrustumPos + rightOffset,RightUpFrustumPos + rightOffset);
        rightPlaneMC.sharedMesh = rightPlaneMF.mesh;
        
        var topOffset = topPlane.normal * customOffset;
        topPlaneMF.mesh = CreateTriangleMesh(CameraPos, RightUpFrustumPos, LeftUpFrustumPos,
            CameraPos + topOffset, RightUpFrustumPos + topOffset, LeftUpFrustumPos + topOffset);
        topPlaneMC.sharedMesh = topPlaneMF.mesh;
        
        var bottomOffset = bottomPlane.normal * customOffset;
        bottomPlaneMF.mesh = CreateTriangleMesh(CameraPos, LeftDownFrustumPos, RightDownFrustumPos,
            CameraPos + bottomOffset,  LeftDownFrustumPos + bottomOffset, RightDownFrustumPos + bottomOffset);
        bottomPlaneMC.sharedMesh = bottomPlaneMF.mesh;
        //
        ObjectsInLeft.Clear();
        ObjectsInRight.Clear();
        ObjectsInTop.Clear();
        ObjectsInBottom.Clear();
        ObjectsInFrustum.Clear();

        StartCoroutine(ActualCut(bTakingPic));
    }

    private IEnumerator ActualCut(bool bTakingPic)
    {
        leftPlaneMC.enabled = true;
        rightPlaneMC.enabled = true;
        topPlaneMC.enabled = true;
        bottomPlaneMC.enabled = true;
        
        yield return new WaitForFixedUpdate();
        
        //At this point, objects overlap with cut planes added into Object Container 
        leftPlaneMC.enabled = false;
        rightPlaneMC.enabled = false;
        topPlaneMC.enabled = false;
        bottomPlaneMC.enabled = false;
        
        List<GameObject> allObjects = new List<GameObject>();
        List<GameObject> intactObjects = new List<GameObject>();

        foreach (GameObject objToCut in ObjectsInLeft)
        {
            if (bTakingPic)
            {
                string OrginalName = objToCut.name;
                objToCut.name = OrginalName + "/Cut";
                GameObject cloneObj = Instantiate(objToCut);
                cloneObj.name = OrginalName;
                cloneObj.transform.position = objToCut.transform.position;
                cloneObj.transform.position = objToCut.transform.position;
                cloneObj.SetActive(false);//hide temporarily and ensure it doesn't overlap with Frustum Volume
                intactObjects.Add(cloneObj);
            }
            if(!allObjects.Contains(objToCut)) allObjects.Add(objToCut);
            
            CutPieceHandler pieceHandler = objToCut.GetComponent<CutPieceHandler>();
            if (!pieceHandler)
            {
                pieceHandler = objToCut.AddComponent<CutPieceHandler>();
                pieceHandler.AddPiece(objToCut);//the original object is still a piece
            }
        
            GameObject newPiece = Cutter.Cut(objToCut, (LeftDownFrustumPos + LeftUpFrustumPos + CameraPos) / 3 , leftPlane.normal);
            pieceHandler.AddPiece(newPiece);
            allObjects.Add(newPiece);
        }
        
        foreach (GameObject objToCut in ObjectsInRight)
        {
            if (bTakingPic)
            {
                string[] name = objToCut.name.Split("/");
                if (name.Length == 1)//it hasn't cut
                {
                    string OrginalName = objToCut.name;
                    objToCut.name = OrginalName + "/Cut";
                    GameObject cloneObj = Instantiate(objToCut);
                    cloneObj.name = OrginalName;
                    cloneObj.transform.position = objToCut.transform.position;
                    cloneObj.transform.position = objToCut.transform.position;
                    cloneObj.SetActive(false);
                    intactObjects.Add(cloneObj);
                }
            }
            if(!allObjects.Contains(objToCut)) allObjects.Add(objToCut);
            
            CutPieceHandler pieceHandler = objToCut.GetComponent<CutPieceHandler>();
            if (!pieceHandler)
            {
                pieceHandler = objToCut.AddComponent<CutPieceHandler>();
                pieceHandler.AddPiece(objToCut);//the original object is still a piece
            }
        
            int amountPiece = pieceHandler.Pieces.Count;
            for(int i = 0; i < amountPiece; i++)
            {
                if(pieceHandler.Pieces[i] == null ) continue;
                GameObject newPiece = Cutter.Cut(pieceHandler.Pieces[i], (RightDownFrustumPos + RightUpFrustumPos + CameraPos) / 3 , rightPlane.normal);
                pieceHandler.AddPiece(newPiece);
                allObjects.Add(newPiece);
            }
        }
        
        foreach (GameObject objToCut in ObjectsInTop)
        {
            if (bTakingPic)
            {
                string[] name = objToCut.name.Split("/");
                if (name.Length == 1)//it hasn't cut
                {
                    string OrginalName = objToCut.name;
                    objToCut.name = OrginalName + "/Cut";
                    GameObject cloneObj = Instantiate(objToCut);
                    cloneObj.name = OrginalName;
                    cloneObj.transform.position = objToCut.transform.position;
                    cloneObj.transform.position = objToCut.transform.position;
                    cloneObj.SetActive(false);
                    intactObjects.Add(cloneObj);
                }
            }
            if(!allObjects.Contains(objToCut)) allObjects.Add(objToCut);
            
            CutPieceHandler pieceHandler = objToCut.GetComponent<CutPieceHandler>();
            if (!pieceHandler)
            {
                pieceHandler = objToCut.AddComponent<CutPieceHandler>();
                pieceHandler.AddPiece(objToCut);//the original object is still a piece
            }
        
            int amountPiece = pieceHandler.Pieces.Count;
            for(int i = 0; i < amountPiece; i++)
            {
                if(pieceHandler.Pieces[i] == null ) continue;
                GameObject newPiece = Cutter.Cut(pieceHandler.Pieces[i], (LeftUpFrustumPos + RightUpFrustumPos + CameraPos) / 3 , topPlane.normal);
                pieceHandler.AddPiece(newPiece);
                allObjects.Add(newPiece);
            }
        }
        
        foreach (GameObject objToCut in ObjectsInBottom)
        {
            if (bTakingPic)
            {
                string[] name = objToCut.name.Split("/");
                if (name.Length == 1)//it hasn't cut
                {
                    string OrginalName = objToCut.name;
                    objToCut.name = OrginalName + "/Cut";
                    GameObject cloneObj = Instantiate(objToCut);
                    cloneObj.name = OrginalName;
                    cloneObj.transform.position = objToCut.transform.position;
                    cloneObj.transform.position = objToCut.transform.position;
                    cloneObj.SetActive(false);
                    intactObjects.Add(cloneObj);
                }
            }
            if(!allObjects.Contains(objToCut)) allObjects.Add(objToCut);
            
            CutPieceHandler pieceHandler = objToCut.GetComponent<CutPieceHandler>();
            if (!pieceHandler)
            {
                pieceHandler = objToCut.AddComponent<CutPieceHandler>();
                pieceHandler.AddPiece(objToCut);//the original object is still a piece
            }
        
            int amountPiece = pieceHandler.Pieces.Count;
            for(int i = 0; i < amountPiece; i++)
            {
                if(pieceHandler.Pieces[i] == null ) continue;
                GameObject newPiece = Cutter.Cut(pieceHandler.Pieces[i],(RightDownFrustumPos + LeftDownFrustumPos + CameraPos) / 3 , bottomPlane.normal);
                pieceHandler.AddPiece(newPiece);
                allObjects.Add(newPiece);
            }
        }
        
        //need to add a little margin aiming inside
         frustumObjectMF.mesh = CreateFrustumObject(CameraPos + (CapturePoint.forward * -customOffset),
             RightDownFrustumPos + (rightPlane.normal * -customOffset),
             RightUpFrustumPos + (rightPlane.normal * -customOffset),
             LeftUpFrustumPos + (leftPlane.normal * -customOffset),
             LeftDownFrustumPos + (leftPlane.normal * -customOffset));
         frustumObjectMC.sharedMesh = frustumObjectMF.mesh;
         //at this point, cut objects is gonna overlap with Frustum Volume Collider.  Intact objects
         frustumObjectMC.enabled = true;
         yield return new WaitForFixedUpdate();
         frustumObjectMC.enabled = false; 
        
         if (ending != null) ObjectsInFrustum.Add(ending);
        
         if (bTakingPic) 
         {
             activeFilm = new PolaroidFilm(ObjectsInFrustum, CapturePoint);//save objects in picture for picture hold and manage in hidden visibility
        
             foreach(var intactObj in intactObjects) intactObj.SetActive(true);
             //Destroy objects contain cut pieces in all object, just only display by Intact Objects above instead of pieces
             foreach (var obj in allObjects) if (obj != null) Destroy(obj);
             
         }
         else 
         {
             if(activeFilm != null) activeFilm.ActivateFilm();//Display Placeholder Mesh Objects in frame and end relative with camera
        
             foreach(var obj in allObjects) if(obj != null) Destroy(obj.GetComponent<CutPieceHandler>());
             foreach(var obj in ObjectsInFrustum) if(obj != null) Destroy(obj);
         }
    }
    
    public void AddObjectToCut(GameObject toCut, int side)
    {
        switch (side)
        {
            case 0:
                if (!ObjectsInLeft.Contains(toCut)) ObjectsInLeft.Add(toCut);
                break;
            case 1:
                if (!ObjectsInRight.Contains(toCut)) ObjectsInRight.Add(toCut);
                break;
            case 2:
                if (!ObjectsInTop.Contains(toCut)) ObjectsInTop.Add(toCut);
                break;
            case 3:
                if (!ObjectsInBottom.Contains(toCut)) ObjectsInBottom.Add(toCut);
                break;
            case 4:
                if (!ObjectsInFrustum.Contains(toCut)) ObjectsInFrustum.Add(toCut);
                break;
        }
    }
    
    
    Mesh CreateTriangleMesh(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector3 v5, Vector3 v6)
    {
        Vector3[] vertices = new Vector3[] { v1, v2, v3, v4, v5, v6 };
        int[] triangles = new int[] {
            0, 1, 2,
            
            0, 1, 4,
            0, 4, 3,
            1, 4, 2,
            2, 4, 5,
            5, 2, 0,
            0, 5, 3,
            
            3, 4, 5
        };
        
        Mesh mesh = new Mesh { vertices = vertices, triangles = triangles };
        return mesh;
    }

    
    Mesh CreateFrustumObject(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector3 v5)
    {
        Vector3[] vertices = new Vector3[] { v1, v2, v3, v4, v5 };

        int[] triangles = new int[] {
            0, 2, 1,

            4, 1, 2,
            4, 2, 3,

            0, 4, 3,

            0, 1, 4,

            0, 3, 2,
        };

        return new Mesh { vertices = vertices, triangles = triangles };

    }
    public class PolaroidFilm
    {
        List<GameObject> placeHolders;

        public PolaroidFilm(List<GameObject> inObjectsInFrame, Transform parentToFollow)
        {
            placeHolders = new List<GameObject>();
            foreach (var obj in inObjectsInFrame)
            {
                var placeholder = GameObject.Instantiate(obj);
                placeholder.transform.position = obj.transform.position;
                placeholder.transform.rotation = obj.transform.rotation;
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
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        var aspectRatio = CameraBinocular.aspect;
        var frustumHeight = 2.0f * CameraBinocular.farClipPlane * Mathf.Tan(CameraBinocular.fieldOfView/2 * Mathf.Deg2Rad);
        var frustumWidth = frustumHeight * aspectRatio;

        var leftUpF = new Vector3(-frustumWidth/2, frustumHeight/2, CameraBinocular.farClipPlane);
        var rightUpF = new Vector3(frustumWidth/2, frustumHeight/2, CameraBinocular.farClipPlane);
        var leftDownF = new Vector3(-frustumWidth/2, -frustumHeight/2, CameraBinocular.farClipPlane);
        var rightDownF = new Vector3(frustumWidth/2, -frustumHeight/2, CameraBinocular.farClipPlane);

        leftUpF = CapturePoint.transform.TransformPoint(leftUpF);
        rightUpF = CapturePoint.transform.TransformPoint(rightUpF);
        leftDownF = CapturePoint.transform.TransformPoint(leftDownF);
        rightDownF = CapturePoint.transform.TransformPoint(rightDownF);

        Gizmos.DrawLine(CapturePoint.position, rightUpF);
        Gizmos.DrawLine(CapturePoint.position, leftUpF);
        Gizmos.DrawLine(CapturePoint.position, rightDownF);
        Gizmos.DrawLine(CapturePoint.position, leftDownF);

        Gizmos.DrawLine(leftDownF, rightDownF);
        Gizmos.DrawLine(leftUpF, rightUpF);

        Gizmos.DrawLine(leftDownF, leftUpF);
        Gizmos.DrawLine(rightDownF, rightUpF);
    }

}

public class CutPieceHandler : MonoBehaviour
{
    public List<GameObject> Pieces;

    public void AddPiece(GameObject inPiece) 
    {
        if (Pieces == null) Pieces = new List<GameObject>();
        Pieces.Add(inPiece);
    }
}

