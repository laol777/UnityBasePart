using UnityEngine;
using System.Collections;

public class HandTracker : MonoBehaviour{

    static nuitrack.HandTracker handTracker;

    static nuitrack.UserHands[] hands;
    public static nuitrack.UserHands[] Hands { get { return hands; } }

    static nuitrack.UserHands currentHands;
    public static nuitrack.UserHands СurrentHands { get { return currentHands; } }

    HandTracker()
    {
        handTracker = nuitrack.HandTracker.Create();
        handTracker.OnUpdateEvent += HandleOnHandsUpdateEvent;
        Debug.Log("___HandTracker.Init() success.");
    }

    static void HandleOnHandsUpdateEvent(nuitrack.HandTrackerData handTrackerData)
    {
        if (handTrackerData == null) return;

        hands = handTrackerData.UsersHands;

        if (NuitrackManager.currentUser != 0)
        {
            currentHands = handTrackerData.GetUserHandsByID(NuitrackManager.currentUser);
        }
        else
        {
            currentHands = null;
        }
    }
}
