"use strict";

var root = document.getElementById("root");
var table = document.createElement("table");
for (var y = 0; y < 24; y++) {
    var tr = document.createElement("tr");
    for (var x = 0; x < 32; x++) {
        var td = document.createElement("td");
        td.id = "pixel_".concat(y * 32 + x);
        tr.appendChild(td);
    }
    table.appendChild(tr);
}

root.appendChild(table);

var connection = new signalR.HubConnectionBuilder().withUrl("/cameraHub").build();

connection.on("ReceiveFrame", function (frame) {

    const min = Number(frame.min);
    let max = Number(frame.max);

    if (max - 1 < min) {
        max++;
    }

    frame.pixels.forEach(function (pixel, idx) {

        const temp = Number(pixel);
        const normalised = Math.floor(255 * (temp - min) / (max - min));
        const pixelElement = document.getElementById("pixel_".concat(idx));
        const bgColor = "rgb(" + normalised.toString() + ", 0, " + (255 - normalised).toString() + ")";
        pixelElement.style.backgroundColor = bgColor;
        pixelElement.textContent = temp.toFixed(1);
    });
});
connection.start()["catch"](function (err) {
    return console.error(err.toString());
});