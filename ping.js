var Utilities = Utilities || {};

Utilities.GetTimeStamp = function () {
    return (new Date).getTime() | 0;
}

Utilities.Ping = function(endPoint, connectionState) {
    with(Utilities) {
        this.timeStamp = this.GetTimeStamp();
        this.websocket = new WebSocket(endPoint);

        this.websocket.onopen = function(evt) {
            connectionState(true, (GetTimeStamp() - this.timeStamp));
            this.websocket = null;
        }.bind(this);

        this.websocket.onclose = function(evt) {
            connectionState(false, (GetTimeStamp() - this.timeStamp));
            this.websocket = null;
        }.bind(this);

        this.websocket.onerror = function(evt) {
            connectionState(false, (GetTimeStamp() - this.timeStamp));
            this.websocket = null;
        }.bind(this);
    }
}