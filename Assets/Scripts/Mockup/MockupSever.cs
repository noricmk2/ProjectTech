using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockupSever : MonoSingleton<MockupSever>
{
    public class RequestBase
    {
        public ProtocolType protocol;
        public Action<ResponseBase> onResponse;
    }

    public class ResponseBase
    {
        
    }

    public void Send(RequestBase request)
    {
        var response = CreateResponse(request);
        request.onResponse?.Invoke(response);
    }

    public ResponseBase CreateResponse(RequestBase request)
    {
        var data = new ResponseBase();
        switch (request.protocol)
        {
            case ProtocolType.IngameEnd:
                break;
            case ProtocolType.IngameStart:
                break;
        }
        return data;
    }
}
