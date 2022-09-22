using UnityEngine;

public enum PieceType
{
    o_Block_0 = 0,
    o_Jump_0 = 1,
    o_Jump_1 = 2,
    o_Jump_2 = 3,
    o_Jump_3 = 4,
    o_Jump_4 = 5,
    o_Slide_0 = 6,
    o_Slide_1 = 7,
    o_Slide_2 = 8,
    o_JumpSlide_0 = 9,
    o_JumpSlide_1 = 10,
    o_JumpSlide_2 = 11,
    None =  12
}

public class Piece : MonoBehaviour
{
    public PieceType type;
    public int visualIndex;
}