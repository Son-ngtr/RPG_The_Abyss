using System;
using UnityEngine;

[Serializable]
public class UI_TreeConnectDetails
{
    public UI_TreeConnectionHandler childNode;
    public NodeConnectionType direction;

    [Range(100f, 350f)]
    public float length;
}

public class UI_TreeConnectionHandler : MonoBehaviour
{
    [SerializeField] private UI_TreeConnectDetails[] connectionDetails;
    [SerializeField] private UI_TreeConnection[] connections;

    private RectTransform rect => GetComponent<RectTransform>();

    private void OnValidate()
    {
        if (connectionDetails.Length <= 0)
        {
            return;
        }

        if (connectionDetails.Length != connections.Length)
        {
            Debug.Log("Amount of details need to be the same as connections. -" + gameObject.name);
            return;
        }

        UpdateConnection();
    }

    private void UpdateConnection()
    {
        for (int i = 0; i < connectionDetails.Length; i++)
        {
            var detail = connectionDetails[i];
            var connection = connections[i];

            connection.DirectConnection(detail.direction, detail.length);
            // Connect child node position = detail.GetConnectionPoint(connection.childNodeConnectionPoint);
            Vector2 targetPosition = connection.GetConnectionPoint(rect);
            detail.childNode?.SetPosition(targetPosition);
        }
    }

    public void SetPosition(Vector2 position) => rect.anchoredPosition = position;
}
