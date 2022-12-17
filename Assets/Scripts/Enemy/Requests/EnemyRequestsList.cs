using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Requests Lists", menuName = "Request List")]
public class EnemyRequestsList : ScriptableObject
{
    public int minimumActiveRequests;
    [SerializeField]
    private List<Request> requestContainer = new List<Request>(); // checks for unique requests
    private List<Request> inactiveRequests = new List<Request>(); // shuffle the requests between these two containers
    private List<Request> activeRequests = new List<Request>();
    public Request SelectRandomRequest() {
        // updates avaiable requests if necessary
        // the request must be unique, no two requests can be the same in the game

        if (!IsInactiveRequestAvailable()) {
            // this is also a limiter to the number of active enemies
            // this is checked already, second test
            Debug.Log("EnemyRequestsList: No more unique requests: Need to add: Returning null");
            return null;
        }
        // selects a random request from the list of available requests
        int randInt = Random.Range(0, inactiveRequests.Count - 1);
        Request randRequest = inactiveRequests[randInt];
        activeRequests.Add(randRequest); // resizes available requests.
        inactiveRequests.RemoveAt(randInt);
        return randRequest;
    }
    public void RestoreRequestToPool(Request _request) {
        activeRequests.Remove(_request); 
        inactiveRequests.Add(_request);
    }
    public void Clear() {
        // clears the requestContainers
        requestContainer.Clear();
        inactiveRequests.Clear();
        activeRequests.Clear();
    }
    public void AddRequest(Request _request) {
        // adds a new request that is generated at runtime, into the container
        for (int i = 0; i <_request.itemObjects.Length; i++) {
            if (_request.itemObjects[i] == null) {
                Debug.LogWarning("EnemyRequestList: null request: aborting addRequest");
                return;
            }
        }
        requestContainer.Add(_request);
        inactiveRequests.Add(_request); 
    }
    public bool IsInactiveRequestAvailable() {
        return inactiveRequests.Count != 0;
    }
    public void RemoveRequest(Request _request) {
        // remove the request from the main request container
        requestContainer.Remove(_request); 
    }
    public bool HasRequest(Request request) {
        // whether the request container has that request
        for (int i = 0; i < requestContainer.Count; i++) {
            if (requestContainer[i].IsEqual(request)) return true;
        }
        return false;
    }
}
[System.Serializable]
public class Request
{
    public ItemObject[] itemObjects;
    public Request(ItemObject[] _itemObjects) {
        itemObjects = _itemObjects;
    }
    public bool IsEqual(Request r){
        if (r == null) return false;

        List<ItemObject> temp = new List<ItemObject>(this.itemObjects);
        for (int i = 0; i < r.itemObjects.Length; i++) {
            if (!temp.Remove(r.itemObjects[i])) return false;
        }
        return true;
    }
}
