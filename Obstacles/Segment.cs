using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    public int SegmentID { set; get; }
    public bool segmentTransition;

    public int segmentLength;
    public Vector3 segmentBegin;
    public Vector3 segmentEnd;

    private PieceSpawner[] pieces;

    private void Awake()
    {
        pieces = gameObject.GetComponentsInChildren<PieceSpawner>();

        if (SegmentManager.Instance.showCollider)
        {
            
            for (int i = 0; i < pieces.Length; i++)
            {
                foreach (MeshRenderer mr in pieces[i].GetComponentsInChildren<MeshRenderer>())
                {
                    mr.enabled = false;
                }
            }
        }
    }

    public void SegmentSpawn()
    {
        gameObject.SetActive(true);
        for (int i = 0; i < pieces.Length; i++)
        {
            pieces[i].PieceSpawn();
        }
    }
    public void SegmentDespawn()
    {
        gameObject.SetActive(false);
        for (int i = 0; i < pieces.Length; i++)
        {
            pieces[i].PieceDespawn();
        }
    }
}
