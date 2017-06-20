var Utilities = Utilities || {};

Utilities.GetTimeStamp = function () {
    return new Date().getTime() | 0;
}

Utilities.Ping = function (endPoint, onPingResponse) {
    with(Utilities) {
        this.timeStamp = this.GetTimeStamp();
        this.websocket = new WebSocket(endPoint);

        this.websocket.onopen = function (evt) {
            if (onPingResponse != null) {
                onPingResponse(true, (GetTimeStamp() - this.timeStamp));
                onPingResponse = null;
                this.Close();
            }
        }.bind(this);

        this.websocket.onclose = function(evt) {
            if (onPingResponse != null) {
                onPingResponse(false, (GetTimeStamp() - this.timeStamp));
                onPingResponse = null;
                this.Close();
            }
        }.bind(this);

        this.websocket.onerror = function(evt) {
            if (onPingResponse != null) {
                onPingResponse(false, (GetTimeStamp() - this.timeStamp));
                onPingResponse = null;
                this.Close();
            }
        }.bind(this);

        this.Close = function () {
            if (this.websocket != null) {
                this.websocket.close();
                this.websocket = null;
            }
        }
    }
}