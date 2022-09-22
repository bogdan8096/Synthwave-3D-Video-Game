using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    public PieceType type;
    private Piece currentPiece;

    public void PieceSpawn()
    {
        int objectAmount = 0;
        switch(type)
        {
            case PieceType.o_Block_0:
                objectAmount = SegmentManager.Instance.block_0.Count;
                break;
            case PieceType.o_Jump_0:
                objectAmount = SegmentManager.Instance.jump_0.Count;
                break;
            case PieceType.o_Jump_1:
                objectAmount = SegmentManager.Instance.jump_1.Count;
                break;
            case PieceType.o_Jump_2:
                objectAmount = SegmentManager.Instance.jump_2.Count;
                break;
            case PieceType.o_Jump_3:
                objectAmount = SegmentManager.Instance.jump_3.Count;
                break;
            case PieceType.o_Jump_4:
                objectAmount = SegmentManager.Instance.jump_4.Count;
                break;
            case PieceType.o_Slide_0:
                objectAmount = SegmentManager.Instance.slide_0.Count;
                break;
            case PieceType.o_Slide_1:
                objectAmount = SegmentManager.Instance.slide_1.Count;
                break;
            case PieceType.o_Slide_2:
                objectAmount = SegmentManager.Instance.slide_2.Count;
                break;
            case PieceType.o_JumpSlide_0:
                objectAmount = SegmentManager.Instance.jumpslide_0.Count;
                break;
            case PieceType.o_JumpSlide_1:
                objectAmount = SegmentManager.Instance.jumpslide_1.Count;
                break;
            case PieceType.o_JumpSlide_2:
                objectAmount = SegmentManager.Instance.jumpslide_2.Count;
                break;
        }
        currentPiece = SegmentManager.Instance.GetPiece(type, Random.Range(0, objectAmount));
        currentPiece.gameObject.SetActive(true);
        currentPiece.transform.SetParent(transform, false);
    }

    public void PieceDespawn()
    {
        currentPiece.gameObject.SetActive(false);
    }
}
