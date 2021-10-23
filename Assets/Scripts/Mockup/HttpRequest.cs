using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HttpRequest
{
    public static void SendStartIngame(Action<MockupSever.ResponseBase> onResponse)
    {
        var request = new MockupSever.RequestBase();
        request.protocol = ProtocolType.IngameStart;
        request.onResponse = onResponse;
        MockupSever.Instance.Send(request);
    }

    public static void SendEndIngame(bool victory, Action<MockupSever.ResponseBase> onResponse)
    {
        var request = new RequestEndIngame();
        request.protocol = ProtocolType.IngameEnd;
        request.victory = victory;
        request.onResponse = onResponse;
        MockupSever.Instance.Send(request);
    }
}

public class RequestStartIngame : MockupSever.RequestBase
{
    
}

public class RequestEndIngame : MockupSever.RequestBase
{
    public bool victory;
}
