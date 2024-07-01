using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BookmarkManager : MonoBehaviour
{
    public OpenBookmark[] Bookmarks = new OpenBookmark[3];
    public static BookmarkManager BMref;
    private void Awake()
    {
        if(BMref == null)
        {
            BMref = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void addBookmark(string _seed)
    {
        for (int i = 0; i < Bookmarks.Length; i++)
        {
            if (Bookmarks[i].Empty)
            {
                Bookmarks[i].markedSeed = _seed;
                Bookmarks[i].Empty = false;
                return;
               
            }
        }
        Debug.Log("No Open Slots");

    }
    public void removeBookmark(int _index)
    {
        if (_index >= Bookmarks.Length)
        {
            Debug.Log("_index is over max bookmarks");
            return;
        }
        if (!Bookmarks[_index].Empty)
        {
            Bookmarks[_index].Empty = true;
            Bookmarks[_index].markedSeed = "";

        }
    }
}
