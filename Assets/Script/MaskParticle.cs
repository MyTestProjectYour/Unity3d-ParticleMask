using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskParticle : MonoBehaviour {
    [Header("实时更新：")]
    [SerializeField]
    private bool isUpdate = false;
    [Header("裁剪范围：")]
    [SerializeField]
    private RectTransform maskRectTr;
    //裁剪范围
    private Vector3[] clipRange = new Vector3[4];
    private List<Material> materials = new List<Material>();
    private void OnEnable() {
        SetClipRange();
        TraverseParticles(transform);
    }

    private void Update()
    {
        if (isUpdate)
        {
            foreach (Material mat in materials)
            {
                if (mat != null && maskRectTr != null)
                {
                    maskRectTr.GetWorldCorners(clipRange);
                    mat.SetFloat("_MinX", clipRange[0].x);
                    mat.SetFloat("_MaxX", clipRange[2].x);
                    mat.SetFloat("_MinY", clipRange[0].y);
                    mat.SetFloat("_MaxY", clipRange[2].y);
                }
            }
        }
    }

    /// <summary>
    /// 重新设置裁剪范围。
    /// </summary>
    /// <param name="rectTransform"></param>
    public void SetMaskRectTr(RectTransform rectTransform)
    {
        maskRectTr = rectTransform;
        SetClipRange();
        TraverseParticles(transform);
    }

    /// <summary>
    /// 设置动态更新
    /// </summary>
    /// <param name="isUpdate"></param>
    public void SetMaskUpdate(bool isUpdate)
    {
        this.isUpdate = isUpdate;
    }

    /// <summary>
    /// 初始化裁剪粒子特效范围
    /// </summary>
    private void SetClipRange() {
        if (maskRectTr == null) 
            return;
        maskRectTr.GetWorldCorners(clipRange);
    }

    /// <summary>
    /// 迭代寻找下面的例子特效组件
    /// </summary>
    /// <param name="parent"></param>
    private void TraverseParticles(Transform parent) {
        foreach (Transform child in parent) {
            SetClipRange(child);
            TraverseParticles(child);
        }
    }

    //设置所有粒子特效的裁剪范围
    private void SetClipRange(Transform child) {
        if (clipRange.Length == 0) 
            return;
        if (maskRectTr == null) 
            return; 
        Renderer r = child.GetComponent<Renderer>();
        if (r == null) 
            return; 
        Material material = r.material;
        if (material == null) 
            return;

        if (!material.shader.name.Equals("Custom/MaskAdditive")) {
            material.shader = Shader.Find("Custom/MaskAdditive");
        }

        material.SetFloat("_MinX", clipRange[0].x);
        material.SetFloat("_MaxX", clipRange[2].x);
        material.SetFloat("_MinY", clipRange[0].y);
        material.SetFloat("_MaxY", clipRange[2].y);
        materials.Add(material);
    }
}
 