using UnityEngine;
using System.Collections.Generic;

public class ysCharacterRim
{
     Material m_characterRimRed ;
     Material m_characterRimBlue ;

    public  Material SetRimRed(GameObject player)
    {
        SkinnedMeshRenderer render = player.GetComponentInChildren<SkinnedMeshRenderer>();
        Material[] originMats = render.sharedMaterials;
        Material[] newMats = new Material[originMats.Length + 1];
        originMats.CopyTo(newMats, 0);
        if (m_characterRimRed==null)
        {
            m_characterRimRed= GameObject.Instantiate(Resources.Load("ysCharacterRimRed")) as Material;
        }
        newMats[newMats.Length - 1] = m_characterRimRed;
        render.sharedMaterials = newMats;
        return m_characterRimRed;
    }
    public Material SetRimBlue(GameObject player)
    {

        SkinnedMeshRenderer render = player.GetComponentInChildren<SkinnedMeshRenderer>();
        Material[] originMats = render.sharedMaterials;
        Material[] newMats = new Material[originMats.Length + 1];
        originMats.CopyTo(newMats, 0);
        if (m_characterRimBlue == null)
        {
            m_characterRimBlue = GameObject.Instantiate(Resources.Load("ysCharacterRimBlue")) as Material;
        }
        newMats[newMats.Length - 1] = m_characterRimBlue;
        render.sharedMaterials = newMats;
        return m_characterRimBlue;
    }
    public void RemoveRimEffect(GameObject player)
    {
        SkinnedMeshRenderer render = player.GetComponentInChildren<SkinnedMeshRenderer>();
        Material[] originMats = render.sharedMaterials;
        List<Material> newMatList = new List<Material>();
        for (int i = 0; i < originMats.Length; i++)
        {

            if (originMats[i] != null&&!originMats[i].name.Contains("ysCharacterRim"))
            {
                newMatList.Add(originMats[i]);
            }
        }
        render.sharedMaterials = newMatList.ToArray();
    }
    public void SetIntensity(float intence)
    {
        if (m_characterRimRed!=null)
        {
            m_characterRimRed.SetFloat("_RimIntensity", intence);
        }
        if (m_characterRimBlue != null)
        {
            m_characterRimBlue.SetFloat("_RimIntensity", intence);
        }
    }
    public void SetRange(float range)
    {
        if (m_characterRimRed != null)
        {
            m_characterRimRed.SetFloat("_RimWidth", range);
        }
        if (m_characterRimBlue != null)
        {
            m_characterRimBlue.SetFloat("_RimWidth", range);
        }
    }
    public void Dispose()
    {
         m_characterRimRed = null;
         m_characterRimBlue = null;
    }
    ~ysCharacterRim()
    {
        Dispose();
    }
    //[UnityEditor.MenuItem("test/add rim")]
    //public static void testrim()
    //{
    //    GameObject player = GameObject.Find("test") as GameObject;

    //    ysCharacterRim rim = new ysCharacterRim();
    //    rim. SetRimRed(player);
    //    rim.SetIntensity(100f);
    //}

    //[UnityEditor.MenuItem("test/remove rim")]
    //public static void testrim2()
    //{
    //    GameObject player = GameObject.Find("test") as GameObject;

    //    ysCharacterRim rim = new ysCharacterRim();
    //    rim.RemoveRimEffect(player);
    //}
}
