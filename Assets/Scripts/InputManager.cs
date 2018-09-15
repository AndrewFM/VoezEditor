using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager {
    public static bool onePushed;
    public static bool twoPushed;
    public static bool threePushed;
    public static bool fourPushed;
    public static bool fivePushed;
    public static bool sixPushed;
    public static bool upPushed;
    public static bool downPushed;
    public static bool leftPushed;
    public static bool rightPushed;
    public static bool wPushed;
    public static bool aPushed;
    public static bool sPushed;
    public static bool dPushed;
    public static bool zPushed;
    public static bool xPushed;
    public static bool cPushed;
    public static bool vPushed;
    public static bool delPushed;
    public static bool spacePushed;
    public static bool returnPushed;
    public static bool leftMousePushed;
    public static bool rightMousePushed;
    public static bool middleMousePushed;
    public static bool anyPushed;

    public void Update()
    {
        UpdateKeyPushTracker(KeyCode.Alpha1, ref oneLastDown, ref oneNowDown, ref onePushed);
        UpdateKeyPushTracker(KeyCode.Alpha2, ref twoLastDown, ref twoNowDown, ref twoPushed);
        UpdateKeyPushTracker(KeyCode.Alpha3, ref threeLastDown, ref threeNowDown, ref threePushed);
        UpdateKeyPushTracker(KeyCode.Alpha4, ref fourLastDown, ref fourNowDown, ref fourPushed);
        UpdateKeyPushTracker(KeyCode.Alpha5, ref fiveLastDown, ref fiveNowDown, ref fivePushed);
        UpdateKeyPushTracker(KeyCode.Alpha6, ref sixLastDown, ref sixNowDown, ref sixPushed);
        UpdateKeyPushTracker(KeyCode.UpArrow, ref upLastDown, ref upNowDown, ref upPushed);
        UpdateKeyPushTracker(KeyCode.DownArrow, ref downLastDown, ref downNowDown, ref downPushed);
        UpdateKeyPushTracker(KeyCode.LeftArrow, ref leftLastDown, ref leftNowDown, ref leftPushed);
        UpdateKeyPushTracker(KeyCode.RightArrow, ref rightLastDown, ref rightNowDown, ref rightPushed);
        UpdateKeyPushTracker(KeyCode.W, ref wLastDown, ref wNowDown, ref wPushed);
        UpdateKeyPushTracker(KeyCode.A, ref aLastDown, ref aNowDown, ref aPushed);
        UpdateKeyPushTracker(KeyCode.S, ref sLastDown, ref sNowDown, ref sPushed);
        UpdateKeyPushTracker(KeyCode.D, ref dLastDown, ref dNowDown, ref dPushed);
        UpdateKeyPushTracker(KeyCode.Z, ref zLastDown, ref zNowDown, ref zPushed);
        UpdateKeyPushTracker(KeyCode.X, ref xLastDown, ref xNowDown, ref xPushed);
        UpdateKeyPushTracker(KeyCode.C, ref cLastDown, ref cNowDown, ref cPushed);
        UpdateKeyPushTracker(KeyCode.V, ref vLastDown, ref vNowDown, ref vPushed);
        UpdateKeyPushTracker(KeyCode.Delete, ref delLastDown, ref delNowDown, ref delPushed);
        UpdateKeyPushTracker(KeyCode.Space, ref spaceLastDown, ref spaceNowDown, ref spacePushed);
        UpdateKeyPushTracker(KeyCode.Return, ref returnLastDown, ref returnNowDown, ref returnPushed);
        UpdateMousePushTracker(0, ref mouseLeftLastDown, ref mouseLeftNowDown, ref leftMousePushed);
        UpdateMousePushTracker(1, ref mouseRightLastDown, ref mouseRightNowDown, ref rightMousePushed);
        UpdateMousePushTracker(2, ref mouseMiddleLastDown, ref mouseMiddleNowDown, ref middleMousePushed);

        anyLastDown = anyNowDown;
        if (Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
            anyNowDown = true;
        else
            anyNowDown = false;
        if (anyNowDown && !anyLastDown)
            anyPushed = true;
        else
            anyPushed = false;
    }

    public static void UpdateKeyPushTracker(KeyCode key, ref bool lastDown, ref bool nowDown, ref bool pushStatus)
    {
        lastDown = nowDown;
        if (Input.GetKey(key))
            nowDown = true;
        else
            nowDown = false;
        if (nowDown && !lastDown)
            pushStatus = true;
        else
            pushStatus = false;
    }

    public static void UpdateMousePushTracker(int button, ref bool lastDown, ref bool nowDown, ref bool pushStatus)
    {
        lastDown = nowDown;
        if (Input.GetMouseButton(button))
            nowDown = true;
        else
            nowDown = false;
        if (nowDown && !lastDown)
            pushStatus = true;
        else
            pushStatus = false;
    }

    public static bool UpTick()
    {
        return upPushed || wPushed || (!Util.ShiftDown() && Input.GetAxis("Mouse ScrollWheel") > 0);
    }

    public static bool DownTick()
    {
        return downPushed || sPushed || (!Util.ShiftDown() && Input.GetAxis("Mouse ScrollWheel") < 0);
    }

    public static bool LeftTick()
    {
        return leftPushed || aPushed || (Util.ShiftDown() && Input.GetAxis("Mouse ScrollWheel") < 0);
    }

    public static bool RightTick()
    {
        return rightPushed || dPushed || (Util.ShiftDown() && Input.GetAxis("Mouse ScrollWheel") > 0);
    }

    public static void ClearMouseInputs()
    {
        leftMousePushed = false;
        rightMousePushed = false;
        middleMousePushed = false;
    }

    private bool oneNowDown;
    private bool oneLastDown;
    private bool twoNowDown;
    private bool twoLastDown;
    private bool threeNowDown;
    private bool threeLastDown;
    private bool fourNowDown;
    private bool fourLastDown;
    private bool fiveNowDown;
    private bool fiveLastDown;
    private bool sixNowDown;
    private bool sixLastDown;

    private bool upNowDown;
    private bool upLastDown;
    private bool downNowDown;
    private bool downLastDown;
    private bool leftNowDown;
    private bool leftLastDown;
    private bool rightNowDown;
    private bool rightLastDown;

    private bool wNowDown;
    private bool wLastDown;
    private bool aNowDown;
    private bool aLastDown;
    private bool sNowDown;
    private bool sLastDown;
    private bool dNowDown;
    private bool dLastDown;

    private bool zNowDown;
    private bool zLastDown;
    private bool xNowDown;
    private bool xLastDown;
    private bool cNowDown;
    private bool cLastDown;
    private bool vNowDown;
    private bool vLastDown;

    private bool delNowDown;
    private bool delLastDown;
    private bool spaceNowDown;
    private bool spaceLastDown;
    private bool returnNowDown;
    private bool returnLastDown;

    private bool mouseLeftNowDown;
    private bool mouseLeftLastDown;
    private bool mouseRightNowDown;
    private bool mouseRightLastDown;
    private bool mouseMiddleNowDown;
    private bool mouseMiddleLastDown;

    private bool anyNowDown;
    private bool anyLastDown;
}
