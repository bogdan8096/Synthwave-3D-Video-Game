using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentManager : MonoBehaviour
{
    public static SegmentManager Instance { set; get; }

    public bool showCollider = true;

    //segment spawning
    private const float distanceBeforeSpawn = 100.0f;
    private const int initialSegmentsAmount = 5;
    private const int initialTransitionsAmount = 2;
    private const int maxSegmentsOnScreen = 10;

    private Transform cameraContainer;

    private int amountOfActiveSegments;
    private int continuousSegments;
    private int currentSpawnPositionZ;
    //private int currentLevel;
    private Vector3 segmentPosition;

    //list of pieces
    public List<Piece> block_0 = new List<Piece>();
    public List<Piece> jump_0 = new List<Piece>();
    public List<Piece> jump_1 = new List<Piece>();
    public List<Piece> jump_2 = new List<Piece>();
    public List<Piece> jump_3 = new List<Piece>();
    public List<Piece> jump_4 = new List<Piece>();
    public List<Piece> slide_0 = new List<Piece>();
    public List<Piece> slide_1 = new List<Piece>();
    public List<Piece> slide_2 = new List<Piece>();
    public List<Piece> jumpslide_0 = new List<Piece>();
    public List<Piece> jumpslide_1 = new List<Piece>();
    public List<Piece> jumpslide_2 = new List<Piece>();

    [HideInInspector]
    public List<Piece> pieces = new List<Piece>();

    //list of segments
    public List<Segment> availableSegments = new List<Segment>();
    public List<Segment> availableTransitions = new List<Segment>();
    [HideInInspector]
    public List<Segment> segments = new List<Segment>();

    //gameplay
    //private bool isMoving = false;

    private void Awake()
    {
        Instance = this;
        cameraContainer = Camera.main.transform;

        currentSpawnPositionZ = 0;
        //currentLevel = 0;
    }

    private void Start()
    {
        for (int i = 0; i < initialSegmentsAmount; i++)
        {
            if (i < initialTransitionsAmount){
                //SpawnTransition();
                InitialTransition();
            }
            else{
                GenerateSegment();
            }
        }
    }

    private void Update()
    {
        if ((currentSpawnPositionZ - cameraContainer.position.z) < distanceBeforeSpawn)
        {
            GenerateSegment();
        }

        if (amountOfActiveSegments >= maxSegmentsOnScreen)
        {
            segments[amountOfActiveSegments - 1].SegmentDespawn();
            amountOfActiveSegments--;
        }
    }
    private void GenerateSegment()
    {
        SpawnSegment();

        if (Random.Range(0f, 1f) < (continuousSegments * 0.25f))
        {
            //spawn transition segment
            continuousSegments = 0;
            SpawnTransition();

        }
        else
        {
            continuousSegments++;
        }
    }


    private void SpawnSegment()
    {
        List<Segment> possibleSegment = availableSegments.FindAll(s => s.segmentBegin.x == segmentPosition.x || s.segmentBegin.y == segmentPosition.y || s.segmentBegin.z == segmentPosition.z);
        int segmentID = Random.Range(0, possibleSegment.Count);

        Segment segment = GetSegment(segmentID, false);

        segmentPosition.x = segment.segmentEnd.x;
        segmentPosition.y = segment.segmentEnd.y;
        segmentPosition.z = segment.segmentEnd.z;

        segment.transform.SetParent(transform);
        segment.transform.localPosition = Vector3.forward * currentSpawnPositionZ;

        currentSpawnPositionZ += segment.segmentLength;
        amountOfActiveSegments++;
        segment.SegmentSpawn();
    }

    private void SpawnTransition()
    {
        List<Segment> possibleTransition = availableTransitions.FindAll(t => t.segmentBegin.x == segmentPosition.x || t.segmentBegin.y == segmentPosition.y || t.segmentBegin.z == segmentPosition.z);
        int transitionID = Random.Range(0, possibleTransition.Count);

        Segment transition = GetSegment(transitionID, true);

        segmentPosition.x = transition.segmentEnd.x;
        segmentPosition.y = transition.segmentEnd.y;
        segmentPosition.z = transition.segmentEnd.z;

        transition.transform.SetParent(transform);
        transition.transform.localPosition = Vector3.forward * currentSpawnPositionZ;

        currentSpawnPositionZ += transition.segmentLength;
        amountOfActiveSegments++;
        transition.SegmentSpawn();
    }

    private void InitialTransition()
    {
        int transitionID = 12;

        Segment transition = GetSegment(transitionID, true);

        segmentPosition.x = transition.segmentEnd.x;
        segmentPosition.y = transition.segmentEnd.y;
        segmentPosition.z = transition.segmentEnd.z;

        transition.transform.SetParent(transform);
        transition.transform.localPosition = Vector3.forward * currentSpawnPositionZ;

        currentSpawnPositionZ += transition.segmentLength;
        amountOfActiveSegments++;
        transition.SegmentSpawn();
    }

    public Segment GetSegment(int segmentID, bool transition)
    {
        Segment segment = null;
        segment = segments.Find(s => s.SegmentID == segmentID && s.segmentTransition == transition && !s.gameObject.activeSelf);

        if (segment == null)
        {
            GameObject segmentObject = Instantiate((transition) ? availableTransitions[segmentID].gameObject : availableSegments[segmentID].gameObject) as GameObject;

            segment = segmentObject.GetComponent<Segment>();

            segment.SegmentID = segmentID;
            segment.segmentTransition = transition;

            segments.Insert(0, segment);
        }
        else
        {
            segments.Remove(segment);
            segments.Insert(0, segment);
        }

        return segment;
    }

    public Piece GetPiece(PieceType pt, int vi)
    {
        Piece piece = pieces.Find(p => p.type == pt && p.visualIndex == vi && !p.gameObject.activeSelf);

        if (piece == null)
        {
            GameObject pieceObject = null;


            switch(pt)
            {
                case PieceType.o_Block_0:
                    pieceObject = block_0[vi].gameObject;
                    break;
                case PieceType.o_Jump_0:
                    pieceObject = jump_0[vi].gameObject;
                    break;
                case PieceType.o_Jump_1:
                    pieceObject = jump_1[vi].gameObject;
                    break;
                case PieceType.o_Jump_2:
                    pieceObject = jump_2[vi].gameObject;
                    break;
                case PieceType.o_Jump_3:
                    pieceObject = jump_3[vi].gameObject;
                    break;
                case PieceType.o_Jump_4:
                    pieceObject = jump_4[vi].gameObject;
                    break;
                case PieceType.o_Slide_0:
                    pieceObject = slide_0[vi].gameObject;
                    break;
                case PieceType.o_Slide_1:
                    pieceObject = slide_1[vi].gameObject;
                    break;
                case PieceType.o_Slide_2:
                    pieceObject = slide_2[vi].gameObject;
                    break;
                case PieceType.o_JumpSlide_0:
                    pieceObject = jumpslide_0[vi].gameObject;
                    break;
                case PieceType.o_JumpSlide_1:
                    pieceObject = jumpslide_1[vi].gameObject;
                    break;
                case PieceType.o_JumpSlide_2:
                    pieceObject = jumpslide_2[vi].gameObject;
                    break;
            }

            pieceObject = Instantiate(pieceObject);
            piece = pieceObject.GetComponent<Piece>();
            pieces.Add(piece);
        }

        return piece;
    }

}
