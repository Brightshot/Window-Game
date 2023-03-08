using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WindowManager : MonoBehaviour
{
    [SerializeField]private List<GameObject> window = new List<GameObject>(3);
    private List<Windows> savedWindows = new List<Windows>(3);

    public class Windows
    {
        public int ID;
        public bool Closed;

        public Windows(int id,bool closed)
        {
            ID = id;
            Closed = closed;
        }
    }

    private void Start()
    {
        for (int i = 0; i < window.Count; i++)
        {
            savedWindows.Add(new Windows(i,false));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            for (int i = 0; i < window.Count; i++)
            {
                window[i].SetActive(true);
                savedWindows[i].Closed= false;
            }
        }
    }

    private bool stillOpen=true;
    public void WindowsClosed(int id)
    {
        window[id].SetActive(false);
        savedWindows[id].Closed = true;

        stillOpen=false;
        for (int i = 0; i < savedWindows.Count; i++)
        {
            if (savedWindows[i].Closed == false) 
            {
                stillOpen= true;
                break;
            }
        }

        CheckWindows();
    }


    private void CheckWindows()
    {
        if (!stillOpen)
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying= false;
#elif !UNITY_EDITOR
            Application.Quit();
#endif
        }
    }
}
