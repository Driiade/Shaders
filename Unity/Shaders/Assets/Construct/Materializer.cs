/*Copyright(c) <2017> <Benoit Constantin ( France )>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using UnityEngine;

using System.Collections;

public class Materializer : MonoBehaviour {

    public enum Mode { NONE, MATERIALIZE_ON_AWAKE, UNMATERIALIZE_ON_AWAKE }

    [SerializeField]
    MeshRenderer m_meshRenderer;

    [Tooltip("The amount of materialization over time")]
    public AnimationCurve m_materializationCurve;

    public float m_speed = 1;
    public Mode m_mode;

    float m_materializationAmount = 0;
    Coroutine m_materializationCoroutine;


    void Awake()
    {
        switch(m_mode)
        {
            case Mode.MATERIALIZE_ON_AWAKE: StartMaterialization(); break;
            case Mode.UNMATERIALIZE_ON_AWAKE: StartUnMaterialization(); break;
        }
    }


    /// <summary>
    /// Start to construct the geometry from the beginning
    /// </summary>
    [ContextMenu("StartMaterialization")]
    public void StartMaterialization()
    {
        if (m_materializationCoroutine != null)
            StopMaterialization();

        m_materializationAmount = 0;
        m_materializationCoroutine = StartCoroutine(IMaterialization());
    }

    IEnumerator IMaterialization()
    {
        while (m_materializationAmount <= m_materializationCurve.keys[m_materializationCurve.keys.Length-1].time)
        {
            m_meshRenderer.material.SetFloat("_MaterializationAmount", m_materializationCurve.Evaluate(m_materializationAmount));
            m_materializationAmount += m_speed*Time.deltaTime;
            yield return null;
        }

        m_meshRenderer.material.SetFloat("_MaterializationAmount", m_materializationCurve.keys[m_materializationCurve.keys.Length - 1].value);
        m_materializationCoroutine = null;
    }

    /// <summary>
    /// Continue the materialization without resetting the current materialization amount value.
    /// </summary>
    [ContextMenu("ContinueMaterialization")]
    public void ContinueMaterialization()
    {
        if(m_materializationCoroutine == null)
            m_materializationCoroutine = StartCoroutine(IMaterialization());
    }

    /// <summary>
    /// Start to destroy the geometry from the end
    /// </summary>
    [ContextMenu("StartUnMaterialization")]
    public void StartUnMaterialization()
    {
        if (m_materializationCoroutine != null)
            StopMaterialization();

        m_materializationAmount = m_materializationCurve.keys[m_materializationCurve.keys.Length - 1].time;
        m_materializationCoroutine = StartCoroutine(IUnMaterialization());
    }

    IEnumerator IUnMaterialization()
    {
        while (m_materializationAmount >= 0)
        {
            m_meshRenderer.material.SetFloat("_MaterializationAmount", m_materializationCurve.Evaluate(m_materializationAmount));
            m_materializationAmount -= m_speed*Time.deltaTime;
            yield return null;
        }

        m_meshRenderer.material.SetFloat("_MaterializationAmount", m_materializationCurve.Evaluate(0));

        m_materializationCoroutine = null;
    }

    /// <summary>
    /// Continue the unMaterialization without resetting the current unMaterialization amount value.
    /// </summary>
    [ContextMenu("ContinueUnMaterialization")]
    public void ContinueUnMaterialization()
    {
        if (m_materializationCoroutine == null)
            m_materializationCoroutine = StartCoroutine(IUnMaterialization());
    }


    /// <summary>
    /// Stop the Materialization or UnMaterialization at the current value.
    /// </summary>
    [ContextMenu("StopMaterialization")]
    public void StopMaterialization()
    {
        if (m_materializationCoroutine != null)
            StopCoroutine(m_materializationCoroutine);

        m_materializationCoroutine = null;
    }
}
