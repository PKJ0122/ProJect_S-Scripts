using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossroadsNode : NodeBase , IPassby
{
    bool selectNextNode = false;


    private void Start()
    {
        CrossRoadDirectionButton.leftButton.onButtonClick += value =>
        {
            SetNextNode(value);
            selectNextNode = true;
        };
        CrossRoadDirectionButton.rightButton.onButtonClick += value =>
        {
            SetNextNode(value);
            selectNextNode = true;
        };
        UIManager.instance.Get<UICrossRoadSelect>().onCrossRoadPopupHide += () =>
        {
            selectNextNode = false;
        };
    }

    public IEnumerator PassBy(int gamblerId)
    {
        UIManager.instance.Get<UICrossRoadSelect>().Show();
        NodeInfo node = GetComponent<NodeInfo>();
        CrossRoadDirectionButton.leftButton.gameObject.SetActive(true);
        CrossRoadDirectionButton.leftButton.gameObject.transform.position =
            Vector3.Lerp(transform.position, node.nextNodes[0].transform.position, 0.5f);
        CrossRoadDirectionButton.leftButton.transform.LookAt(transform);
        CrossRoadDirectionButton.leftButton.transform.position += new Vector3(0, 1.5f, 0);

        CrossRoadDirectionButton.rightButton.gameObject.SetActive(true);
        CrossRoadDirectionButton.rightButton.gameObject.transform.position =
            Vector3.Lerp(transform.position, node.nextNodes[1].transform.position, 0.5f);
        CrossRoadDirectionButton.rightButton.transform.LookAt(transform);
        CrossRoadDirectionButton.rightButton.transform.position += new Vector3(0, 1.5f, 0);

        yield return new WaitUntil(() => selectNextNode);
        CrossRoadDirectionButton.leftButton.gameObject.SetActive(false);
        CrossRoadDirectionButton.rightButton.gameObject.SetActive(false);
        UIManager.instance.Get<UICrossRoadSelect>().Hide();
    }

    private void SetNextNode(int value)
    {
        NodeInfo node = GetComponent<NodeInfo>();
        node.nextNode = node.nextNodes[value];
    }
}
