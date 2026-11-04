using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class objectManager : MonoBehaviour
{
    public GameObject[] availableItems;

    public List<string> registeredObject = new List<string>();

    public Transform[] PositionsRack;
    public TMP_Text logText;

    private List<GameObject> instantiatedObjects = new List<GameObject>();

    private List<int> objectsIndices = new List<int>();
    private Stack<objectAction> discardedObjects = new Stack<objectAction>();

    private class objectAction
    {
        public int indexPrefab;
        public int indexPosition;
        public string information;
    }


    public void registerObject()
    {
        if (availableItems.Length == 0 || instantiatedObjects.Count >= PositionsRack.Length )
            return;
        int number = Random.Range(0, availableItems.Length);

        int position = instantiatedObjects.Count;

        if ( position >= PositionsRack.Length)
            return;

        GameObject Object = Instantiate(
            availableItems[number],
            PositionsRack[position].position,Quaternion.identity);

        ObjectData data = Object.GetComponent<ObjectData>();

        if (data != null)
        {
            string log = "ID: " + data.identifier + " - " + data.nameObject;

            registeredObject.Add(log);
            instantiatedObjects.Add(Object);
            objectsIndices.Add(number);

            discardedObjects.Clear();

            updateText();

        }
        else
        {
            Destroy( Object );

        }


    }

    public void Undo()
    {

        if (instantiatedObjects.Count == 0)
            return;

        int last = instantiatedObjects.Count - 1;

        objectAction action = new objectAction();

        action.indexPrefab = objectsIndices[last];
        action.indexPosition = last;
        action.information = registeredObject[last];

        discardedObjects.Push(action);
        Destroy(instantiatedObjects[last]);

        instantiatedObjects.RemoveAt(last);
        objectsIndices.RemoveAt(last);
        registeredObject.RemoveAt(last);


        updateText();



    }

    public void redo()
    {
        if (discardedObjects.Count == 0)
            return;

        objectAction action = discardedObjects.Peek();
        GameObject Object = Instantiate(availableItems[action.indexPrefab], PositionsRack[action.indexPosition].position, Quaternion.identity);

        registeredObject.Add(action.information);
        instantiatedObjects.Add(Object );
        objectsIndices.Add(action.indexPrefab);

        discardedObjects.Pop();
        updateText();

    }

    void updateText()
    {
        logText.text = "REGISTERED OBJECTS:\n";

        foreach (string Object in registeredObject)
        {
            logText.text += Object + "\n";
        }
    }

}