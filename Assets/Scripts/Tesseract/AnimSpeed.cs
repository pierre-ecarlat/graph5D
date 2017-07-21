using UnityEngine;
using System.Collections;

public class AnimSpeed : MonoBehaviour
{
    public Animation anim;

    void Start() {
        anim["tesseract anim"].speed = 0.2f;      
    }
}