using System.Text.RegularExpressions;
using UnityEngine;

public class AnimationStart : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Animator animator;

    public void Play()
    {
        animator = GetComponent<Animator>();
        animator.Play(Regex.Replace(gameObject.name.Replace("(Clone)", ""), @"\d", ""));
        
    }
    void Start()
    {
        Play();
    }

    // Update is called once per frame
}
