var Utilities = Utilities || {};

Utilities.BiometricClient = function (endPoint, onOpen, onMessage, onError, onClose) {
    this.clientSocket = new WebSocket(endPoint);

    console.log("BiometricClient.Constructor");

    this.Send = function (message) {
        this.clientSocket.send(message);
    }

    this.clientSocket.onopen = function (evt) {
        console.log("BiometricClient.OnOpen");

        if (onOpen != null) {
            onOpen(evt);
        }
    }.bind(this);

    this.clientSocket.onmessage = function (message) {
        console.log("BiometricClient.OnMessage");

        onMessage(message);
    }.bind(this);

    this.clientSocket.onclose = function (evt) {
        console.log("BiometricClient.OnClose");

        if (onClose != null) {
            onClose(evt)
        }
        this.clientSocket = null;
    }.bind();

    this.Close = function () {
        if (this.clientSocket != null) {
            this.clientSocket.close();
            this.clientSocket = null;
        }
    }.bind(this);
}