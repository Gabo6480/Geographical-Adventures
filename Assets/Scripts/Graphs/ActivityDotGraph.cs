using TMPro;
using Unity.Android.Gradle.Manifest;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent (typeof(RectTransform))]
public class ActivityDotGraph : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Vector4 padding = Vector4.zero;
    [SerializeField] private float size = 10f;
    [SerializeField] private int rows = 10;
    [SerializeField] private int columns = 10;
    [Header("References")]
    [SerializeField] private RectTransform dotParent;
    [SerializeField] private TMP_Text[] Numbers;
    [Space]
    [SerializeField] private DotGraphData[] data;

    private Image[,] circles;

    private void Awake()
    {
        CleanChildren();
    }

    private void FixedUpdate()
    {
        GenerateRandomData();
        UpdateGraph();
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        EditorApplication.delayCall += RevalidateChildren;
    }
#endif

    private void GenerateRandomData()
    {
        if (dotParent == null)
        {
            Debug.LogError("Dot Parent Reference Missing!");
            return;
        }
        if (data == null || data.Length != rows * columns) data = new DotGraphData[rows * columns];
        for (int i = 0; i < rows * columns; i++)
        {
            data[i] = new DotGraphData(Random.value, Random.value);
        }
    }
    private Image UpdateCircle(Image? circleImage, Vector2 anchoredPosition, string name, Color color, float size)
    {
        if(circleImage == null)
        {
            GameObject circleGameObject = new GameObject(name, typeof(Image));
            circleGameObject.transform.SetParent(dotParent, false);
            circleImage = circleGameObject.GetComponent<Image>();
            var circleRectTranform = circleGameObject.GetComponent<RectTransform>();
            circleRectTranform.anchoredPosition = anchoredPosition;
            circleRectTranform.sizeDelta = new Vector2(size, size);
            circleRectTranform.anchorMin = Vector2.zero;
            circleRectTranform.anchorMax = Vector2.zero;
        }
        circleImage.sprite = circleSprite;
        circleImage.color = color;

        return circleImage;
    }

    [ContextMenu("Clean Children")]
    void CleanChildren()
    {
        if (dotParent == null)
        {
            Debug.LogError("Dot Parent Reference Missing!");
            return;
        }
        int numChildren = dotParent.childCount;
        for (int i = numChildren - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(dotParent.GetChild(i).gameObject);
        }
        circles = null;
    }

    private void RevalidateChildren()
    {
        CleanChildren();
        UpdateGraph();

        EditorApplication.delayCall -= RevalidateChildren;
    }

    [ContextMenu("Test")]
    void Test()
    {
        CleanChildren();

        GenerateRandomData();

        UpdateGraph();
    }

    [ContextMenu("Update Graph")]
    void UpdateGraph()
    {
        if(dotParent == null)
        {
            Debug.LogError("Dot Parent Reference Missing!");
            return;
        }

        float rowDelta = (dotParent.rect.height - padding.y - padding.w) / ((float)rows);
        float columnDelta = (dotParent.rect.width - padding.x - padding.z) / ((float)columns);

        if(circles == null || circles.GetLength(0) != rows || circles.GetLength(1) != columns) circles = new Image[rows,columns];

        for (int j = 0; j < columns; j++)
        {
            for (int i = 0; i < rows; i++)
            {
                var item = data[i + j * rows];

                var color = gradient.Evaluate(item.value);
                color.a = item.intensity;
                circles[i, j] = UpdateCircle(
                    circles[i, j],
                    new Vector2(
                        padding.x + columnDelta * ((float)j + 0.5f), 
                        padding.y + rowDelta * ((float)i + 0.5f)), 
                    $"circle{i}x{j}",
                    color, 
                    size);
            }
        }
    }
}
