var Utilities = Utilities || {};

Utilities.BiometricClient = function (endPoint, onOpen, onMessage, onError, onClose) {
    console.log("BiometricClient");

    this.clientSocket = new WebSocket(endPoint);

    this.Send = function (message) {
        console.log("Before Send");
        this.clientSocket.send(message);
        console.log("After Send");
    }.bind(this);

    this.clientSocket.onopen = function (evt) {
        console.log("OnOpen");
        if (onOpen != null) {
            onOpen(evt);
        }
    }.bind(this);

    this.clientSocket.onmessage = function (message) {
        console.log("OnMessage");
        onMessage(message);
    }.bind(this);

    this.clientSocket.onclose = function (evt) {
        console.log("OnClose");
        if (onClose != null) {
            onClose(evt)
        }
        this.clientSocket = null;
    }.bind();

    this.Close = function () {
        console.log("Close");
        if (this.clientSocket != null) {
            this.clientSocket.close();
            this.clientSocket = null;
        }
    }.bind(this);
}